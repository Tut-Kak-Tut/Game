using System.Collections.Generic;
using UnityEngine;

namespace Game.Combat
{
    public static class CombatTargeting
    {
        public static List<ICombatant> ResolveInRadius(Vector2 origin, float radius, int maxTargets, int casterTeamId)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, radius);
            List<ICombatant> result = new List<ICombatant>(maxTargets);

            for (int i = 0; i < hits.Length; i++)
            {
                if (!hits[i].TryGetComponent(out CombatantBehaviour combatant))
                {
                    continue;
                }

                if (!combatant.IsAlive || combatant.TeamId == casterTeamId)
                {
                    continue;
                }

                result.Add(combatant);
                if (result.Count >= maxTargets)
                {
                    break;
                }
            }

            return result;
        }
    }
}
