using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Regen Cooldown (Seconds)")]
    public float regenCooldown = 2.0f; // Через сколько секунд начнется регенерация

    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float healthRegenRate = 1f;
    private float healthRegenTimer;

    [Header("Mana")]
    public float maxMana = 50f;
    public float currentMana;
    public float manaRegenRate = 2f;
    private float manaRegenTimer;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 5f;
    private float staminaRegenTimer;

    [Header("Protection and Damage")]
    public float physicalArmor = 10f;
    public float magicResistance = 5f;
    public float physicalDamage = 15f;

    void Awake()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentStamina = maxStamina;
    }

    void Update()
    {
        HandleRegeneration();
    }

    private void HandleRegeneration()
    {
        // Уменьшаем таймеры каждый кадр
        if (healthRegenTimer > 0) healthRegenTimer -= Time.deltaTime;
        if (manaRegenTimer > 0) manaRegenTimer -= Time.deltaTime;
        if (staminaRegenTimer > 0) staminaRegenTimer -= Time.deltaTime;

        // Регенерация здоровья (только если таймер вышел)
        if (healthRegenTimer <= 0 && currentHealth < maxHealth)
            currentHealth = Mathf.MoveTowards(currentHealth, maxHealth, healthRegenRate * Time.deltaTime);

        // Регенерация маны
        if (manaRegenTimer <= 0 && currentMana < maxMana)
            currentMana = Mathf.MoveTowards(currentMana, maxMana, manaRegenRate * Time.deltaTime);

        // Регенерация стамины
        if (staminaRegenTimer <= 0 && currentStamina < maxStamina)
            currentStamina = Mathf.MoveTowards(currentStamina, maxStamina, staminaRegenRate * Time.deltaTime);
    }

    // --- МЕТОДЫ ПОЛУЧЕНИЯ УРОНА ---

    public void TakePhysicalDamage(float damage)
    {
        float finalDamage = Mathf.Max(damage - physicalArmor, 0);
        ApplyDamage(finalDamage);
    }

    public void TakeMagicDamage(float damage)
    {
        float finalDamage = Mathf.Max(damage - magicResistance, 0);
        ApplyDamage(finalDamage);
    }

    private void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        healthRegenTimer = regenCooldown; // Сбрасываем КД регенерации здоровья при уроне
        
        if (currentHealth <= 0) Die();
    }

    // --- МЕТОДЫ ТРАТЫ РЕСУРСОВ ---

    public bool UseMana(float amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            manaRegenTimer = regenCooldown; // Сбрасываем КД регенерации маны
            return true;
        }
        return false;
    }

    public bool UseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            staminaRegenTimer = regenCooldown; // Сбрасываем КД регенерации стамины
            return true;
        }
        return false;
    }

    public bool setHealth(float amount)
    {
        if (amount < 0 || amount > maxHealth) return false;
        currentHealth = amount;
        return true;
    }
    public bool setMana(float amount)
    {
        if (amount < 0 || amount > maxMana) return false;
        currentMana = amount;
        return true;
    }
    public bool setStamina(float amount)
    {
        if (amount < 0 || amount > maxStamina) return false;
        currentStamina = amount;
        return true;
    }
    
    void Die() => Debug.Log("Смерть!");
}