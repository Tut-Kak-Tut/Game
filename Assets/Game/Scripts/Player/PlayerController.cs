using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    
    private Vector2 moveInput;
    private bool isDashing;
    private bool canDash = true;

    void Update()
    {
        if (isDashing) return;

        // Получение ввода (WASD или стрелки)
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Нормализация вектора, чтобы по диагонали не бегать быстрее
        if (moveInput != Vector2.zero)
        {
            moveInput.Normalize();
        }

        // Установка параметров анимации
        animator.SetFloat("Horizontal", moveInput.x);
        animator.SetFloat("Vertical", moveInput.y);
        animator.SetFloat("Speed", moveInput.sqrMagnitude);

        // Рывок на пробел (Space)
        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        // Физическое перемещение
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        // Если игрок не движется, рывок идет в сторону последнего направления (или вправо по дефолту)
        Vector2 dashDir = moveInput == Vector2.zero ? Vector2.right : moveInput;
        
        rb.velocity = dashDir * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        rb.velocity = Vector2.zero; // Остановка после рывка

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}