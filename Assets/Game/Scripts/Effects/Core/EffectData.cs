using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct StatModifier 
{
    public StatType stat; // Теперь берется из GlobalEnums
    public float value;
}


[CreateAssetMenu(fileName = "NewEffect", menuName = "RPG System/Effect Data")]
public class EffectData : ScriptableObject
{
    [Header("Visuals")]
    public string effectName;
    public Sprite icon;
    public Color effectColor = Color.white;

    [Header("Main Settings")]
    public float duration;       
    public float tickInterval;   
    public bool isStackable;     

    [Header("Multi-Stat Settings")]
    public List<StatModifier> modifiers = new List<StatModifier>();
    
    [Header("Stacking")]
    public int maxStacks = 1; // По умолчанию 1

    [Header("Logic Config")]
    public EffectType type;        
    public float dotDamage;       
    public DamageType damageType;
}