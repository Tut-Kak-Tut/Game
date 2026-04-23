
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterStats))]
public class EnemyAI : MonoBehaviour
{
    public CharacterStats stats;
    public NavMeshAgent agent;

    [Header("Targeting")]
    public Transform target;
    public float detectionRange = 10f;
    public float stopDistance = 2f;
    public float attackRange = 2.5f;
    public float attackCooldown = 1.2f;

    [Header("Patrol")]
    public Transform[] patrolPoints;
    public float patrolWaitTime = 1.5f;

    private int currentPatrolIndex = 0;
    public float patrolWaitTimer = 0f;
    private bool waitingAtPoint = false;
    private bool chasingPlayer = false;
    private float nextAttackTime;

    void Start()
    {
        stats = GetComponent<CharacterStats>();
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
            agent = gameObject.AddComponent<NavMeshAgent>();
        agent.stoppingDistance = stopDistance;
        agent.speed = stats.walkSpeed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }

        // Переносим врага на ближайшую точку NavMesh, если он вне сетки
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1f, NavMesh.AllAreas))
            transform.position = hit.position;
    }

    void Update()
    {
        if (target == null || patrolPoints == null || patrolPoints.Length == 0) return;

        float playerDist = Vector3.Distance(transform.position, target.position);
        bool playerNear = playerDist <= detectionRange;

        if (playerNear)
        {
            chasingPlayer = true;
            waitingAtPoint = false;
            // Преследуем игрока, но держим дистанцию
            if (playerDist > stopDistance)
            {
                agent.isStopped = false;
                Vector3 dir = (transform.position - target.position).normalized;
                Vector3 stopPoint = target.position + dir * stopDistance;
                agent.SetDestination(stopPoint);
            }
            else
            {
                agent.isStopped = true;
            }
            if (playerDist <= attackRange)
                TryAttack();
        }
        else
        {
            // Если только что потеряли игрока — возвращаемся к ближайшей точке патруля
            if (chasingPlayer)
            {
                int nearest = 0;
                float minDist = float.MaxValue;
                for (int i = 0; i < patrolPoints.Length; i++)
                {
                    float d = Vector3.Distance(transform.position, patrolPoints[i].position);
                    if (d < minDist)
                    {
                        minDist = d;
                        nearest = i;
                    }
                }
                currentPatrolIndex = nearest;
                agent.isStopped = false;
                agent.SetDestination(patrolPoints[currentPatrolIndex].position);
                waitingAtPoint = false;
                chasingPlayer = false;
            }

            // Патрулируем по точкам
            if (!waitingAtPoint)
            {
                agent.isStopped = false;
                agent.SetDestination(patrolPoints[currentPatrolIndex].position);
                if (Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position) < 1f)
                {
                    waitingAtPoint = true;
                    patrolWaitTimer = 0f;;
                }
            }
            else
            {
                agent.isStopped = true;
                patrolWaitTimer += Time.deltaTime;
                if (patrolWaitTimer >= patrolWaitTime)
                {
                    waitingAtPoint = false;
                    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                }
            }
        }

        FlipSprite();
    }

    void FlipSprite()
    {
        Vector3 velocity = agent.velocity;
        if (velocity.x < -0.1f)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (velocity.x > 0.1f)
            transform.localScale = new Vector3(1, 1, 1);
    }

     // Останавливаем движение при столкновении с игроком
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (agent != null) agent.isStopped = true;
        }
    }

    // Возобновляем движение после выхода из столкновения
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (agent != null) agent.isStopped = false;
        }
    }
    void TryAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            CharacterStats playerStats = target.GetComponent<CharacterStats>();
            if (playerStats != null)
            {
                Debug.Log(gameObject.name + " ударил игрока!");
                playerStats.TakeDamage(stats.physicalDamage, DamageType.Physical);
            }
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}