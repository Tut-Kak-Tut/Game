using UnityEngine;

namespace Game.Combat
{
    [CreateAssetMenu(menuName = "RPG System/Melee Weapon Data", fileName = "MeleeWeaponData")]
    public class MeleeWeaponData : ScriptableObject
    {
        public float damage = 15f;
        public float hitRadius = 0.8f;
        public float hitOffset = 0.6f;
        public float hitFrameDelay = 0.1f;
        public float animDuration = 0.4f;
        public float cooldown = 0.5f;
    }
}
