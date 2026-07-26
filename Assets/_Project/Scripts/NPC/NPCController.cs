using Interaction;
using UnityEngine;
using EventBus;

namespace DacicZero.NPC {
    #region Events
#warning when upgrading to c# 10, replace this struct with 'uncomment the following line' to reduce boilerplate.
    // public record struct StartDialogEvent(Data.Dialog.DialogSequenceSO Sequence, NPCController NPC) : IEvent;
    public readonly struct StartDialogEvent : IEvent {
        public readonly Data.Dialog.DialogSequenceSO Sequence;
        public readonly NPCController NPC;

        public StartDialogEvent(Data.Dialog.DialogSequenceSO sequence, NPCController npc) {
            Sequence = sequence;
            NPC = npc;
        }
    }
    #endregion

    /// <summary> controls individual npc interaction and dialog triggers. </summary>
    [RequireComponent(typeof(Interactable), typeof(Collider))]
    public class NPCController : MonoBehaviour {
        #region Variables & Properties
        [Header("Components")]
        [SerializeField] private Interactable _interactable;

        [Header("Visual Feedback")]
        [Tooltip("object with outline shader or glow particle system.")]
        [SerializeField] private GameObject _glowObject;

        [Header("Dialog Data")]
        [Tooltip("sequence to play on interact.")]
        [SerializeField] private Data.Dialog.DialogSequenceSO _defaultDialog;

        public Data.Dialog.DialogSequenceSO DefaultDialog => _defaultDialog;
        #endregion

        #region Unity Lifecycle
        private void Awake() {
            EnsureReferences();
            gameObject.layer = LayerMask.NameToLayer("UI");
        }

        private void OnEnable() { if (_interactable != null) _interactable.OnInteract.AddListener(OnPlayerInteracted); }
        private void OnDisable() { if (_interactable != null) _interactable.OnInteract.RemoveListener(OnPlayerInteracted); }
        #endregion

        #region Interaction & Visuals
        private void EnsureReferences() {
            if (_interactable == null) TryGetComponent(out _interactable);
            if (_glowObject != null) _glowObject.SetActive(false);
        }

        private void OnPlayerInteracted(Transform interactor) {
            if (DefaultDialog != null)
                EventBus<StartDialogEvent>.Raise(0, new(DefaultDialog, this));
            else
                Debug.LogWarning($"NPC {gameObject.name} lacks default dialog!");
        }

        private void OnMouseEnter() { if (_glowObject != null) _glowObject.SetActive(true); }
        private void OnMouseExit() { if (_glowObject != null) _glowObject.SetActive(false); }
        #endregion
    }
}
