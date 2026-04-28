using Game.CoreRuntime;
using UnityEngine;

namespace Game.Combat
{
    [RequireComponent(typeof(CharacterStats))]
    public class CombatantBehaviour : MonoBehaviour, ICombatant
    {
        [SerializeField] private string combatantId;
        [SerializeField] private int teamId;
        [SerializeField] private CharacterStats characterStats;

        public string CombatantId => string.IsNullOrWhiteSpace(combatantId) ? gameObject.name : combatantId;
        public int TeamId => teamId;
        public bool IsAlive => characterStats != null && characterStats.currentHealth > 0f;
        public RuntimeStatBlock Stats { get; } = new RuntimeStatBlock();
        public Transform TargetTransform => transform;
        public bool IsTargetable => IsAlive;

        private void Awake()
        {
            if (characterStats == null)
            {
                characterStats = GetComponent<CharacterStats>();
            }

            SyncFromCharacterStats();
        }

        private void Update()
        {
            Stats.Tick(Time.deltaTime);
        }

        public void ApplyDamage(float amount, DamageType damageType, GameObject source)
        {
            if (characterStats == null)
            {
                return;
            }

            global::DamageType mapped = global::DamageType.True;
            if (damageType == DamageType.Physical) mapped = global::DamageType.Physical;
            else if (damageType == DamageType.Magic) mapped = global::DamageType.Magic;

            characterStats.TakeDamage(amount, mapped);
        }

        public void ApplyHealing(float amount, GameObject source)
        {
            if (characterStats == null)
            {
                return;
            }

            characterStats.currentHealth = Mathf.Min(characterStats.currentHealth + amount, characterStats.maxHealth);
        }

        public void SyncFromCharacterStats()
        {
            if (characterStats == null)
            {
                return;
            }

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
