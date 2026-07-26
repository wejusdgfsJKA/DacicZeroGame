using UnityEngine;
using UnityEngine.UI;

namespace DacicZero.UI {
    /// <summary> generic button to automate closing the nearest parent panel. </summary>
    [RequireComponent(typeof(Button))]
    public class UniversalCloseButtonUI : MonoBehaviour {
        private Button _button;
        private IClosable _parentMenu;

        private void Awake() {
            _button = GetComponent<Button>();
            _parentMenu = GetComponentInParent<IClosable>();

            if (_parentMenu == null) {
                Debug.LogWarning($"[UniversalCloseButtonUI] No IClosable parent found for {gameObject.name}!");
            }
        }

        private void OnEnable() {
            if (_button != null) _button.onClick.AddListener(OnClicked);
        }

        private void OnDisable() {
            if (_button != null) _button.onClick.RemoveListener(OnClicked);
        }

        private void OnClicked() {
            _parentMenu?.Close();
        }
    }
}
