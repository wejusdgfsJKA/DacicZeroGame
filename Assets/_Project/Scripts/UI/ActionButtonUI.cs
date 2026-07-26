using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using EventBus;

namespace DacicZero.UI {
    /// <summary> generic button triggering both unity events and eventbus actions </summary>
    [RequireComponent(typeof(Button))]
    public class ActionButtonUI : MonoBehaviour {
        [Header("UI References")]
        /// <summary> label displayed on the button </summary>
        [SerializeField] private TextMeshProUGUI _buttonText;

        [Header("Event Settings")]
        [Tooltip("Action ID broadcasted via EventBus (e.g. 'OpenMap'). Leave empty to only use UnityEvent.")]
        [SerializeField] private string _actionId;
        
        /// <summary> optional payload for the eventbus action </summary>
        [SerializeField] private string _actionParams;

        [Header("Inspector Callbacks")]
        /// <summary> local event fired on click </summary>
        public UnityEvent OnAction;

        private Button _button;

        private void Awake() {
            _button = GetComponent<Button>();
            // fallback: find text component if unassigned
            if (_buttonText == null) _buttonText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void OnEnable() { if (_button != null) _button.onClick.AddListener(OnClicked); }
        private void OnDisable() { if (_button != null) _button.onClick.RemoveListener(OnClicked); }

        /// <summary> configures the button dynamically from code </summary>
        public void Setup(string label, string actionId, string actionParams = "") {
            if (_buttonText != null) _buttonText.text = label;
            _actionId = actionId;
            _actionParams = actionParams;
        }

        private void OnClicked() {
            // fire inspector events
            OnAction?.Invoke();

            // broadcast global action if defined
            if (!string.IsNullOrEmpty(_actionId))
                EventBus<Dialog.DialogActionFiredEvent>.Raise(0, new Dialog.DialogActionFiredEvent(_actionId, _actionParams));
        }
    }
}
