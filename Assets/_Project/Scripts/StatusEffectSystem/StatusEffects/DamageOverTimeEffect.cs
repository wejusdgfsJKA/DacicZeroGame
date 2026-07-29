

using EventBus;
using HP;

public class DamageOverTimeStatusEffect : StatusEffect
{
    private int tickDamage;
    /// <summary>
    /// Deals damage multiple times for a set amount of time.
    /// </summary>
    /// <param name="damage"> The damage dealt</param>
    /// <param name="duration"> Total Duration of the effect.</param>
    /// <param name="tickTimer"> How often should the damage be dealt.</param>
    public DamageOverTimeStatusEffect(int damage, float duration, float tickTimer = 1f)
    {
        tickDamage = damage;
        Duration = duration;
        TickTimer = tickTimer;
    }
    public override void OnTick()
    {
        EventBus<TakeDamage>.Raise(TargetCollider.transform.root.GetInstanceID(), new TakeDamage(tickDamage, Source, TargetCollider));
    }
}