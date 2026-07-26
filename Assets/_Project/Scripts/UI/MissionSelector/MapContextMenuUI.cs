using UnityEngine;

namespace DacicZero.UI.MissionSelector {
    /// <summary> context menu overlay for map interactions. </summary>
    public class MapContextMenuUI : MonoBehaviour, IClosable {
        [Header("UI References")]
        [Tooltip("The main visual panel to toggle on/off.")]
        [SerializeField] private GameObject _menuPanel;

        public bool IsOpen => _menuPanel != null && _menuPanel.activeSelf;

        public void Show() {
            if (_menuPanel != null) _menuPanel.SetActive(true);
            UIManager.RegisterMenu(this);
        }

        public void Close() {
            if (_menuPanel != null) _menuPanel.SetActive(false);
            UIManager.UnregisterMenu(this);
        }
    }
}
