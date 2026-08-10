using System;
using System.Collections.Generic;
using DacicZero.Data.Weapons;

namespace DacicZero.Global {
    public static class PlayerLoadout {
        
        /// <summary> activated whenever weapons are added, removed, or equipped </summary>
        public static event Action OnLoadoutChanged;

        private static readonly List<WeaponDataSO> _ownedWeapons = new();
        public static IReadOnlyList<WeaponDataSO> OwnedWeapons => _ownedWeapons;

        public static WeaponDataSO EquippedPrimary { get; private set; }
        public static WeaponDataSO EquippedSecondary { get; private set; }

        public static void Initialize(WeaponDataSO startingPrimary, WeaponDataSO startingSecondary) {
            _ownedWeapons.Clear();
            EquippedPrimary = null;
            EquippedSecondary = null;

            if (startingPrimary != null) {
                _ownedWeapons.Add(startingPrimary);
                EquippedPrimary = startingPrimary;
            }
            
            if (startingSecondary != null) {
                if (!_ownedWeapons.Contains(startingSecondary)) _ownedWeapons.Add(startingSecondary);
                EquippedSecondary = startingSecondary;
            }

            OnLoadoutChanged?.Invoke();
        }

        public static void AddWeapon(WeaponDataSO weapon) {
            if (weapon == null || _ownedWeapons.Contains(weapon)) return;
            
            _ownedWeapons.Add(weapon);
            OnLoadoutChanged?.Invoke();
        }

        public static void UpgradeWeapon(WeaponDataSO oldWeapon, WeaponDataSO newWeapon) {
            if (oldWeapon == null || newWeapon == null) return;

            if (_ownedWeapons.Contains(oldWeapon)) _ownedWeapons.Remove(oldWeapon);
            if (!_ownedWeapons.Contains(newWeapon)) _ownedWeapons.Add(newWeapon);

            if (EquippedPrimary == oldWeapon) EquippedPrimary = newWeapon;
            if (EquippedSecondary == oldWeapon) EquippedSecondary = newWeapon;

            OnLoadoutChanged?.Invoke();
        }

        public static void EquipWeapon(WeaponDataSO weapon) {
            if (weapon == null) return;
            bool changedState = false;

            if (!_ownedWeapons.Contains(weapon)) {
                _ownedWeapons.Add(weapon);
                changedState = true;
            }

            if (weapon.Type == WeaponType.Primary && EquippedPrimary != weapon) {
                EquippedPrimary = weapon;
                changedState = true;
            }
            else if (weapon.Type == WeaponType.Secondary && EquippedSecondary != weapon) {
                EquippedSecondary = weapon;
                changedState = true;
            }

            if (changedState) OnLoadoutChanged?.Invoke();
        }
    }
}