using UnityEngine;

namespace Game.Combat
{
    public interface ITargetable
    {
        Transform TargetTransform { get; }
        bool IsTargetable { get; }
    }

    public interface IDamageable
    {
        void ApplyDamage(float amount, DamageType damageType, GameObject source);
        void ApplyHealing(float amount, GameObject source);
    }

    public interface ICombatant : ITargetable, IDamageable
    {
        string CombatantId { get; }
        int TeamId { get; }
        bool IsAlive { get; }
        RuntimeStatBlock Stats { get; }
    }
}
