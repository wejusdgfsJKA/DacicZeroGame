using System.Collections;
using UnityEngine;

namespace MBT {
    public class StunnableExecutor : MonoBehaviour, IStunnable {
        [SerializeField] protected MonoBehaviourTree monoBehaviourTree;
        [SerializeField] protected float tickRate = 0.1f;
        protected WaitForSeconds wait;
        protected WaitUntil waitUntil;
        Coroutine coroutine;
        public bool stunned;
        private void Awake() {
            wait = new WaitForSeconds(tickRate);
            waitUntil = new WaitUntil(() => !stunned);
        }
        void Reset() {
            monoBehaviourTree = GetComponent<MonoBehaviourTree>();
            OnValidate();
        }
        private void OnEnable() {
            stunned = false;
            coroutine = StartCoroutine(tickBT());
        }
        IEnumerator tickBT() {
            while (true) {
                yield return wait;
                yield return waitUntil;
                monoBehaviourTree.Tick();
            }
        }
        private void OnDisable() {
            if (coroutine != null) {
                StopCoroutine(coroutine);
            }
        }

        void OnValidate() {
            if (monoBehaviourTree != null && monoBehaviourTree.parent != null) {
                monoBehaviourTree = null;
                Debug.LogWarning("Subtree should not be target of update. Select parent tree instead.", this.gameObject);
            }
        }

        public void Stun() {
            stunned = true;
        }

        public void EndStun() {
            stunned = false;
        }
    }
}
