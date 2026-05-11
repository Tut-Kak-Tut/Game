using UnityEngine;

namespace Game.Combat
{
    [CreateAssetMenu(menuName = "RPG System/Weapons/Ranged", fileName = "RangedWeaponData")]
    public class RangedWeaponData : WeaponData
    {
        [Header("Ranged Specific")]
        public GameObject projectilePrefab;
        public float projectileSpeed = 12f;
        public float projectileLifetime = 2f;
        public float spawnOffset = 0.5f;

        public override bool PerformAttack(PlayerWeaponAttack invoker, Vector2 facing, GameObject sourceGO)
        {
            if (projectilePrefab == null || sourceGO == null)
            {
                Debug.LogWarning($"RangedWeaponData '{name}': projectilePrefab or sourceGO is null.");
                return false;
            }

            Vector2 spawnPos = (Vector2)sourceGO.transform.position + facing * spawnOffset;
            GameObject go = Object.Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

            Projectile projectile = go.GetComponent<Projectile>();
            if (projectile == null)
            {
                Debug.LogError($"RangedWeaponData '{name}': projectilePrefab has no Projectile component.");
                Object.Destroy(go);
                return false;
            }

            projectile.Init(damage, damageType, sourceGO, projectileSpeed, projectileLifetime, facing, invoker.EnemyLayer);
            return true;
        }
    }
}
