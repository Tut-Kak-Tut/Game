using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Spells
{
    public enum TargetingMode
    {
        Self,
        Unit,
        Position,
        Directional
    }

    [CreateAssetMenu(fileName = "SpellDefinition", menuName = "RPG/Spells/Spell Definition")]
    public class SpellDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string id;
        public string displayName;
        public Sprite icon;
        public List<string> tags = new();

        [Header("Timing")]
        public float castTime = 0.25f;
        public float recoverTime = 0.15f;
        public float cooldown = 2f;
        public bool interruptOnMove;

        [Header("Cost")]
        public float manaCost;
        public float staminaCost;
        public int chargeCost = 1;

        [Header("Targeting")]
        public TargetingMode targetingMode = TargetingMode.Unit;
        public float range = 6f;
        public int maxTargets = 1;
        public bool requiresLineOfSight;

        [Header("Effect Modules")]
        public List<SpellEffectDefinition> effects = new();
    }

    [Serializable]
    public struct SpellCastIntent
    {
        public SpellDefinition Spell;
        public GameObject ExplicitTarget;
        public Vector3 TargetPoint;
        public Vector2 Direction;
    }
}
