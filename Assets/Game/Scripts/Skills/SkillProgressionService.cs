using System;
using System.Collections.Generic;
using Game.Combat;
using Game.CoreRuntime;
using UnityEngine;

namespace Game.Skills
{
    public enum SkillRespecMode
    {
        FullReset,
        BranchReset,
        SafeReset
    }

    [Serializable]
    public sealed class SkillTreeState
    {
        public int Level = 1;
        public int CurrentXp;
        public int AvailablePoints;
        public List<string> UnlockedNodeIds = new();
        public int RespecCount;
    }

    [RequireComponent(typeof(CombatantBehaviour))]
    public class SkillProgressionService : MonoBehaviour, ISaveDataProvider
    {
        [SerializeField] private SkillTreeDefinition skillTree;
        [SerializeField] private int baseXpPerLevel = 100;
        [SerializeField] private int pointsPerLevel = 1;
        [SerializeField] private int baseRespecCost = 100;

        public string SaveId => "skills.player";
        public SkillTreeState State { get; } = new SkillTreeState();

        private readonly HashSet<string> unlocked = new();
        private CombatantBehaviour combatant;
        private IDisposable xpSubscription;
        private IDisposable xpDeductSubscription;

        private void Awake()
        {
            combatant = GetComponent<CombatantBehaviour>();
            if (GameSession.Instance != null)
            {
                GameSession.Instance.SaveService.RegisterProvider(this);
                xpSubscription = GameSession.Instance.EventBus.Subscribe<XpGrantedEvent>(OnXpGranted);
                xpDeductSubscription = GameSession.Instance.EventBus.Subscribe<XpDeductedEvent>(OnXpDeducted);
            }
        }

        private void OnDestroy()
        {
            xpSubscription?.Dispose();
            xpDeductSubscription?.Dispose();
        }

        public int GetXpToNextLevel() => GetXpThreshold(State.Level);

        public bool TryUnlockNode(SkillNodeDefinition node)
        {
            if (node == null || unlocked.Contains(node.nodeId))
            {
                return false;
            }

            if (State.AvailablePoints < node.cost || !MeetsPrerequisites(node))
            {
                return false;
            }

            State.AvailablePoints -= node.cost;
            unlocked.Add(node.nodeId);
            State.UnlockedNodeIds.Add(node.nodeId);
            ApplyPassiveBonuses(node, true);
            return true;
        }

        public bool Respec(SkillRespecMode mode, string branchRootId = null)
        {
            int requiredCurrency = baseRespecCost + (State.RespecCount * 50);
            _ = requiredCurrency; // currency integration can be wired to inventory/economy later.

            if (mode == SkillRespecMode.SafeReset && HasDependentNodes(branchRootId))
            {
                return false;
            }

            if (mode == SkillRespecMode.BranchReset && !string.IsNullOrWhiteSpace(branchRootId))
            {
                ResetBranch(branchRootId);
            }
            else
            {
                ResetAll();
            }

            State.RespecCount++;
            return true;
        }

        public object CaptureState()
        {
            return State;
        }

        public void RestoreState(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            SkillTreeState restored = JsonUtility.FromJson<SkillTreeState>(json);
            if (restored == null)
            {
                return;
            }

            ResetAll();
            State.Level = restored.Level;
            State.CurrentXp = restored.CurrentXp;
            State.AvailablePoints = restored.AvailablePoints;
            State.RespecCount = restored.RespecCount;
            for (int i = 0; i < restored.UnlockedNodeIds.Count; i++)
            {
                SkillNodeDefinition node = FindNode(restored.UnlockedNodeIds[i]);
                if (node != null)
                {
                    unlocked.Add(node.nodeId);
                    State.UnlockedNodeIds.Add(node.nodeId);
                    ApplyPassiveBonuses(node, true);
                }
            }
        }

        private void OnXpGranted(XpGrantedEvent xpEvent)
        {
            State.CurrentXp += Mathf.Max(0, xpEvent.Amount);
            while (State.CurrentXp >= GetXpThreshold(State.Level))
            {
                State.CurrentXp -= GetXpThreshold(State.Level);
                State.Level++;
                State.AvailablePoints += pointsPerLevel;
                GameSession.Instance?.EventBus.Publish(new LevelUpEvent(State.Level));
            }
        }

