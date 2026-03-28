using UnityEngine;

public enum EffectType { DamageOverTime, BuffStats, DebuffStats }
public enum DamageType { Physical, Magic, True }
public enum ModificationType { Flat, Percent }
// Теперь эффекты влияют на основные атрибуты
public enum StatType { Strength, Agility, Constitution, Intelligence, Wisdom, Charisma }

[CreateAssetMenu(fileName = "NewEffect", menuName = "RPG System/Effect Data")]
public class EffectData : ScriptableObject
{
    [Header("Visuals")]
    public string effectName;
    public Sprite icon;

    [Header("Main Settings")]
    public float duration;       
    public float tickInterval;   
    public float power;          
    public bool isStackable;     

    [Header("Type Config")]
    public EffectType type;        
    public StatType targetStat;    
    public ModificationType modType; 
    public DamageType damageType;
}