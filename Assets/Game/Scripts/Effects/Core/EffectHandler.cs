using System.Collections.Generic;
using UnityEngine;

public class EffectHandler : MonoBehaviour
{
    private CharacterStats _stats;
    public List<ActiveEffect> _activeEffects = new List<ActiveEffect>();

    void Awake() => _stats = GetComponent<CharacterStats>();

    public void ApplyEffect(EffectData data)
    {
        var existing = _activeEffects.Find(e => e.Data == data);
        
        // Если эффект не стакается и уже есть — обновляем время
        if (existing != null && !data.isStackable)
        {
            existing.RemainingTime = data.duration;
            return;
        }

        // В остальных случаях (стакается или новый) — добавляем
        ActiveEffect newEffect = new ActiveEffect(data);
        _activeEffects.Add(newEffect);
        
        if (data.type != EffectType.DamageOverTime)
            ModifyStat(data, data.power);
    }

    void Update()
    {
        for (int i = _activeEffects.Count - 1; i >= 0; i--)
        {
            var effect = _activeEffects[i];
            effect.RemainingTime -= Time.deltaTime;

            // Логика периодического урона (DOT)
            if (effect.Data.type == EffectType.DamageOverTime) // Убрали EffectData.
            {
                effect.TickTimer -= Time.deltaTime;
                if (effect.TickTimer <= 0)
                {
                    _stats.TakeDamage(effect.Data.power, effect.Data.damageType);
                    effect.TickTimer = effect.Data.tickInterval;
                }
            }

            // Удаление эффекта
            if (effect.RemainingTime <= 0)
            {
                if (effect.Data.type != EffectType.DamageOverTime) // Убрали EffectData.
                    ModifyStat(effect.Data, -effect.Data.power);
                
                _activeEffects.RemoveAt(i);
            }
        }
    }

    private void ModifyStat(EffectData data, float amount)
    {
        // В этой системе мы используем Flat изменения для атрибутов (напр. +5 к Силе)
        // Но если ты выбрал Percent в EffectData, можно умножать базу.
        float valueToApply = amount;
        if (data.modType == ModificationType.Percent)
        {
            // Пример: +10% к Силе от базового значения
            // Реализуем по необходимости
        }

        switch (data.targetStat)
        {
            case StatType.Strength: _stats.strMod += valueToApply; break;
            case StatType.Agility: _stats.agiMod += valueToApply; break;
            case StatType.Constitution: _stats.conMod += valueToApply; break;
            case StatType.Intelligence: _stats.intMod += valueToApply; break;
            case StatType.Wisdom: _stats.wisMod += valueToApply; break;
            case StatType.Charisma: _stats.chaMod += valueToApply; break;
        }
    }
}