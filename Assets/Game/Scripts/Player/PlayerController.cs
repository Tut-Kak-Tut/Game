using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.7f; // Во сколько раз быстрее бежим
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    
    private Vector2 moveInput;
    private Vector2 lastMoveDirection = Vector2.down;
    private bool isDashing;
    private bool isSprinting; // Флаг спринта
    private bool canDash = true;

    // Автоматически вызывается Action "Move"
    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        if (moveInput != Vector2.zero) lastMoveDirection = moveInput.normalized;
    }

    // Автоматически вызывается Action "Sprint" (при нажатии и отпускании)
    private void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed;
    }

    // Автоматически вызывается Action "Dash"
    private void OnDash(InputValue value)
    {
        if (value.isPressed && canDash && !isDashing)
        {
            StartCoroutine(DashRoutine());
        }
    }

    void Update()
    {
        if (isDashing) return;
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        // Вычисляем итоговую скорость
        float currentSpeed = isSprinting ? walkSpeed * sprintMultiplier : walkSpeed;
        rb.linearVelocity = moveInput * currentSpeed;
    }

    private void UpdateAnimations()
    {
        if (animator != null)
        {
            animator.SetFloat("Horizontal", moveInput.x);
            animator.SetFloat("Vertical", moveInput.y);
            
            // Если мы бежим, значение Speed в аниматоре будет выше (например, 1.7 вместо 1.0)
            // Это позволит тебе в Blend Tree сделать разную анимацию для ходьбы и бега
            float animSpeed = isSprinting && moveInput.sqrMagnitude > 0 ? sprintMultiplier : moveInput.sqrMagnitude;
            animator.SetFloat("Speed", animSpeed);
            
            if (moveInput.sqrMagnitude > 0.01f)
            {
                animator.SetFloat("LastMoveX", moveInput.x);
                animator.SetFloat("LastMoveY", moveInput.y);
            }
        }
    }

    private IEnumerator DashRoutine()
    {
        canDash = false;
        isDashing = true;

        Vector2 dashDir = moveInput != Vector2.zero ? moveInput.normalized : lastMoveDirection;
        rb.linearVelocity = dashDir * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}