using System;
using Game.Combat;
using Game.CoreRuntime;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [SerializeField] private WeaponData defaultWeapon;
    public WeaponData CurrentWeapon { get; private set; }
    public event Action<WeaponData> OnWeaponChanged;

    private void Awake()
    {
        if (defaultWeapon != null) Equip(defaultWeapon);
    }

    public void Equip(WeaponData weapon)
    {
        if (weapon == null) return;
        CurrentWeapon = weapon;
        OnWeaponChanged?.Invoke(weapon);
        GameSession.Instance?.EventBus.Publish(
            new PlayerWeaponEquippedEvent(weapon.weaponId, weapon.displayName, weapon.damageType));
    }
}
