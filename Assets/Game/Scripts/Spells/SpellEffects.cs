using Game.Combat;
using UnityEngine;

namespace Game.Spells
{
    public readonly struct SpellContext
    {
        public readonly CombatantBehaviour Caster;
        public readonly GameObject ExplicitTarget;
        public readonly Vector3 TargetPoint;
        public readonly Vector2 Direction;
        public readonly int Seed;

        public SpellContext(CombatantBehaviour caster, GameObject explicitTarget, Vector3 targetPoint, Vector2 direction, int seed)
        {
            Caster = caster;
            ExplicitTarget = explicitTarget;
            TargetPoint = targetPoint;
            Direction = direction;
            Seed = seed;
        }
    }

    public abstract class SpellEffectDefinition : ScriptableObject
    {
        public abstract void Execute(SpellContext context);
    }

    [CreateAssetMenu(fileName = "DirectDamageEffect", menuName = "RPG/Spells/Effects/Direct Damage")]
    public class DirectDamageEffect : SpellEffectDefinition
    {
        public float amount = 10f;
        public DamageType damageType = DamageType.Magic;

        public override void Execute(SpellContext context)
        {
            if (context.ExplicitTarget == null || !context.ExplicitTarget.TryGetComponent(out CombatantBehaviour target))
            {
                return;
            }

            target.ApplyDamage(amount, damageType, context.Caster != null ? context.Caster.gameObject : null);
        }
    }

    [CreateAssetMenu(fileName = "HealEffect", menuName = "RPG/Spells/Effects/Heal")]
    public class HealEffect : SpellEffectDefinition
    {
        public float amount = 12f;

        public override void Execute(SpellContext context)
        {
            CombatantBehaviour target = context.Caster;
            if (context.ExplicitTarget != null && context.ExplicitTarget.TryGetComponent(out CombatantBehaviour explicitTarget))
            {
                target = explicitTarget;
            }

            if (target != null)
            {
                target.ApplyHealing(amount, context.Caster != null ? context.Caster.gameObject : null);
            }
        }
    }

    [CreateAssetMenu(fileName = "ApplyStatModifierEffect", menuName = "RPG/Spells/Effects/Apply Stat Modifier")]
    public class ApplyStatModifierEffect : SpellEffectDefinition
    {
        public RuntimeStatId stat;
        public ModifierKind kind = ModifierKind.Additive;
        public float value = 3f;
        public float duration = 4f;

        public override void Execute(SpellContext context)
        {
            CombatantBehaviour target = context.Caster;
            if (context.ExplicitTarget != null && context.ExplicitTarget.TryGetComponent(out CombatantBehaviour explicitTarget))
            {
                target = explicitTarget;
            }

            if (target == null)
            {
                return;
            }

            target.Stats.AddModifier(new RuntimeStatModifier
            {
                SourceId = $"{name}_{context.Seed}",
                Stat = stat,
                Kind = kind,
                Value = value,
                Duration = duration
            });
        }
    }
}
