using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterStats stats;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [Header("Movement Settings")]
    [SerializeField] private float sprintMultiplier = 1.6f;
    
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeedMultiplier = 3f; // Рывок в 3 раза быстрее ходьбы
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.8f;
    [SerializeField] private float dashStaminaCost = 20f;

    [Header("Sprint Settings")]
    [SerializeField] private float sprintStaminaCost = 12f;
    
    private Vector2 moveInput;
    private Vector2 lastMoveDirection = Vector2.down;
    private bool isDashing;
    private bool isSprinting;
    private bool canDash = true;

    public Vector2 FacingDirection => lastMoveDirection.sqrMagnitude > 0.01f ? lastMoveDirection : Vector2.down;
    public bool IsDashing => isDashing;

    private void Awake()
    {
        if (stats == null) stats = GetComponent<CharacterStats>();
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        if (moveInput != Vector2.zero) lastMoveDirection = moveInput.normalized;
    }

    private void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed;
    }

    private void OnDash(InputValue value)
    {
        if (value.isPressed && canDash && !isDashing && stats.UseStamina(dashStaminaCost))
        {
            StartCoroutine(DashRoutine());
        }
    }

    void Update()
    {
        if (isDashing) return;

        HandleSprintLogic();
        UpdateAnimations();
    }

    private void HandleSprintLogic()
    {
        // Если кнопка зажата и мы идем
        if (isSprinting && moveInput.sqrMagnitude > 0)
        {
            bool hasStamina = stats.UseStamina(sprintStaminaCost * Time.deltaTime);
            if (!hasStamina) isSprinting = false;
        }
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        // Берем скорость прямо из CharacterStats (которая зависит от Ловкости)
        float baseSpeed = stats.walkSpeed;
        float currentSpeed = isSprinting ? baseSpeed * sprintMultiplier : baseSpeed;
        
        rb.linearVelocity = moveInput * currentSpeed;
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;

        animator.SetFloat("Horizontal", moveInput.x);
        animator.SetFloat("Vertical", moveInput.y);
        
        float animSpeed = (isSprinting && moveInput.sqrMagnitude > 0) ? sprintMultiplier : moveInput.sqrMagnitude;
        animator.SetFloat("Speed", animSpeed);
        
        if (moveInput.sqrMagnitude > 0.01f)
        {
            animator.SetFloat("LastMoveX", moveInput.x);
            animator.SetFloat("LastMoveY", moveInput.y);
        }
    }

    private IEnumerator DashRoutine()
    {
        canDash = false;
        isDashing = true;

        Vector2 dashDir = moveInput != Vector2.zero ? moveInput.normalized : lastMoveDirection;
        // Скорость рывка теперь тоже отталкивается от статов ловкости
        rb.linearVelocity = dashDir * (stats.walkSpeed * dashSpeedMultiplier);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}