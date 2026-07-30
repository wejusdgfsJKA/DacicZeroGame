using EventBus;
using NUnit.Framework.Internal;
using System;
using UnityEngine;
using UnityEngine.Events;
public class StatusEffect
{
    public Collider TargetCollider;
    public Transform Source;

    public float Duration; // total Duration of the effect
    public float TickTimer = -1f; // how often should OnTick proc
    public float TimeUntilNextTick;
    public virtual void OnTick() { } // an action that would happen multiple times ex: takedamage for damage over time effects
    public virtual void OnApply() { } // an action that would happen when applying the effect ex: decrease walkspeed by 10
    public virtual void OnRemove() { } // an action that happens after the effect's duration has passes, usually just undoing the OnApply
}