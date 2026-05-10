using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterStats playerStats;
    
    [Header("Bars Fill Images")]
    [SerializeField] private Image healthFill;
    [SerializeField] private Image staminaFill;

    [Header("Settings")]
    [SerializeField] private bool smoothUpdates = true;
    [SerializeField] private float smoothSpeed = 8f;

    private void OnEnable()
    {
        if (playerStats != null)
        {
            // Подписываемся на события: когда меняются ресурсы или сами статы
            playerStats.OnResourceChanged += RefreshUI;
            playerStats.OnStatsRecalculated += RefreshUI;
        }
    }

    private void OnDisable()
    {
        if (playerStats != null)
        {
            // Обязательно отписываемся, чтобы не было ошибок при удалении объекта
            playerStats.OnResourceChanged -= RefreshUI;
            playerStats.OnStatsRecalculated -= RefreshUI;
        }
    }

    // Этот метод вызывается ТОЛЬКО когда что-то изменилось
    private void RefreshUI()
    {
        if (smoothUpdates)
        {
            // Если хотим плавности, запускаем маленькую корутину или используем простой Update
            // Но для простоты оставим расчет в методе, который дергает событие
            UpdateBars();
        }
        else
        {
            // Мгновенное обновление
            healthFill.fillAmount = playerStats.currentHealth / playerStats.maxHealth;
            staminaFill.fillAmount = playerStats.currentStamina / playerStats.maxStamina;
        }
    }

    // Если ты хочешь ОЧЕНЬ плавную анимацию (Lerp), Update можно оставить, 
    // но расчет targetFill делать только по событию. 
    // Однако, самый чистый вариант для производительности — это мгновенное или корутинное обновление.
    
    void Update()
    {
        // Оставим только Lerp здесь для визуальной красоты, 
        // но теперь он работает с уже готовыми данными из CharacterStats
        if (playerStats == null) return;

        float hTarget = playerStats.currentHealth / playerStats.maxHealth;
        float sTarget = playerStats.currentStamina / playerStats.maxStamina;

        healthFill.fillAmount = Mathf.MoveTowards(healthFill.fillAmount, hTarget, Time.deltaTime * smoothSpeed);
        staminaFill.fillAmount = Mathf.MoveTowards(staminaFill.fillAmount, sTarget, Time.deltaTime * smoothSpeed);
    }

    private void UpdateBars() 
    {
        // Метод-заглушка для расширения логики (например, изменение цвета полосок при лоу-хп)
    }
}