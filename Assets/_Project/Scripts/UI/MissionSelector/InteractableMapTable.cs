using UnityEngine;
using Interaction;
using EventBus;

namespace DacicZero.Interaction {
    [RequireComponent(typeof(Interactable))]
    public class InteractableMapTable : MonoBehaviour {
        
        private Interactable _interactableComponent;

        private void Awake() { _interactableComponent = GetComponent<Interactable>(); }

        private void OnEnable() { 
            if (_interactableComponent != null) 
                _interactableComponent.OnInteract.AddListener(OnTableInteracted); 
        }
        
        private void OnDisable() { 
            if (_interactableComponent != null) 
                _interactableComponent.OnInteract.RemoveListener(OnTableInteracted); 
        }

        private void OnTableInteracted(Transform interactor) {
            EventBus<UI.MissionSelector.MapStateChangedEvent>.Raise(0, new UI.MissionSelector.MapStateChangedEvent(true));
        }
    }
}