        private void OnXpDeducted(XpDeductedEvent evt)
        {
            State.CurrentXp = Mathf.Max(0, State.CurrentXp - Mathf.Max(0, evt.Amount));
        }

        private int GetXpThreshold(int level) => baseXpPerLevel + (level - 1) * 25;

        private bool MeetsPrerequisites(SkillNodeDefinition node)
        {
            for (int i = 0; i < node.prerequisites.Count; i++)
            {
                SkillPrerequisite prereq = node.prerequisites[i];
                switch (prereq.Type)
                {
                    case SkillPrerequisiteType.Level:
                        if (State.Level < prereq.RequiredLevel) return false;
                        break;
                    case SkillPrerequisiteType.ParentNode:
                        if (!unlocked.Contains(prereq.ReferenceId)) return false;
                        break;
                    case SkillPrerequisiteType.QuestComplete:
                        // Quest system is event-driven; this hook can check a quest service later.
                        return false;
                    case SkillPrerequisiteType.TagOwned:
                        // Tag ownership can be connected to player profile/archetype in future wave.
                        return false;
                }
            }

            return true;
        }

        private void ApplyPassiveBonuses(SkillNodeDefinition node, bool add)
        {
            float mul = add ? 1f : -1f;
            for (int i = 0; i < node.passiveBonuses.Count; i++)
            {
                PassiveStatBonus bonus = node.passiveBonuses[i];
                combatant.Stats.AddModifier(new RuntimeStatModifier
                {
                    SourceId = $"skill_{node.nodeId}",
                    Stat = bonus.Stat,
                    Kind = bonus.Kind,
                    Value = bonus.Value * mul,
                    Duration = 0f
                });
            }
        }

        private SkillNodeDefinition FindNode(string nodeId)
        {
            if (skillTree == null)
            {
                return null;
            }

            for (int i = 0; i < skillTree.nodes.Count; i++)
            {
                if (skillTree.nodes[i] != null && skillTree.nodes[i].nodeId == nodeId)
                {
                    return skillTree.nodes[i];
                }
            }

            return null;
        }

        private void ResetAll()
        {
            for (int i = 0; i < State.UnlockedNodeIds.Count; i++)
            {
                SkillNodeDefinition node = FindNode(State.UnlockedNodeIds[i]);
                if (node != null)
                {
                    combatant.Stats.RemoveSource($"skill_{node.nodeId}");
                }
            }

            State.UnlockedNodeIds.Clear();
            unlocked.Clear();
            State.AvailablePoints = Mathf.Max(State.AvailablePoints, 0);
        }

        private void ResetBranch(string branchRootId)
        {
            List<string> toRemove = new List<string>();
            for (int i = 0; i < State.UnlockedNodeIds.Count; i++)
            {
                if (State.UnlockedNodeIds[i] == branchRootId)
                {
                    toRemove.Add(State.UnlockedNodeIds[i]);
                }
            }

            for (int i = 0; i < toRemove.Count; i++)
            {
                unlocked.Remove(toRemove[i]);
                State.UnlockedNodeIds.Remove(toRemove[i]);
                combatant.Stats.RemoveSource($"skill_{toRemove[i]}");
                State.AvailablePoints += 1;
            }
        }

        private bool HasDependentNodes(string nodeId)
        {
            if (string.IsNullOrWhiteSpace(nodeId) || skillTree == null)
            {
                return false;
            }

            for (int i = 0; i < skillTree.nodes.Count; i++)
            {
                SkillNodeDefinition node = skillTree.nodes[i];
                if (node == null)
                {
                    continue;
                }

                for (int p = 0; p < node.prerequisites.Count; p++)
                {
                    if (node.prerequisites[p].Type == SkillPrerequisiteType.ParentNode &&
                        node.prerequisites[p].ReferenceId == nodeId &&
                        unlocked.Contains(node.nodeId))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
