using EventBus;
using System;
using UnityEngine;
using Weapons;

public class BowSfxPlayer : WeaponSfxPlayer
{
    [SerializeField]
    private AudioClip chargeSfx;

    [SerializeField]
    private float chargePitch = 1f;

    [SerializeField]
    private float chargeDelay = 0f;

    private Coroutine currentCorutine = null;

    private Boolean chargeSfxIsPlaying = false;
    protected override void OnEnable()
    {
        base.OnEnable();
        EventBus<WeaponChargeStart>.AddActions(gameObject.GetInstanceID(), actionNoArgs: PlayChargeSound);
        EventBus<WeaponChargeStop>.AddActions(gameObject.GetInstanceID(), actionNoArgs: StopChargeSound);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EventBus<WeaponChargeStart>.RemoveActions(gameObject.GetInstanceID(), actionNoArgs: PlayChargeSound);
        EventBus<WeaponChargeStop>.RemoveActions(gameObject.GetInstanceID(), actionNoArgs: StopChargeSound);
    }

    protected override void PlayFireAudio()
    {
        StopChargeSound();
        base.PlayFireAudio();
    }

    protected override void PlayAltFireAudio()
    {
        StopChargeSound();
        base.PlayAltFireAudio();
    }

    private void PlayChargeSound()
    {
        if (!chargeSfxIsPlaying) 
        {
            chargeSfxIsPlaying = true;
            currentCorutine = StartCoroutine(PlayWithDelay(chargeSfx, chargeDelay, chargePitch));
        }

    }

    private void StopChargeSound()
    {
        if (currentCorutine != null)
            StopCoroutine(currentCorutine);
        if (audioSource.clip == chargeSfx)
            audioSource.Stop();
        chargeSfxIsPlaying = false;
    }
}
