using System;
using System.Collections.Generic;
using Game.Combat;
using UnityEngine;

namespace Game.Skills
{
    public enum SkillPrerequisiteType
    {
        Level,
        ParentNode,
        QuestComplete,
        TagOwned
    }

    [Serializable]
    public struct SkillPrerequisite
    {
        public SkillPrerequisiteType Type;
        public string ReferenceId;
        public int RequiredLevel;
    }

    [Serializable]
    public struct PassiveStatBonus
    {
        public RuntimeStatId Stat;
        public ModifierKind Kind;
        public float Value;
    }

    [CreateAssetMenu(fileName = "SkillNodeDefinition", menuName = "RPG/Skills/Skill Node")]
    public class SkillNodeDefinition : ScriptableObject
    {
        public string nodeId;
        public string displayName;
        public int tier;
        public int cost = 1;
        public string category;
        public List<SkillPrerequisite> prerequisites = new();
        public List<PassiveStatBonus> passiveBonuses = new();
    }

    [CreateAssetMenu(fileName = "SkillTreeDefinition", menuName = "RPG/Skills/Skill Tree")]
    public class SkillTreeDefinition : ScriptableObject
    {
        public List<SkillNodeDefinition> nodes = new();
    }
}
