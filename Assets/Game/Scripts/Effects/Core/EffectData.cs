using UnityEngine;

// Перечисления вынесены за пределы класса, чтобы их видели все скрипты в проекте
public enum EffectType { DamageOverTime, BuffStats, DebuffStats }
public enum StatType { None, Armor, MagicResistance, PhysicalDamage }
public enum DamageType { Physical, Magic, True }

[CreateAssetMenu(fileName = "NewEffect", menuName = "RPG System/Effect Data")]
public class EffectData : ScriptableObject
{
    [Header("Visuals")]
    public string effectName;
    public Sprite icon;

    [Header("Settings")]
    public float duration;       
    public float tickInterval;   
    public float power;          
    
    [Header("Type Config")]
    public EffectType type;        // Тип эффекта (DOT, Buff, Debuff)
    public StatType targetStat;    // Что меняем (для баффов)
    public DamageType damageType;  // Тип урона (для DOT)


    public enum EffectType { DamageOverTime, BuffStats, DebuffStats }
    public enum StatType { None, Armor, MagicResistance, PhysicalDamage }
}