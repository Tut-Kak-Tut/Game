using UnityEngine;

namespace Game.CoreRuntime
{
    [CreateAssetMenu(menuName = "RPG System/Death Mode Config", fileName = "DeathModeConfig")]
    public class DeathModeConfig : ScriptableObject
    {
        [Header("Default Mode")]
        public DeathMode defaultMode = DeathMode.Standard;

        [Header("Standard Penalty")]
        [Range(0f, 1f)] public float xpPenaltyFraction = 0.10f;
        [Range(0f, 1f)] public float goldPenaltyFraction = 0.10f;

        [Header("Respawn FX")]
        public float respawnFadeOutDuration = 0.5f;
        public float respawnFadeInDuration = 0.5f;

        [Header("Combat Tuning")]
        public float iframesAfterHit = 0.4f;
        [Range(0f, 1f)] public float staggerHealthThreshold = 0.15f;
        public float staggerDuration = 0.3f;
        public float critMultiplier = 2f;
        public float critChancePerAgi = 0.5f;
    }
}
