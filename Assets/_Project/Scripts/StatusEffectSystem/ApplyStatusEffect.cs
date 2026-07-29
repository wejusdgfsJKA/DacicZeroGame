using EventBus;
using UnityEngine;

public struct ApplyStatusEffect : IEvent
{
    /// <summary>
    /// The status effect that should be applied.
    /// </summary>
    public StatusEffect StatusEffect { get; set; }
    /// <summary>
    /// The source of the damage.
    /// </summary>
    public Transform Source { get; set; }
    /// <summary>
    /// The collider we hit.
    /// </summary>
    public Collider ColliderHit { get; set; }
    public ApplyStatusEffect(StatusEffect statusEffect, Transform source, Collider collider)
    {
        StatusEffect = statusEffect;
        Source = source;
        ColliderHit = collider;
    }
}