using EventBus;
using HP;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StatusEffectComponent : MonoBehaviour
{
    List<StatusEffect> effects;
    protected void Awake()
    {
        //add damage binding for this entity
        EventBus<ApplyStatusEffect>.AddActions(transform.GetInstanceID(), ApplyStatusEffect);
    }
    protected void OnEnable()
    {
        effects = new List<StatusEffect>();
    }

    public void ApplyStatusEffect(ApplyStatusEffect applyStatusEffect)
    {
        StatusEffect effect = applyStatusEffect.StatusEffect;
        effect.TargetCollider = applyStatusEffect.ColliderHit;
        effect.Source = applyStatusEffect.Source;

        effects.Add(effect);
        effect.OnApply();
    }

    public void Update()
    {
        for (int i = effects.Count - 1; i >= 0; i--)
        {
            var effect = effects[i];

            if (effect.TickTimer > 0f)
            {
                effect.TimeUntilNextTick -= Time.deltaTime;
                if (effect.TimeUntilNextTick <= 0)
                {
                    effect.OnTick();
                    effect.TimeUntilNextTick = effect.TickTimer;
                }
            }

            effect.Duration -= Time.deltaTime;
            if (effect.Duration <= 0)
            {
                effect.OnRemove();
                effects.RemoveAt(i);
            }
        }
    }

    protected void OnDestroy()
    {
        //clear this binding from the event bus.
        EventBus<ApplyStatusEffect>.RemoveActions(transform.GetInstanceID(), ApplyStatusEffect);
    }
}