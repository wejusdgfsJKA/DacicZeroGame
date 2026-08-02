using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DacicZero.UI.Dialog {
    [RequireComponent(typeof(Button))]
    public class DialogOptionUI : MonoBehaviour {
        
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _optionText;

        public Data.Dialog.DialogOption OptionData { get; private set; }

        private Button _button;
        private DialogUIController _controller;

        private void Awake() {
            _button = GetComponent<Button>();
            
            if (_optionText == null) _optionText = GetComponentInChildren<TextMeshProUGUI>();
            
            _button.onClick.AddListener(OnClicked);
        }

        private void OnDestroy() { if (_button != null) _button.onClick.RemoveListener(OnClicked); }
        private void OnClicked() { if (_controller != null && OptionData != null) _controller.OnOptionSelected(OptionData); }

        public void Setup(Data.Dialog.DialogOption data, DialogUIController ctrl) {
            if (data == null) return;

            OptionData = data;
            _controller = ctrl;

            if (_optionText != null) _optionText.text = data.OptionText;
        }

    }
}