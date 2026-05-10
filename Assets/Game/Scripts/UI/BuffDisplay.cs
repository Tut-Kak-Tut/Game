using UnityEngine;
using System.Collections.Generic;

public class BuffDisplay : MonoBehaviour
{
    public GameObject iconPrefab;      // Префаб BuffIcon
    public Transform container;        // Ссылка на BuffPanel (Horizontal Layout Group)
    
    private EffectHandler _handler;
    private Dictionary<ActiveEffect, BuffIconUI> _spawnedIcons = new Dictionary<ActiveEffect, BuffIconUI>();

    void Awake()
    {
        _handler = GetComponent<EffectHandler>();
    }

    void Update()
    {
        UpdateBuffIcons();
    }

    private void UpdateBuffIcons()
    {
        // 1. Создаем иконки для тех эффектов, которых еще нет на панели
        foreach (var effect in _handler.ActiveEffects) 
        {
            if (!_spawnedIcons.ContainsKey(effect))
            {
                GameObject newIcon = Instantiate(iconPrefab, container);
                BuffIconUI ui = newIcon.GetComponent<BuffIconUI>();
                _spawnedIcons.Add(effect, ui);
            }
        }

        // 2. Обновляем и чистим
        List<ActiveEffect> toRemove = new List<ActiveEffect>();

        foreach (var pair in _spawnedIcons)
        {
            // Проверяем, живой ли еще эффект в Handler
            if (!_handler.ActiveEffects.Contains(pair.Key))
            {
                toRemove.Add(pair.Key);
            }
            else
            {
                // Обновляем картинку и таймер
                pair.Value.UpdateIcon(pair.Key.Data.icon, pair.Key.RemainingTime);
            }
        }

        // Удаляем лишние иконки
        foreach (var effect in toRemove)
        {
            Destroy(_spawnedIcons[effect].gameObject);
            _spawnedIcons.Remove(effect);
        }
    }
}