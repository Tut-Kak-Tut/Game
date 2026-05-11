using UnityEngine;
using System.Collections; // ОБЯЗАТЕЛЬНО для IEnumerator
using System.Collections.Generic;

public class EffectHandler : MonoBehaviour
{
    private CharacterStats _stats;
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    [SerializeField] private DamageTextManager damageTextManager;

    [SerializeField] private List<ActiveEffect> _activeEffects = new List<ActiveEffect>();
    public List<ActiveEffect> ActiveEffects => _activeEffects;

    void Awake() 
    {
        _stats = GetComponent<CharacterStats>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null) _originalColor = _spriteRenderer.color;
        if (damageTextManager == null)
            damageTextManager = DamageTextManager.Instance;
    }

    public void SetDamageTextManager(DamageTextManager manager)
    {
        damageTextManager = manager;
    }

    public void TriggerHitFlash()
    {
        if (!gameObject.activeInHierarchy) return;
        StartCoroutine(FlashColor(Color.white, 0.12f));
    }

    public void ApplyEffect(EffectData data)
    {
        var existingEffects = _activeEffects.FindAll(e => e.Data == data);
        
        // --- ЛОГИКА REFRESH (Обновление времени) ---
        if (existingEffects.Count > 0 && !data.isStackable)
        {
            existingEffects[0].RemainingTime = data.duration;
            return; 
        }

        if (data.isStackable && existingEffects.Count >= data.maxStacks && data.maxStacks > 0)
        {
            existingEffects[0].RemainingTime = data.duration;
            return;
        }

        ActiveEffect newEffect = new ActiveEffect(data);
        _activeEffects.Add(newEffect);
        
        if (data.type != EffectType.DamageOverTime)
        {
            ModifyStat(data, 1f); 
        }
    }

    void Update()
    {
        for (int i = _activeEffects.Count - 1; i >= 0; i--)
        {
            var effect = _activeEffects[i];
            effect.RemainingTime -= Time.deltaTime;

            if (effect.Data.type == EffectType.DamageOverTime)
            {
                effect.TickTimer -= Time.deltaTime;
                if (effect.TickTimer <= 0)
                {
                    float finalDamage = effect.Data.dotDamage + (_stats.spellPower * 0.1f);
                    _stats.TakeDamage(finalDamage, effect.Data.damageType);
                    
                    // --- ВИЗУАЛЬНЫЙ ТИК (Вспышка) ---
                    StartCoroutine(FlashColor(Color.red, 0.1f)); 

                    effect.TickTimer = effect.Data.tickInterval;
                }
            }
            if (effect.Data.type == EffectType.HealingOverTime)
            {
                effect.TickTimer -= Time.deltaTime;
                if (effect.TickTimer <= 0)
                {
                    // Лечим: берем значение dotDamage как силу лечения
                    _stats.currentHealth = Mathf.Min(_stats.currentHealth + effect.Data.dotDamage, _stats.maxHealth);
                    
                    // Спавним ЗЕЛЕНЫЙ текст через твой DamageTextManager
                    if (damageTextManager != null)
                        damageTextManager.SpawnText(transform.position, effect.Data.dotDamage.ToString(), Color.green);
                    
                    // Вспышка зеленым цветом (визуальный тик)
                    StartCoroutine(FlashColor(Color.green, 0.1f)); 

                    effect.TickTimer = effect.Data.tickInterval;
                }
            }
            if (effect.RemainingTime <= 0) RemoveEffectAt(i);
        }
        UpdateVisuals();
    }

    // ИСПРАВЛЕННАЯ КОРУТИНА (без ошибки CS0305)
    private IEnumerator FlashColor(Color flashColor, float time)
    {
        if (_spriteRenderer == null) yield break;
        _spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(time);
        UpdateVisuals(); 
    }

    private void UpdateVisuals()
    {
        if (_spriteRenderer == null) return;
        _spriteRenderer.color = (_activeEffects.Count > 0) ? _activeEffects[_activeEffects.Count - 1].Data.effectColor : _originalColor;
    }

    private void ModifyStat(EffectData data, float multiplier)
    {
        foreach (var mod in data.modifiers)
        {
            float finalValue = mod.value * multiplier;

            // Если тип модификатора - процентный
            if (mod.modType == ModificationType.Percent)
            {
                // Пример: Сила 10 * (5% * 0.01) = 0.5 к модификатору
                // Здесь логика зависит от того, как у тебя устроены статы
                // Допустим, мы просто передаем процент от базового стата:
                // finalValue = (_stats.GetBaseStat(mod.stat) * (mod.value / 100f)) * multiplier;
            }

            ApplyToStat(mod.stat, finalValue);
        }
    }

    private void ApplyToStat(StatType stat, float value)
    {
        switch (stat)
        {
            case StatType.Strength: _stats.strMod += value; break;
            case StatType.Agility: _stats.agiMod += value; break;
            case StatType.CritChance: _stats.critMod += value; break;
            // Добавь остальные статы по аналогии...
        }
    }

    private void RemoveEffectAt(int index)
    {
        var effect = _activeEffects[index];
        if (effect.Data.type != EffectType.DamageOverTime) ModifyStat(effect.Data, -1f);
        _activeEffects.RemoveAt(index);
    }
}