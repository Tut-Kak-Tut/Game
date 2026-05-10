using UnityEngine;

namespace Game.Combat
{
    public abstract class WeaponData : ScriptableObject
    {
        [Header("Identity")]
        public string weaponId;
        public string displayName;

        [Header("Combat")]
        public DamageType damageType = DamageType.Physical;
        public float damage = 15f;
        public float cooldown = 0.5f;
        public float animDuration = 0.4f;
        public float hitFrameDelay = 0.1f;

        [Header("Animation")]
        public string attackAnimTrigger = "Attack";

        public abstract bool PerformAttack(PlayerWeaponAttack invoker, Vector2 facing, GameObject sourceGO);
    }
}
