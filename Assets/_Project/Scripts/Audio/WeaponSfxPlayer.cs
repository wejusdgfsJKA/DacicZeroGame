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

    private AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        EventBus<WeaponFired>.AddActions(gameObject.GetInstanceID(), actionNoArgs: PlayFireAudio);
        EventBus<WeaponAltFired>.AddActions(gameObject.GetInstanceID(), actionNoArgs: PlayAltFireAudio);
    }

    private void OnDisable()
    {
        EventBus<WeaponFired>.RemoveActions(gameObject.GetInstanceID(), actionNoArgs: PlayFireAudio);
        EventBus<WeaponAltFired>.RemoveActions(gameObject.GetInstanceID(), actionNoArgs: PlayAltFireAudio);
    }

    private void PlayFireAudio()
    {
        Debug.Log("Playing audio for WeaponFire event");
        float randomPitch = pitchFire + Random.Range(-pitchVariation, pitchVariation);
        audioSource.pitch = randomPitch;
        StartCoroutine(PlayWithDelay(fireSfx, audioDelayFire));
    }

    private void PlayAltFireAudio()
    {
        float randomPitch = pitchAltFire + Random.Range(-pitchVariation, pitchVariation);
        audioSource.pitch = randomPitch;
        StartCoroutine(PlayWithDelay(altfireSfx, audioDelayAltFire));
    }

    private IEnumerator PlayWithDelay(AudioClip sound, float time)
    {
        yield return new WaitForSeconds(time);
        audioSource.PlayOneShot(sound);
    }

}
