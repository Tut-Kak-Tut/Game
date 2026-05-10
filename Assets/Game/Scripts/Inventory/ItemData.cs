using UnityEngine;

// Типы предметов
public enum ItemType { Generic, Weapon, Armor, Consumable, Quest }

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Общая информация")]
    public string itemName;
    public ItemType itemType;
    public Sprite icon;
    [TextArea] public string description;

    [Header("Настройки стака")]
    public bool isStackable;
    public int maxStackSize = 99;

    [Header("Характеристики (для экипировки/еды)")]
    public float healthEffect;    // Лечение или урон
    public float manaEffect;      // Восстановление маны
    public float staminaEffect;   // Восстановление выносливости
    public float damageValue;     // Урон (если оружие)
    public float defenseValue;    // Защита (если броня)
}