using System;
using System.Collections;
using System.Collections.Generic;
using Game.Combat;
using Game.CoreRuntime;
using UnityEngine;

namespace Game.Spells
{
    public enum SpellCastState
    {
        Idle,
        Priming,
        Casting,
        Recover,
        Cooldown
    }

    [Serializable]
    public struct SpellCooldownState
    {
        public string SpellId;
        public float RemainingSeconds;
    }

    [Serializable]
    public class PlayerSpellState
    {
        public List<string> unlockedSpellIds = new();
        public List<string> actionBarSpellIds = new();
        public List<SpellCooldownState> cooldowns = new();
    }

    [RequireComponent(typeof(CombatantBehaviour))]
    public class SpellCasterRuntime : MonoBehaviour, ISaveDataProvider
    {
        [SerializeField] private List<SpellDefinition> knownDefinitions = new();
        [SerializeField] private CharacterStats characterStats;

        public SpellCastState State { get; private set; } = SpellCastState.Idle;
        public string SaveId => "spells.player";
        public IReadOnlyList<SpellDefinition> Definitions => knownDefinitions;

        private readonly Dictionary<string, float> cooldowns = new();
        private CombatantBehaviour caster;
        private bool isCasting;

        private void Awake()
        {
            caster = GetComponent<CombatantBehaviour>();
            if (characterStats == null)
            {
                characterStats = GetComponent<CharacterStats>();
            }

            if (GameSession.Instance != null)
            {
                GameSession.Instance.SaveService.RegisterProvider(this);
            }
        }

        private void Update()
        {
            if (cooldowns.Count == 0)
            {
                return;
            }

            List<string> keys = new List<string>(cooldowns.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                cooldowns[keys[i]] = Mathf.Max(0f, cooldowns[keys[i]] - Time.deltaTime);
            }
        }

        public bool TryCast(SpellCastIntent intent)
        {
            if (isCasting || intent.Spell == null)
            {
                return false;
            }

            if (cooldowns.TryGetValue(intent.Spell.id, out float cd) && cd > 0f)
            {
                return false;
            }

            if (characterStats != null && characterStats.currentMana < intent.Spell.manaCost)
            {
                return false;
            }

            StartCoroutine(CastRoutine(intent));
            return true;
        }

        public float GetCooldownRemaining(string spellId)
        {
            return cooldowns.TryGetValue(spellId, out float value) ? value : 0f;
        }

        private IEnumerator CastRoutine(SpellCastIntent intent)
        {
            isCasting = true;
            State = SpellCastState.Priming;
            GameSession.Instance?.EventBus.Publish(new SpellCastStartEvent(intent.Spell.id));
            yield return new WaitForSeconds(intent.Spell.castTime);

            State = SpellCastState.Casting;
            SpellContext context = new SpellContext(caster, intent.ExplicitTarget, intent.TargetPoint, intent.Direction, UnityEngine.Random.Range(0, int.MaxValue));
            for (int i = 0; i < intent.Spell.effects.Count; i++)
            {
                intent.Spell.effects[i].Execute(context);
            }

            if (characterStats != null)
            {
                characterStats.currentMana = Mathf.Max(0f, characterStats.currentMana - intent.Spell.manaCost);
                characterStats.UseStamina(intent.Spell.staminaCost);
            }

            GameSession.Instance?.EventBus.Publish(new SpellResolvedEvent(intent.Spell.id));
            State = SpellCastState.Recover;
            yield return new WaitForSeconds(intent.Spell.recoverTime);

            State = SpellCastState.Cooldown;
            cooldowns[intent.Spell.id] = intent.Spell.cooldown;
            State = SpellCastState.Idle;
            isCasting = false;
        }

        public object CaptureState()
        {
            PlayerSpellState state = new PlayerSpellState();
            for (int i = 0; i < knownDefinitions.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(knownDefinitions[i].id))
                {
                    state.unlockedSpellIds.Add(knownDefinitions[i].id);
                }
            }

            foreach (var pair in cooldowns)
            {
                state.cooldowns.Add(new SpellCooldownState { SpellId = pair.Key, RemainingSeconds = pair.Value });
            }

            return state;
        }

        public void RestoreState(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            PlayerSpellState state = JsonUtility.FromJson<PlayerSpellState>(json);
            if (state == null)
            {
                return;
            }

            cooldowns.Clear();
            for (int i = 0; i < state.cooldowns.Count; i++)
            {
                cooldowns[state.cooldowns[i].SpellId] = state.cooldowns[i].RemainingSeconds;
            }
        }
    }
}
