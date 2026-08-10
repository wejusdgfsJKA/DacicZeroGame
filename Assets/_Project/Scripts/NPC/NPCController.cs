using Interaction;
using UnityEngine;
using EventBus;

namespace DacicZero.NPC {
#warning when upgrading to c# 10, uncomment the record struct below and remove the standard struct to reduce boilerplate.
    // public readonly record struct StartDialogEvent(Data.Dialog.DialogSequenceSO Sequence, NPCController NPC) : IEvent;
    
    public readonly struct StartDialogEvent : IEvent {
        public readonly Data.Dialog.DialogSequenceSO Sequence;
        public readonly NPCController NPC;

        public StartDialogEvent(Data.Dialog.DialogSequenceSO sequence, NPCController npc) {
            Sequence = sequence;
            NPC = npc;
        }
    }

    [RequireComponent(typeof(Interactable), typeof(Collider))]
    public class NPCController : MonoBehaviour {
        [Header("Components")]
        [SerializeField] private Interactable _interactable;

        [Header("Visual Feedback")]
        [Tooltip("object with outline shader or glow particle system.")]
        [SerializeField] private GameObject _glowObject;

        [Header("Dialog Data")]
        [Tooltip("sequence to play on interact.")]
        [SerializeField] private Data.Dialog.DialogSequenceSO _defaultDialog;

        [Header("Settings")]
        [Tooltip("The layer assigned to this NPC for raycasting. Do NOT use the 'UI' layer!")]
        [SerializeField] private string _interactableLayer = "Default";

        public Data.Dialog.DialogSequenceSO DefaultDialog => _defaultDialog;
        private void Awake() {
            EnsureReferences();
            SetInteractionLayer();
        }

        private void OnEnable() { if (_interactable != null) _interactable.OnInteract.AddListener(OnPlayerInteracted); }
        private void OnDisable() {  if (_interactable != null) _interactable.OnInteract.RemoveListener(OnPlayerInteracted);  }
        
        private void EnsureReferences() {
            if (_interactable == null) TryGetComponent(out _interactable);
            if (_glowObject != null) _glowObject.SetActive(false);
        }

        private void SetInteractionLayer() {
            int targetLayer = LayerMask.NameToLayer(_interactableLayer);
            
            if (targetLayer == -1) {
                Debug.LogWarning($"[NPCController] Layer '{_interactableLayer}' does not exist! Defaulting to current layer.");
                return;
            }
            
            gameObject.layer = targetLayer;
        }

        private void OnPlayerInteracted(Transform interactor) {
            if (DefaultDialog != null) 
                EventBus<StartDialogEvent>.Raise(0, new(DefaultDialog, this));
            else 
                Debug.LogWarning($"NPC {gameObject.name} lacks default dialog!");
        }

        private void OnMouseEnter() {  if (_glowObject != null) _glowObject.SetActive(true);  }
        private void OnMouseExit() {  if (_glowObject != null) _glowObject.SetActive(false);  }
    }
}