    using EventBus;
using UnityEngine;

public class ProjectileSfxPlayer : MonoBehaviour
{

    [SerializeField]
    private AudioClip impactSfx;

    [SerializeField, Range(0f, 1.5f)]
    private float clipVolume;

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
        AudioSource.PlayClipAtPoint(impactSfx, gameObject.transform.position, clipVolume);
    }
}
