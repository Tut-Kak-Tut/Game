using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterStats stats; // Ссылка на наш новый скрипт
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.7f;
    
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashStaminaCost = 25f; // Стоимость рывка

    [Header("Sprint Settings")]
    [SerializeField] private float sprintStaminaCost = 15f; // Трата стамины в СЕКУНДУ при беге
    
    private Vector2 moveInput;
    private Vector2 lastMoveDirection = Vector2.down;
    private bool isDashing;
    private bool isSprinting; 
    private bool canDash = true;

    private void Awake()
    {
        // Если забыл перетащить в инспекторе, попробуем найти на этом же объекте
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
        // Проверяем: нажата кнопка + КД прошел + мы не в рывке + ЕСТЬ СТАМИНА
        if (value.isPressed && canDash && !isDashing && stats.UseStamina(dashStaminaCost))
        {
            StartCoroutine(DashRoutine());
        }
    }

    void Update()
    {
        if (isDashing) return;

        // Логика траты стамины при спринте
        HandleSprintStamina();
        UpdateAnimations();
    }

    private void HandleSprintStamina()
    {
        if (isSprinting && moveInput.sqrMagnitude > 0)
        {
            // Пытаемся потратить стамину. UseStamina вернет false, если её нет.
            bool hasStamina = stats.UseStamina(sprintStaminaCost * Time.deltaTime);
            
            if (!hasStamina)
            {
                isSprinting = false; // Принудительно выключаем бег, если устали
            }
        }
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        // Скорость зависит от того, бежим ли мы (isSprinting уже учитывает наличие стамины)
        float currentSpeed = isSprinting ? walkSpeed * sprintMultiplier : walkSpeed;
        rb.linearVelocity = moveInput * currentSpeed;
    }

    private void UpdateAnimations()
    {
        if (animator != null)
        {
            animator.SetFloat("Horizontal", moveInput.x);
            animator.SetFloat("Vertical", moveInput.y);
            
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