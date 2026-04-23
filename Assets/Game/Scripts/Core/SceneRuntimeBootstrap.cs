using UnityEngine;

public class SceneRuntimeBootstrap : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Transform playerTarget;
    [SerializeField] private DamageTextManager damageTextManager;
    [SerializeField] private EnemyAI[] enemyAgents;
    [SerializeField] private CharacterStats[] statsReceivers;
    [SerializeField] private EffectHandler[] effectReceivers;

    private void Awake()
    {
        if (damageTextManager == null)
            damageTextManager = FindAnyObjectByType<DamageTextManager>();

        if (enemyAgents != null)
        {
            foreach (EnemyAI enemy in enemyAgents)
            {
                if (enemy != null && playerTarget != null)
                    enemy.SetTarget(playerTarget);
            }
        }

        if (statsReceivers != null)
        {
            foreach (CharacterStats stats in statsReceivers)
            {
                if (stats != null)
                    stats.SetDamageTextManager(damageTextManager);
            }
        }

        if (effectReceivers != null)
        {
            foreach (EffectHandler handler in effectReceivers)
            {
                if (handler != null)
                    handler.SetDamageTextManager(damageTextManager);
            }
        }
    }
}
