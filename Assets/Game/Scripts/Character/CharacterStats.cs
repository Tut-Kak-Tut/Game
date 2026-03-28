using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("--- CORE ATTRIBUTES ---")]
    public int strength = 10;
    public int agility = 10;
    public int constitution = 10;
    public int intelligence = 10;
    public int wisdom = 10;
    public int charisma = 10;

    [Header("--- ATTRIBUTE MODIFIERS ---")]
    [field: SerializeField] public float strMod { get; set; }
    [field: SerializeField] public float agiMod { get; set; }
    [field: SerializeField] public float conMod { get; set; }
    [field: SerializeField] public float intMod { get; set; }
    [field: SerializeField] public float wisMod { get; set; }
    [field: SerializeField] public float chaMod { get; set; }

    public float FinalStr => strength + strMod;
    public float FinalAgi => agility + agiMod;
    public float FinalCon => constitution + conMod;
    public float FinalInt => intelligence + intMod;
    public float FinalWis => wisdom + wisMod;
    public float FinalCha => charisma + chaMod;

    [Header("--- DERIVED STATS (FORMULAS) ---")]
    public float maxHealth => (FinalCon * 10f) + 50f; 
    public float maxMana => (FinalInt * 10f) + 20f;
    public float maxStamina => (FinalCon * 5f) + 50f; // Стамина зависит от Телосложения
    
    public float physicalDamage => (FinalStr * 2f);
    public float physicalArmor => (FinalAgi * 1.5f);
    public float magicResistance => (FinalWis * 1.5f);

    // СКОРОСТЬ: Базовая 4 + 0.2 за каждое очко Ловкости
    public float walkSpeed => 4f + (FinalAgi * 0.15f);

    [Header("--- CURRENT RESOURCES ---")]
    public float currentHealth;
    public float currentMana;
    public float currentStamina;

    [Header("--- REGEN RATES ---")]
    public float healthRegenRate => FinalCon * 0.2f;
    public float staminaRegenRate => FinalCon * 0.5f; // Реген стамины от Телосложения

    private float regenTimer;
    public float regenCooldown = 2.0f;

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
        if (regenTimer > 0) regenTimer -= Time.deltaTime;

        if (regenTimer <= 0)
        {
            if (currentHealth < maxHealth)
                currentHealth = Mathf.MoveTowards(currentHealth, maxHealth, healthRegenRate * Time.deltaTime);
            
            if (currentStamina < maxStamina)
                currentStamina = Mathf.MoveTowards(currentStamina, maxStamina, staminaRegenRate * Time.deltaTime);
        }
    }

    // МЕТОД ДЛЯ ТРАТЫ СТАМИНЫ (вызывается из PlayerController)
    public bool UseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            regenTimer = regenCooldown; // Сбрасываем таймер регена при трате
            return true;
        }
        return false;
    }

    public void TakeDamage(float amount, DamageType type)
    {
        float finalDamage = amount;
        if (type == DamageType.Physical) finalDamage = Mathf.Max(amount - physicalArmor, 0);
        else if (type == DamageType.Magic) finalDamage = Mathf.Max(amount - magicResistance, 0);

        currentHealth = Mathf.Max(currentHealth - finalDamage, 0);
        regenTimer = regenCooldown; 
        if (currentHealth <= 0) Die();
    }

    void Die() => Debug.Log(gameObject.name + " погиб!");
}