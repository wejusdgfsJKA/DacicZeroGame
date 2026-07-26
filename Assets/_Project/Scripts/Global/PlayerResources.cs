using System;
using UnityEngine;

namespace DacicZero.Global {
    /// <summary> static manager for global player resources like scrap. </summary>
    public static class PlayerResources {
        public static event Action<int> OnScrapChanged;

        public static int Scrap { get; private set; } = 1000;

        public static void AddScrap(int amount) {
            if (amount > 0) SetScrap(Scrap + amount);
        }

        public static bool SpendScrap(int amount) {
            if (amount <= 0 || Scrap < amount) return false;

            SetScrap(Scrap - amount);
            return true;
        }

        public static void SetScrap(int amount) {
            amount = Mathf.Max(0, amount);
            if (Scrap == amount) return;

            Scrap = amount;
            OnScrapChanged?.Invoke(Scrap);
        }
    }
}
