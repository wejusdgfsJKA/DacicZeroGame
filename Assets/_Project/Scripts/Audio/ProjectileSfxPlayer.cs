using EventBus;
using UnityEngine;
using UnityEngine.Audio;

public class ProjectileSfxPlayer : MonoBehaviour
{

    [SerializeField]
    private AudioClip impactSfx;

    [SerializeField, Range(0f, 1.5f)]
    private float clipVolume;

    [SerializeField]
    private float pitchImpact = 1f;

    [SerializeField]
    private float impactPitchVariation = 0.2f;

    [SerializeField]
    private AudioMixerGroup mixerGroup;

    private void OnEnable()
    {
        EventBus<ProjectileImpact>.AddActions(gameObject.GetInstanceID(), actionNoArgs: PlayImpactAudio);
    }

    private void OnDisable()
    {
        EventBus<ProjectileImpact>.RemoveActions(gameObject.GetInstanceID(), actionNoArgs: PlayImpactAudio);
    }
    private void PlayImpactAudio()
    {
        //AudioSource.PlayClipAtPoint(impactSfx, gameObject.transform.position, clipVolume);
        GameObject tempObject = new GameObject("tempAudio");
        tempObject.transform.position = transform.position;
        AudioSource newAudioSource = tempObject.AddComponent<AudioSource>();
        float randomPitch = pitchImpact + Random.Range(-impactPitchVariation, impactPitchVariation);
        newAudioSource.pitch = randomPitch;
        newAudioSource.volume = clipVolume;
        newAudioSource.outputAudioMixerGroup = mixerGroup;
        newAudioSource.spatialBlend = 1f;
        newAudioSource.PlayOneShot(impactSfx);
        float clipLength = impactSfx.length / Mathf.Max(0.1f, Mathf.Abs(randomPitch)) + 0.1f;
        Destroy(tempObject, clipLength);
    }
}
