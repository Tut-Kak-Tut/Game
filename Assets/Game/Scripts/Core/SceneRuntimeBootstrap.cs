using UnityEngine;
using Game.Combat;
using Game.CoreRuntime;
using Game.Skills;

public class SceneRuntimeBootstrap : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private GameSession gameSessionPrefab;
    [SerializeField] private Transform playerTarget;
    [SerializeField] private DamageTextManager damageTextManager;
    [SerializeField] private EnemyAI[] enemyAgents;
    [SerializeField] private CharacterStats[] statsReceivers;
    [SerializeField] private EffectHandler[] effectReceivers;

    [Header("Death Mode")]
    [SerializeField] private DeathModeConfig deathModeConfig;
    [SerializeField] private Transform hubSpawnPoint;
    [SerializeField] private CharacterStats playerStats;
    [SerializeField] private DeathMode initialDeathMode = DeathMode.Standard;

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

        InitializeDeathMode();
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

    private void InitializeDeathMode()
    {
        if (GameSession.Instance == null || GameSession.Instance.DeathModeService == null) return;
        if (playerStats == null)
        {
            Debug.LogWarning("SceneRuntimeBootstrap: playerStats is not assigned — DeathModeService will not be initialized.", this);
            return;
        }

        CombatantBehaviour playerCombatant = playerStats.GetComponent<CombatantBehaviour>();
        string combatantId = playerCombatant != null ? playerCombatant.CombatantId : playerStats.gameObject.name;

        SkillProgressionService progression = playerStats.GetComponent<SkillProgressionService>();
        System.Func<int, int> xpLookup = progression != null ? (System.Func<int, int>)(_ => progression.GetXpToNextLevel()) : null;

        GameSession.Instance.DeathModeService.Initialize(
            initialDeathMode,
            deathModeConfig,
            hubSpawnPoint,
            playerStats,
            combatantId,
            xpLookup);
    }
}
