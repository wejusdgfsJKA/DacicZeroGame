using System.Collections.Generic;
using UnityEngine;

namespace DacicZero.UI {
    /// <summary> global manager handling open menus and escape key stack. </summary>
    public class UIManager : MonoBehaviour {
        public static UIManager Instance { get; private set; }

        private readonly Stack<IClosable> _openMenus = new Stack<IClosable>();

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape) && _openMenus.TryPop(out IClosable topMenu))
                topMenu.Close();
        }

        public static void RegisterMenu(IClosable menu) {
            if (Instance == null || menu == null) return;
            if (!Instance._openMenus.Contains(menu))
                Instance._openMenus.Push(menu);
        }

        public static void UnregisterMenu(IClosable menu) {
            if (Instance == null || menu == null) return;

            // Rebuild stack to remove specific menu if it's closed out of order
            if (Instance._openMenus.Contains(menu)) {
                var tempStack = new Stack<IClosable>();
                while (Instance._openMenus.TryPop(out IClosable current))
                    if (current != menu) tempStack.Push(current);

                while (tempStack.TryPop(out IClosable current))
                    Instance._openMenus.Push(current);
            }
        }
    }
}
