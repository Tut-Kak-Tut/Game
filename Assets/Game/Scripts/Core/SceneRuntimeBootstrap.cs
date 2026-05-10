using UnityEngine;
using Game.CoreRuntime;

public class SceneRuntimeBootstrap : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private GameSession gameSessionPrefab;
    [SerializeField] private Transform playerTarget;
    [SerializeField] private DamageTextManager damageTextManager;
    [SerializeField] private EnemyAI[] enemyAgents;
    [SerializeField] private CharacterStats[] statsReceivers;
    [SerializeField] private EffectHandler[] effectReceivers;

    private void Awake()
    {
        EnsureGameSession();

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

    private void EnsureGameSession()
    {
        if (GameSession.Instance != null)
        {
            return;
        }

        if (gameSessionPrefab != null)
        {
            Instantiate(gameSessionPrefab);
        }
        else
        {
            GameObject fallback = new GameObject("GameSession");
            fallback.AddComponent<GameSession>();
        }
    }
}
