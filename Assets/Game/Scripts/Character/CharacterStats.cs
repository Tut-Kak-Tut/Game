using UnityEngine;
using System; // Нужно для Action

public class CharacterStats : MonoBehaviour
{
    // СОБЫТИЯ: на них будут подписываться UI и другие системы
    public event Action OnResourceChanged;      // Когда меняется текущее ХП/МП/Стамина
    public event Action OnStatsRecalculated;    // Когда меняются баффы (макс. статы)

    [Header("--- CORE ATTRIBUTES ---")]
    public int strength = 10;
    public int agility = 10;
    public int constitution = 10;
    public int intelligence = 10;
    public int wisdom = 10;
    public int charisma = 10;

    [Header("--- ATTRIBUTE MODIFIERS ---")]
    // С помощью свойств (Properties) мы будем вызывать событие при каждом изменении мода
    private float _strMod;
    public float strMod { get => _strMod; set { _strMod = value; OnStatsRecalculated?.Invoke(); } }

    private float _agiMod;
    public float agiMod { get => _agiMod; set { _agiMod = value; OnStatsRecalculated?.Invoke(); } }

    private float _conMod;
    public float conMod { get => _conMod; set { _conMod = value; OnStatsRecalculated?.Invoke(); } }

    private float _intMod;
    public float intMod { get => _intMod; set { _intMod = value; OnStatsRecalculated?.Invoke(); } }

    private float _wisMod;
    public float wisMod { get => _wisMod; set { _wisMod = value; OnStatsRecalculated?.Invoke(); } }

    private float _chaMod;
    public float chaMod { get => _chaMod; set { _chaMod = value; OnStatsRecalculated?.Invoke(); } }

    [Header("--- DIRECT MODIFIERS ---")]
    private float _critMod;
    public float critMod { get => _critMod; set { _critMod = value; OnStatsRecalculated?.Invoke(); } }

    private float _spellPowerMod;
    public float spellPowerMod { get => _spellPowerMod; set { _spellPowerMod = value; OnStatsRecalculated?.Invoke(); } }

    // Финальные значения атрибутов
    public float FinalStr => strength + strMod;
    public float FinalAgi => agility + agiMod;
    public float FinalCon => constitution + conMod;
    public float FinalInt => intelligence + intMod;
    public float FinalWis => wisdom + wisMod;
    public float FinalCha => charisma + chaMod;

    [Header("--- DERIVED STATS ---")]
    public float maxHealth => (FinalCon * 10f) + 50f; 
    public float maxMana => (FinalInt * 10f) + 20f;
    public float maxStamina => (FinalCon * 5f) + 50f;
    
    public float physicalDamage => (FinalStr * 2f);
    public float physicalArmor => (FinalAgi * 1.5f);
    public float magicResistance => (FinalWis * 1.5f);
    public float walkSpeed => 4f + (FinalAgi * 0.15f);

    [Header("--- ADVANCED STATS ---")]
    public float critChance => 5f + (FinalAgi * 0.5f) + critMod;
    public float critMultiplier => 1.5f + (FinalStr * 0.01f);
    public float spellPower => 10f + (FinalInt * 2f) + spellPowerMod;

    [Header("--- CURRENT RESOURCES ---")]
    public float currentHealth;
    public float currentMana;
    public float currentStamina;

    public float healthRegenRate => FinalCon * 0.2f;
    public float staminaRegenRate => FinalCon * 0.5f;

    private float regenTimer;
    public float regenCooldown = 2.0f;

    void Awake()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentStamina = maxStamina;
    }

    void Update() => HandleRegeneration();

    private void HandleRegeneration()
    {
        if (regenTimer > 0) regenTimer -= Time.deltaTime;
        
        bool isRegen = false;
        if (regenTimer <= 0)
        {
            if (currentHealth < maxHealth)
            {
                currentHealth = Mathf.MoveTowards(currentHealth, maxHealth, healthRegenRate * Time.deltaTime);
                isRegen = true;
            }
            if (currentStamina < maxStamina)
            {
                currentStamina = Mathf.MoveTowards(currentStamina, maxStamina, staminaRegenRate * Time.deltaTime);
                isRegen = true;
            }
        }
        
        if (isRegen) OnResourceChanged?.Invoke();
    }

    public bool UseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            regenTimer = regenCooldown;
            OnResourceChanged?.Invoke(); // Уведомляем об изменении
            return true;
        }
        return false;
    }

    public void TakeDamage(float amount, DamageType type)
{
    float finalDamage = amount;

    // РАСЧЕТ УРОНА СО СТАТАМИ
    if (type == DamageType.Physical)
        finalDamage = Mathf.Max(amount - physicalArmor, 0);
    else if (type == DamageType.Magic)
        finalDamage = Mathf.Max(amount - magicResistance, 0);

    // НАНЕСЕНИЕ УРОНА (МЫ ЭТО УЖЕ ДЕЛАЛИ)
    currentHealth = Mathf.Max(currentHealth - finalDamage, 0);
    regenTimer = regenCooldown; 

    // СОБЫТИЯ UI (МЫ ЭТО УЖЕ ДЕЛАЛИ)
    OnResourceChanged?.Invoke();

    // ========================================================
    // ВОТ ТУТ ДОБАВЛЯЕМ ВСПЛЫВАЮЩИЙ ТЕКСТ!
    // ========================================================
    if (DamageTextManager.Instance != null)
    {
        // Спавним чуть выше центра персонажа (+ 2 метра вверх)
        Vector3 spawnPos = transform.position + Vector3.up * 2f;
        
        Color textColor = Color.white;
        if (type == DamageType.Magic) textColor = Color.cyan;
        if (type == DamageType.True) textColor = Color.yellow;

        DamageTextManager.Instance.SpawnText(spawnPos, Mathf.RoundToInt(finalDamage).ToString(), textColor);
    }
    // ========================================================

    if (currentHealth <= 0) Die();
}

    void Die() => Debug.Log(gameObject.name + " погиб!");
}