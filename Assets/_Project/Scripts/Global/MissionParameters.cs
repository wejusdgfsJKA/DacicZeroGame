using DacicZero.Data.Weapons;

namespace DacicZero.Global {
    /// <summary> static state passing data between scenes. </summary>
    public static class MissionParameters {
        public static bool IsReplay { get; set; }

        /// <summary> snapshots the current loadout at the moment the mission starts. </summary>
        public static WeaponDataSO ActivePrimary { get; private set; }
        public static WeaponDataSO ActiveSecondary { get; private set; }

        public static void PrepareMissionLaunch(bool isReplay) {
            IsReplay = isReplay;
            ActivePrimary = PlayerLoadout.EquippedPrimary;
            ActiveSecondary = PlayerLoadout.EquippedSecondary;

            UnityEngine.Debug.Log($"[MissionParameters] Launching Mission... Replay: {IsReplay} | Primary: {(ActivePrimary != null ? ActivePrimary.WeaponId : "None")} | Secondary: {(ActiveSecondary != null ? ActiveSecondary.WeaponId : "None")}");
        }
    }
}
