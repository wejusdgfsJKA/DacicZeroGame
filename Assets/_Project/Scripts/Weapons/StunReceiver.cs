using EventBus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapons;
public class StunReceiver : MonoBehaviour {
    [SerializeField] MonoBehaviour[] scripts;
    List<IStunnable> toDisable = new();
    Coroutine coroutine;
    private void Awake() {
        EventBus<StunEvent>.AddActions(transform.GetInstanceID(), ReceiveStun);
        for (int i = 0; i < scripts.Length; i++) {
            var s = scripts[i] as IStunnable;
            if (s != null) {
                toDisable.Add(s);
            }
        }
    }
    private void OnDisable() {
        if (coroutine != null) {
            StopCoroutine(coroutine);
        }
        for (int i = 0; i < toDisable.Count; i++) {
            toDisable[i].EndStun();
        }
    }
    public void ReceiveStun(StunEvent stunEvent) {
        coroutine = StartCoroutine(stunCoroutine(stunEvent.Duration));
    }
    IEnumerator stunCoroutine(float duration) {
        for (int i = 0; i < toDisable.Count; i++) {
            toDisable[i].Stun();
        }
        yield return new WaitForSeconds(duration);
        for (int i = 0; i < toDisable.Count; i++) {
            toDisable[i].EndStun();
        }
    }
}
