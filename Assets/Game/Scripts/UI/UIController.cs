using UnityEngine;
using UnityEngine.UI; // Обязательно для работы с Image

public class UIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterStats playerStats;
    
    [Header("Health Bar")]
    [SerializeField] private Image healthFill;
    
    [Header("Stamina Bar")]
    [SerializeField] private Image staminaFill;

    void Update()
    {
        if (playerStats == null) return;

        // Обновляем полоски каждый кадр
        UpdateHealth();
        UpdateStamina();
    }

    private void UpdateHealth()
    {
        // Рассчитываем процент (от 0 до 1)
        float targetFill = playerStats.currentHealth / playerStats.maxHealth;
        
        // Плавное изменение (Lerp) - полоска не будет дергаться
        healthFill.fillAmount = Mathf.Lerp(healthFill.fillAmount, targetFill, Time.deltaTime * 5f);
    }

    private void UpdateStamina()
    {
        float targetFill = playerStats.currentStamina / playerStats.maxStamina;
        
        // Плавное изменение для стамины
        staminaFill.fillAmount = Mathf.Lerp(staminaFill.fillAmount, targetFill, Time.deltaTime * 10f);
    }
}