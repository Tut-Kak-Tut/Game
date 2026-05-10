using System.Collections;
using Game.Combat;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CombatantBehaviour))]
[RequireComponent(typeof(PlayerEquipment))]
public class PlayerWeaponAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController controller;
    [SerializeField] private CombatantBehaviour combatant;
    [SerializeField] private PlayerEquipment equipment;

    [Header("Hit Detection")]
    [SerializeField] private LayerMask enemyLayer = ~0;

    public Collider2D[] HitBuffer { get; } = new Collider2D[8];
    public LayerMask EnemyLayer => enemyLayer;
    public CombatantBehaviour Combatant => combatant;

    private bool _onCooldown;

    private void Awake()
    {
        if (controller == null) controller = GetComponent<PlayerController>();
        if (combatant == null)  combatant  = GetComponent<CombatantBehaviour>();
        if (equipment == null)  equipment  = GetComponent<PlayerEquipment>();
        if (animator == null)   animator   = GetComponent<Animator>();
    }

    private void OnAttack(InputValue value)
    {
        if (!value.isPressed || _onCooldown) return;
        if (equipment == null) return;
        WeaponData weapon = equipment.CurrentWeapon;
        if (weapon == null) return;
        if (combatant == null || !combatant.IsAlive || combatant.IsStaggered) return;
        if (controller != null && controller.IsDashing) return;

        StartCoroutine(AttackRoutine(weapon));
    }

    private IEnumerator AttackRoutine(WeaponData weapon)
    {
        _onCooldown = true;

        if (!string.IsNullOrEmpty(weapon.attackAnimTrigger))
            animator?.SetTrigger(weapon.attackAnimTrigger);

        if (weapon.hitFrameDelay > 0f)
            yield return new WaitForSeconds(weapon.hitFrameDelay);

        Vector2 facing = controller != null ? controller.FacingDirection : Vector2.down;
        bool fired = weapon.PerformAttack(this, facing, gameObject);

        float remaining = Mathf.Max(0f, weapon.animDuration - weapon.hitFrameDelay);
        if (remaining > 0f) yield return new WaitForSeconds(remaining);

        if (fired && weapon.cooldown > 0f)
            yield return new WaitForSeconds(weapon.cooldown);

        _onCooldown = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (equipment == null || equipment.CurrentWeapon == null) return;
        if (controller == null) return;
        if (equipment.CurrentWeapon is MeleeWeaponData melee)
        {
            Vector2 facing = controller.FacingDirection;
            Vector3 origin = transform.position + (Vector3)(facing * melee.hitOffset);
            Gizmos.color = new Color(1f, 0.4f, 0.2f, 0.6f);
            Gizmos.DrawWireSphere(origin, melee.hitRadius);
        }
    }
}
