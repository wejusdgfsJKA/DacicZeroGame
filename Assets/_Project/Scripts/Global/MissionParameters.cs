using DacicZero.Data.Weapons;
using UnityEngine;

namespace DacicZero.Global {
    public static class MissionParameters {
        
        public static bool IsReplay { get; private set; }
        
        public static WeaponDataSO ActivePrimary { get; private set; }
        public static WeaponDataSO ActiveSecondary { get; private set; }

        /// <summary> caches the mission parameters right before loading the scene </summary>
        public static void PrepareMissionLaunch(bool isReplay, WeaponDataSO primaryWeapon, WeaponDataSO secondaryWeapon) {
            IsReplay = isReplay;
            
            ActivePrimary = primaryWeapon;
            ActiveSecondary = secondaryWeapon;

            Debug.Log($"[MISSION PARAMETERS] Launching Mission... Replay: {IsReplay} | Primary: {(ActivePrimary != null ? ActivePrimary.WeaponId : "None")} | Secondary: {(ActiveSecondary != null ? ActiveSecondary.WeaponId : "None")}");
        }

        /// <summary> clears the static data, call this when returning to the main menu to prevent data bleed </summary>
        public static void ClearSessionData() {
            IsReplay = false;
            ActivePrimary = null;
            ActiveSecondary = null;
        }
    }
}
