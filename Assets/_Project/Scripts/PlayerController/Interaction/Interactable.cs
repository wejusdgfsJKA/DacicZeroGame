using EventBus;
using UnityEngine;
using UnityEngine.Events;
namespace Interaction
{
    public class Interactable : MonoBehaviour
    {
        /// <summary>
        /// This fires when this object is interacted with.
        /// </summary>
        public UnityEvent<Transform> OnInteract;
        
        // CHANGED: Awake -> OnEnable so it re-subscribes if toggled off/on during gameplay.
        protected void OnEnable()
        {
            if (!EventBus<InteractionEvent>.AddActions(transform.GetInstanceID(), Interact))
            {
                Debug.LogError($"{transform} unable to add actions to InteractionEvent bus!");
            }
        }
        
        protected void OnDisable()
        {
            // CHANGED: removed error log. fails silently to prevent console spam when exiting Play Mode.
            EventBus<InteractionEvent>.RemoveBinding(transform.GetInstanceID());
            
            // CHANGED: removed OnInteract.RemoveAllListeners() so it doesn't permanently delete Inspector setups.
        }
        
        public void Interact(InteractionEvent @event)
        {
            OnInteract?.Invoke(@event.Interactor);
        }
    }
}