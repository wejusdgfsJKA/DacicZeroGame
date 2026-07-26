using System;
using System.Collections.Generic;
using DacicZero.Data.Weapons;

namespace DacicZero.Global {
    /// <summary> static manager tracking the player's weapon inventory. </summary>
    public static class PlayerLoadout {
        public static event Action OnLoadoutChanged;

        public static List<WeaponDataSO> OwnedWeapons { get; private set; } = new();

        public static WeaponDataSO EquippedPrimary { get; private set; }
        public static WeaponDataSO EquippedSecondary { get; private set; }

        public static void Initialize(WeaponDataSO startingPrimary, WeaponDataSO startingSecondary) {
            OwnedWeapons.Clear();
            if (startingPrimary != null) AddWeapon(startingPrimary);
            if (startingSecondary != null) AddWeapon(startingSecondary);
            EquipWeapon(startingPrimary);
            EquipWeapon(startingSecondary);
        }

        public static void AddWeapon(WeaponDataSO weapon) {
            if (weapon == null || OwnedWeapons.Contains(weapon)) return;
            OwnedWeapons.Add(weapon);
            OnLoadoutChanged?.Invoke();
        }

        public static void UpgradeWeapon(WeaponDataSO oldWeapon, WeaponDataSO newWeapon) {
            if (OwnedWeapons.Contains(oldWeapon)) {
                OwnedWeapons.Remove(oldWeapon);
                OwnedWeapons.Add(newWeapon);

                if (EquippedPrimary == oldWeapon) EquippedPrimary = newWeapon;
                if (EquippedSecondary == oldWeapon) EquippedSecondary = newWeapon;

                OnLoadoutChanged?.Invoke();
            }
        }

        public static void EquipWeapon(WeaponDataSO weapon) {
            if (weapon == null) return;
            if (!OwnedWeapons.Contains(weapon)) AddWeapon(weapon);

            if (weapon.Type == WeaponType.Primary)
                EquippedPrimary = weapon;
            else if (weapon.Type == WeaponType.Secondary)
                EquippedSecondary = weapon;

            OnLoadoutChanged?.Invoke();
        }
    }
}
