using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DacicZero.UI.Dialog {
    /// <summary> visual option button component inside the dialog panel. </summary>
    [RequireComponent(typeof(Button))]
    public class DialogOptionUI : MonoBehaviour {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _optionText;

        public Data.Dialog.DialogOption OptionData { get; private set; }

        private Button _button;
        private DialogUIController _controller;

        private void Awake() => EnsureReferences();

        private void EnsureReferences() {
            if (_button == null) TryGetComponent(out _button);
            if (_optionText == null) _optionText = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void Setup(Data.Dialog.DialogOption data, DialogUIController ctrl) {
            if (data == null) return;

            EnsureReferences();

            OptionData = data;
            _controller = ctrl;

            if (_optionText != null) {
                _optionText.text = data.OptionText;
                if (data.FontSize > 0) _optionText.fontSize = data.FontSize;
            }

            if (_button != null) {
                _button.onClick.RemoveListener(OnClicked);
                _button.onClick.AddListener(OnClicked);
            }
        }

        private void OnClicked() { if (_controller != null) _controller.OnOptionSelected(OptionData); }
    }
}
