using Game.CoreRuntime;
using Game.Spells;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Combat
{
    [RequireComponent(typeof(CharacterStats))]
    public class CombatantBehaviour : MonoBehaviour, ICombatant
    {
        [SerializeField] private string combatantId;
        [SerializeField] private int teamId;
        [SerializeField] private CharacterStats characterStats;

        [Header("Combat Tuning")]
        [SerializeField] private DeathModeConfig combatTuning;
        [SerializeField] private Vector2 knockbackForce = new Vector2(3f, 0f);
        [SerializeField] private bool useKnockback = true;

        public string CombatantId => string.IsNullOrWhiteSpace(combatantId) ? gameObject.name : combatantId;
        public int TeamId => teamId;
        public bool IsAlive => characterStats != null && !characterStats.IsDead && characterStats.currentHealth > 0f;
        public RuntimeStatBlock Stats { get; } = new RuntimeStatBlock();
        public Transform TargetTransform => transform;
        public bool IsTargetable => IsAlive;
        public bool IsStaggered => _staggerTimer > 0f;
        public bool IsInIframes => _iframeTimer > 0f;

        private Rigidbody2D _rb2D;
        private Collider2D _collider;
        private Animator _animator;
        private EffectHandler _effectHandler;
        private SpellCasterRuntime _spellCaster;
        private NavMeshAgent _navAgent;

        private float _iframeTimer;
        private float _staggerTimer;
        private bool _deathSubscribed;

        private void Awake()
        {
            if (characterStats == null)
                characterStats = GetComponent<CharacterStats>();

            _rb2D = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _animator = GetComponent<Animator>();
            _effectHandler = GetComponent<EffectHandler>();
            _spellCaster = GetComponent<SpellCasterRuntime>();
            _navAgent = GetComponent<NavMeshAgent>();

            characterStats.SetCombatantId(CombatantId);

            SyncFromCharacterStats();
        }

        private void OnEnable()
        {
            if (characterStats != null && !_deathSubscribed)
            {
                characterStats.OnDeath += HandleOwnDeath;
                _deathSubscribed = true;
            }
        }

        private void OnDisable()
        {
            if (characterStats != null && _deathSubscribed)
            {
                characterStats.OnDeath -= HandleOwnDeath;
                _deathSubscribed = false;
            }
        }

        private void Update()
        {
            Stats.Tick(Time.deltaTime);

            if (_iframeTimer > 0f) _iframeTimer -= Time.deltaTime;
            if (_staggerTimer > 0f) _staggerTimer -= Time.deltaTime;
        }

        public void ApplyDamage(float amount, DamageType damageType, GameObject source)
        {
            if (characterStats == null || !IsAlive) return;
            if (_iframeTimer > 0f) return;

            float scaled = amount;
            bool wasCrit = false;
            string sourceId = source != null ? source.name : string.Empty;

            CharacterStats sourceStats = source != null ? source.GetComponent<CharacterStats>() : null;
            if (sourceStats != null)
            {
                if (damageType == DamageType.Physical)
                    scaled *= 1f + sourceStats.FinalStr * 0.02f;
                else if (damageType == DamageType.Magic)
                    scaled *= 1f + sourceStats.FinalInt * 0.02f;

                float critChance = sourceStats.critChance;
                if (combatTuning != null && critChance > 0f)
                {
                    if (Random.value * 100f < critChance)
                    {
                        wasCrit = true;
                        scaled *= combatTuning.critMultiplier;
                    }
                }

                CombatantBehaviour sourceCombatant = source.GetComponent<CombatantBehaviour>();
                if (sourceCombatant != null)
                    sourceId = sourceCombatant.CombatantId;
            }

            characterStats.TakeDamage(scaled, damageType, sourceId, wasCrit);

            float iframeWindow = combatTuning != null ? combatTuning.iframesAfterHit : 0.4f;
            _iframeTimer = iframeWindow;

            float ratio = characterStats.maxHealth > 0f ? scaled / characterStats.maxHealth : 0f;
            float staggerThreshold = combatTuning != null ? combatTuning.staggerHealthThreshold : 0.15f;
            if (ratio >= staggerThreshold)
            {
                _staggerTimer = combatTuning != null ? combatTuning.staggerDuration : 0.3f;
                _spellCaster?.Interrupt();
                _animator?.SetTrigger("Stagger");
            }

            _effectHandler?.TriggerHitFlash();
            _animator?.SetTrigger("Hit");

            if (useKnockback && damageType == DamageType.Physical && _rb2D != null && source != null)
            {
                Vector2 dir = ((Vector2)transform.position - (Vector2)source.transform.position).normalized;
                if (dir.sqrMagnitude > 0.001f)
                    _rb2D.AddForce(dir * knockbackForce.x, ForceMode2D.Impulse);
            }

            GameSession.Instance?.EventBus.Publish(new DamageDealtEvent(
                sourceId,
                CombatantId,
                scaled,
                damageType,
                wasCrit));
        }

        public void ApplyHealing(float amount, GameObject source)
        {
            if (characterStats == null) return;
            characterStats.Heal(amount);
        }

        private void HandleOwnDeath()
        {
            if (_collider != null) _collider.enabled = false;
            if (_navAgent != null && _navAgent.isOnNavMesh) _navAgent.isStopped = true;
            _animator?.SetTrigger("Death");
        }

        public void SyncFromCharacterStats()
        {
            if (characterStats == null) return;

            Stats.SetBase(RuntimeStatId.MaxHealth, characterStats.maxHealth);
            Stats.SetBase(RuntimeStatId.MaxMana, characterStats.maxMana);
            Stats.SetBase(RuntimeStatId.MaxStamina, characterStats.maxStamina);
            Stats.SetBase(RuntimeStatId.PhysicalDamage, characterStats.physicalDamage);
            Stats.SetBase(RuntimeStatId.PhysicalArmor, characterStats.physicalArmor);
            Stats.SetBase(RuntimeStatId.MagicResistance, characterStats.magicResistance);
            Stats.SetBase(RuntimeStatId.MoveSpeed, characterStats.walkSpeed);
            Stats.SetBase(RuntimeStatId.CritChance, characterStats.critChance);
            Stats.SetBase(RuntimeStatId.SpellPower, characterStats.spellPower);
        }
    }
}
