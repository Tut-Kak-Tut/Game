using System;
using System.Collections.Generic;

namespace Game.Combat
{
    public enum RuntimeStatId
    {
        MaxHealth,
        MaxMana,
        MaxStamina,
        PhysicalDamage,
        PhysicalArmor,
        MagicResistance,
        MoveSpeed,
        CritChance,
        SpellPower
    }

    public enum ModifierKind
    {
        Additive,
        Multiplicative
    }

    [Serializable]
    public struct RuntimeStatModifier
    {
        public string SourceId;
        public RuntimeStatId Stat;
        public ModifierKind Kind;
        public float Value;
        public float Duration;
    }

    public sealed class RuntimeStatBlock
    {
        private readonly Dictionary<RuntimeStatId, float> baseStats = new();
        private readonly List<RuntimeStatModifier> activeModifiers = new();

        public void SetBase(RuntimeStatId stat, float value) => baseStats[stat] = value;

        public float GetFinal(RuntimeStatId stat)
        {
            float baseValue = baseStats.TryGetValue(stat, out float v) ? v : 0f;
            float additive = 0f;
            float multiplicative = 1f;

            for (int i = 0; i < activeModifiers.Count; i++)
            {
                RuntimeStatModifier modifier = activeModifiers[i];
                if (modifier.Stat != stat)
                {
                    continue;
                }

                if (modifier.Kind == ModifierKind.Additive)
                {
                    additive += modifier.Value;
                }
                else
                {
                    multiplicative *= 1f + modifier.Value;
                }
            }

            return (baseValue + additive) * multiplicative;
        }

        public void AddModifier(RuntimeStatModifier modifier)
        {
            activeModifiers.Add(modifier);
        }

        public void RemoveSource(string sourceId)
        {
            activeModifiers.RemoveAll(m => m.SourceId == sourceId);
        }

        public void Tick(float deltaTime)
        {
            for (int i = activeModifiers.Count - 1; i >= 0; i--)
            {
                if (activeModifiers[i].Duration <= 0f)
                {
                    continue;
                }

                RuntimeStatModifier updated = activeModifiers[i];
                updated.Duration -= deltaTime;
                if (updated.Duration <= 0f)
                {
                    activeModifiers.RemoveAt(i);
                }
                else
                {
                    activeModifiers[i] = updated;
                }
            }
        }
    }
}
