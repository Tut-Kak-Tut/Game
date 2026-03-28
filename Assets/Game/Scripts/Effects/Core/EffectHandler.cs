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
        if (existing != null)
        {
            existing.RemainingTime = data.duration;
        }
        else
        {
            ActiveEffect newEffect = new ActiveEffect(data);
            _activeEffects.Add(newEffect);
            
            // Если это бафф/дебафф, применяем изменение статов сразу
            if (data.type != EffectData.EffectType.DamageOverTime)
                ModifyStat(data, data.power);
        }
    }

    void Update()
    {
        for (int i = _activeEffects.Count - 1; i >= 0; i--)
        {
            var effect = _activeEffects[i];
            effect.RemainingTime -= Time.deltaTime;

            // Логика периодического урона (DOT)
            if (effect.Data.type == EffectData.EffectType.DamageOverTime)
            {
                effect.TickTimer -= Time.deltaTime;
                if (effect.TickTimer <= 0)
                {
                    // ВЫЗЫВАЕМ НОВЫЙ МЕТОД:
                    _stats.TakeDamage(effect.Data.power, effect.Data.damageType);
                    
                    effect.TickTimer = effect.Data.tickInterval;
                }
            }

            // Удаление эффекта
            if (effect.RemainingTime <= 0)
            {
                if (effect.Data.type != EffectData.EffectType.DamageOverTime)
                    ModifyStat(effect.Data, -effect.Data.power); // Возвращаем статы назад
                
                _activeEffects.RemoveAt(i);
            }
        }
    }

    private void ModifyStat(EffectData data, float amount)
    {
        switch (data.targetStat)
        {
            case EffectData.StatType.Armor: _stats.armorModifier += amount; break;
            case EffectData.StatType.MagicResistance: _stats.magicResModifier += amount; break;
            case EffectData.StatType.PhysicalDamage: _stats.damageModifier += amount; break;
        }
    }
}