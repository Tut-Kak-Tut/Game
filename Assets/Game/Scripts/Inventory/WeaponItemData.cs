using Game.Combat;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Weapon Item", fileName = "WeaponItem")]
public class WeaponItemData : ItemData
{
    [Header("Weapon Reference")]
    public WeaponData weapon;

    private void OnValidate()
    {
        itemType = ItemType.Weapon;

        if (weapon != null)
        {
            if (string.IsNullOrWhiteSpace(itemName))
                itemName = weapon.displayName;
            damageValue = weapon.damage;
        }
    }
}
