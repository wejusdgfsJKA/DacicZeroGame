using EventBus;
using System.Collections;
using UnityEngine;
using Weapons;

public class WeaponSfxPlayer : MonoBehaviour
{
    [SerializeField]
    private float audioDelayFire;

    [SerializeField]
    private float pitchFire;

    [SerializeField]
    private float pitchAltFire;

    [SerializeField]
    private float audioDelayAltFire;

    [SerializeField]
    private AudioClip fireSfx, altfireSfx;

    [SerializeField]
    private float pitchVariation = 0.2f;

    protected AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    protected virtual void OnEnable()
    {
        EventBus<WeaponFired>.AddActions(gameObject.GetInstanceID(), actionNoArgs: PlayFireAudio);
        EventBus<WeaponAltFired>.AddActions(gameObject.GetInstanceID(), actionNoArgs: PlayAltFireAudio);
    }

    protected virtual void OnDisable()
    {
        EventBus<WeaponFired>.RemoveActions(gameObject.GetInstanceID(), actionNoArgs: PlayFireAudio);
        EventBus<WeaponAltFired>.RemoveActions(gameObject.GetInstanceID(), actionNoArgs: PlayAltFireAudio);
    }

    protected virtual void PlayFireAudio()
    {
        float randomPitch = pitchFire + Random.Range(-pitchVariation, pitchVariation);
        StartCoroutine(PlayWithDelay(fireSfx, audioDelayFire, randomPitch));
    }

    protected virtual void PlayAltFireAudio()
    {
        float randomPitch = pitchAltFire + Random.Range(-pitchVariation, pitchVariation);
        StartCoroutine(PlayWithDelay(altfireSfx, audioDelayAltFire, randomPitch));
    }

    protected IEnumerator PlayWithDelay(AudioClip sound, float time, float pitch)
    {
        yield return new WaitForSeconds(time);
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(sound);
    }

}
