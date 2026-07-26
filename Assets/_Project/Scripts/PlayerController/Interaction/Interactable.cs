using EventBus;
using UnityEngine;
using UnityEngine.Events;
namespace Interaction {
    public class Interactable : MonoBehaviour {
        /// <summary>
        /// This fires when this object is interacted with.
        /// </summary>
        public UnityEvent<Transform> OnInteract;
        protected void Awake() {
            if (!EventBus<InteractionEvent>.AddActions(transform.GetInstanceID(), Interact)) {
                Debug.LogError($"{transform} unable to add actions to InteractionEvent bus!");
            }
        }
        protected void OnDisable() {
            EventBus<InteractionEvent>.RemoveBinding(transform.GetInstanceID());
            OnInteract.RemoveAllListeners();
        }
        public void Interact(InteractionEvent @event) {
            OnInteract?.Invoke(@event.Interactor);
        }
    }
}
