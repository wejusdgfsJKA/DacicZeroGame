using UnityEngine;
using Interaction;
using DacicZero.UI.MissionSelector;

namespace DacicZero.Interaction {
    /// <summary> environment object that opens the mission map. </summary>
    [RequireComponent(typeof(Interactable))]
    public class InteractableMapTable : MonoBehaviour {
        [Header("References")]
        [Tooltip("The Mission Map UI to open upon interaction.")]
        [SerializeField] private MissionSelectorUI _missionMapUI;

        private Interactable _interactableComponent;

        private void Awake() {
            if (_interactableComponent == null) TryGetComponent(out _interactableComponent);

#if UNITY_EDITOR
            if (_missionMapUI == null)
                Debug.LogError($"[InteractableMapTable] Map UI missing on {gameObject.name}! Please assign it.");
#endif
        }

        private void OnEnable() { if (_interactableComponent != null) _interactableComponent.OnInteract.AddListener(OnTableInteracted); }
        private void OnDisable() { if (_interactableComponent != null) _interactableComponent.OnInteract.RemoveListener(OnTableInteracted); }

        private void OnTableInteracted(Transform interactor) { if (_missionMapUI != null) _missionMapUI.OpenMap(); }
    }
}
