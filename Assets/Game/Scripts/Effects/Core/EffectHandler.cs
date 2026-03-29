using System.Collections.Generic;
using UnityEngine;

public class EffectHandler : MonoBehaviour
{
    private CharacterStats _stats;
    private SpriteRenderer _spriteRenderer; // Ссылка на спрайт
    private Color _originalColor; // Исходный цвет персонажа

    [Header("Debug View")]
    [SerializeField] private List<ActiveEffect> _activeEffects = new List<ActiveEffect>();
    public List<ActiveEffect> ActiveEffects => _activeEffects;

    void Awake() 
    {
        _stats = GetComponent<CharacterStats>();
        _spriteRenderer = GetComponent<SpriteRenderer>(); // Находим рендерер
        if (_spriteRenderer != null) _originalColor = _spriteRenderer.color;
    }

    public void ApplyEffect(EffectData data)
    {
        // 1. Ищем уже существующие копии этого эффекта
        var existingEffects = _activeEffects.FindAll(e => e.Data == data);
        
        // 2. Если эффект НЕ стакается и уже висит — просто обновляем ему время
        if (existingEffects.Count > 0 && !data.isStackable)
        {
            existingEffects[0].RemainingTime = data.duration;
            return;
        }

        // 3. Если эффект стакается, проверяем лимит (maxStacks)
        // (Для этого в EffectData должно быть поле maxStacks)
        if (data.isStackable && existingEffects.Count >= data.maxStacks && data.maxStacks > 0)
        {
            // Обновляем время самому "старому" эффекту в пачке
            existingEffects[0].RemainingTime = data.duration;
            return;
        }

        // 4. Создаем и добавляем новый экземпляр эффекта
        ActiveEffect newEffect = new ActiveEffect(data);
        _activeEffects.Add(newEffect);
        
        // Если это бафф/дебафф (не DOT) — применяем модификаторы сразу
        if (data.type != EffectType.DamageOverTime)
        {
            ModifyStat(data, 1f); 
        }

        Debug.Log($"Применен эффект: {data.effectName}. Всего стаков: {_activeEffects.FindAll(e => e.Data == data).Count}");
    }

    void Update()
    {
        for (int i = _activeEffects.Count - 1; i >= 0; i--)
        {
            var effect = _activeEffects[i];
            effect.RemainingTime -= Time.deltaTime;

            // Логика DOT
            if (effect.Data.type == EffectType.DamageOverTime)
            {
                effect.TickTimer -= Time.deltaTime;
                if (effect.TickTimer <= 0)
                {
                    float finalDamage = effect.Data.dotDamage + (_stats.spellPower * 0.1f);
                    _stats.TakeDamage(finalDamage, effect.Data.damageType);
                    effect.TickTimer = effect.Data.tickInterval;
                }
            }

            if (effect.RemainingTime <= 0) RemoveEffectAt(i);
        }

        UpdateVisuals(); // Обновляем цвет каждый кадр
    }
    private void UpdateVisuals()
    {
        if (_spriteRenderer == null) return;

        if (_activeEffects.Count > 0)
        {
            // Берем цвет последнего примененного эффекта
            _spriteRenderer.color = _activeEffects[_activeEffects.Count - 1].Data.effectColor;
        }
        else
        {
            // Возвращаем исходный цвет, если эффектов нет
            _spriteRenderer.color = _originalColor;
        }
    }

    private void RemoveEffectAt(int index)
    {
        var effect = _activeEffects[index];

        // Если это был бафф, нужно откатать изменения статов назад
        if (effect.Data.type != EffectType.DamageOverTime)
        {
            ModifyStat(effect.Data, -1f);
        }

        _activeEffects.RemoveAt(index);
        Debug.Log($"Эффект {effect.Data.effectName} закончился.");
    }

    private void ModifyStat(EffectData data, float multiplier)
    {
        foreach (var mod in data.modifiers)
        {
            float finalValue = mod.value * multiplier;

            switch (mod.stat)
            {
                case StatType.Strength: _stats.strMod += finalValue; break;
                case StatType.Agility: _stats.agiMod += finalValue; break;
                case StatType.Constitution: _stats.conMod += finalValue; break;
                case StatType.Intelligence: _stats.intMod += finalValue; break;
                case StatType.Wisdom: _stats.wisMod += finalValue; break;
                case StatType.Charisma: _stats.chaMod += finalValue; break;
                case StatType.CritChance: _stats.critMod += finalValue; break;
                case StatType.SpellPower: _stats.spellPowerMod += finalValue; break;
            }
        }
    }
}