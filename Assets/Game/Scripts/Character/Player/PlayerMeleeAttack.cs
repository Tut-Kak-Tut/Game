using System.Collections;
using Game.Combat;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CombatantBehaviour))]
public class PlayerMeleeAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MeleeWeaponData weaponData;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController controller;
    [SerializeField] private CombatantBehaviour combatant;

    [Header("Hit Detection")]
    [SerializeField] private LayerMask enemyLayer = ~0;

    private readonly Collider2D[] _hitBuffer = new Collider2D[8];
    private bool _onCooldown;

    private void Awake()
    {
        if (controller == null) controller = GetComponent<PlayerController>();
        if (combatant == null) combatant = GetComponent<CombatantBehaviour>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    private void OnAttack(InputValue value)
    {
        if (!value.isPressed) return;
        if (_onCooldown) return;
        if (weaponData == null) return;
        if (combatant == null || !combatant.IsAlive || combatant.IsStaggered) return;
        if (controller != null && controller.IsDashing) return;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        _onCooldown = true;
        animator?.SetTrigger("Attack");

        yield return new WaitForSeconds(weaponData.hitFrameDelay);

        Vector2 facing = controller != null ? controller.FacingDirection : Vector2.down;
        Vector2 origin = (Vector2)transform.position + facing * weaponData.hitOffset;

        int hitCount = Physics2D.OverlapCircleNonAlloc(origin, weaponData.hitRadius, _hitBuffer, enemyLayer);
        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = _hitBuffer[i];
            if (hit == null || hit.gameObject == gameObject) continue;

            CombatantBehaviour target = hit.GetComponentInParent<CombatantBehaviour>();
            if (target == null || !target.IsAlive) continue;
            if (target.TeamId == combatant.TeamId) continue;

            target.ApplyDamage(weaponData.damage, DamageType.Physical, gameObject);
        }

        float remaining = Mathf.Max(0f, weaponData.animDuration - weaponData.hitFrameDelay);
        if (remaining > 0f) yield return new WaitForSeconds(remaining);

        if (weaponData.cooldown > 0f) yield return new WaitForSeconds(weaponData.cooldown);

        _onCooldown = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (weaponData == null) return;
        Vector2 facing = controller != null ? controller.FacingDirection : Vector2.down;
        Vector3 origin = transform.position + (Vector3)(facing * weaponData.hitOffset);
        Gizmos.color = new Color(1f, 0.4f, 0.2f, 0.6f);
        Gizmos.DrawWireSphere(origin, weaponData.hitRadius);
    }
}
