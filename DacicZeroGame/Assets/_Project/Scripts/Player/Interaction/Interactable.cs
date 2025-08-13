using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    /// <summary>
    /// This fires when this object is interacted with.
    /// </summary>
    protected System.Action<Transform> OnInteract;
    protected void OnEnable()
    {
        InteractableManager.Instance.Register(this);
    }
    protected void OnDisable()
    {
        OnInteract = delegate { };
        InteractableManager.Instance.DeRegister(transform);
    }
    protected void OnDestroy()
    {
        OnDisable();
    }
    /// <summary>
    /// Add an action that will happen when this object is interacted with.
    /// </summary>
    /// <param name="action"></param>
    public void AddAction(System.Action<Transform> action)
    {
        OnInteract += action;
    }
    public void RemoveAction(Action<Transform> action)
    {
        OnInteract -= action;
    }
    /// <summary>
    /// Interact with this object.
    /// </summary>
    /// <param name="source">The entity that interacted with this object.</param>
    public void Interact(Transform source)
    {
        OnInteract(source);
    }
}