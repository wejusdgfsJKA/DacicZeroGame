using System.Collections.Generic;
using UnityEngine;

public class InteractableManager : MonoBehaviour
{
    public static InteractableManager Instance { get; private set; }
    protected Dictionary<int, Interactable> interactables { get; } = new();
    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    /// <summary>
    /// An entity is trying to interact with an object.
    /// </summary>
    /// <param name="target">The object in question.</param>
    /// <param name="interactor">The entity in question.</param>
    /// <returns>True if the interaction was successfull, false otherwise.</returns>
    public bool Interact(Transform target, Transform interactor)
    {
        Interactable interactable;
        if (interactables.TryGetValue(target.GetInstanceID(), out interactable))
        {
            interactable.Interact(interactor);
            return true;
        }
        return false;
    }
    /// <summary>
    /// Checks if an object is an interactable.
    /// </summary>
    /// <param name="interactable"></param>
    /// <returns>True if an object is an interactable.</returns>
    public bool IsInteractable(Transform interactable)
    {
        return interactables.ContainsKey(interactable.GetInstanceID());
    }
    /// <summary>
    /// Register an active interactable, so it can be interacted with.
    /// </summary>
    /// <param name="interactable">The interactable that wants to register itself.</param>
    /// <returns>True if the registration was successfull, false otherwise.</returns>
    public bool Register(Interactable interactable)
    {
        if (!interactables.ContainsKey(interactable.transform.GetInstanceID()))
        {
            interactables.Add(interactable.transform.GetInstanceID(), interactable);
            return true;
        }
        return false;
    }
    /// <summary>
    /// De-register an interactable, so it can no longer be interacted with.
    /// </summary>
    /// <param name="transform">The interactable to be de-registered.</param>
    /// <returns>True if de-registration was successfull, false otherwise.</returns>
    public bool DeRegister(Transform transform)
    {
        return interactables.Remove(transform.GetInstanceID());
    }
    private void OnApplicationQuit()
    {
        interactables.Clear();
    }
}