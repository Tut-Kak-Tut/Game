using UnityEngine;

namespace Game.Combat
{
    [CreateAssetMenu(menuName = "RPG System/Weapons/Melee", fileName = "MeleeWeaponData")]
    public class MeleeWeaponData : WeaponData
    {
        [Header("Melee Specific")]
        public float hitRadius = 0.8f;
        public float hitOffset = 0.6f;

        public override bool PerformAttack(PlayerWeaponAttack invoker, Vector2 facing, GameObject sourceGO)
        {
            if (invoker == null || sourceGO == null) return false;

            Vector2 origin = (Vector2)sourceGO.transform.position + facing * hitOffset;
            int hits = Physics2D.OverlapCircleNonAlloc(origin, hitRadius, invoker.HitBuffer, invoker.EnemyLayer);

            int sourceTeam = invoker.Combatant != null ? invoker.Combatant.TeamId : 0;

            for (int i = 0; i < hits; i++)
            {
                Collider2D col = invoker.HitBuffer[i];
                if (col == null || col.gameObject == sourceGO) continue;

                CombatantBehaviour target = col.GetComponentInParent<CombatantBehaviour>();
                if (target == null || !target.IsAlive) continue;
                if (target.TeamId == sourceTeam) continue;

                target.ApplyDamage(damage, damageType, sourceGO);
            }

            return true;
        }
    }
}
