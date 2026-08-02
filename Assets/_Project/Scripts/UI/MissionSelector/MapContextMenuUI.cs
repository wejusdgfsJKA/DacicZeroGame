using UnityEngine;
using DacicZero.UI;

namespace DacicZero.UI.MissionSelector {
    public class MapContextMenuUI : MonoBehaviour {
        public bool IsOpen => gameObject.activeSelf;
        public void Show() {  gameObject.SetActive(true); }
        public void Close() {
            if (TryGetComponent(out UIAnimator animator)) animator.Hide();
            else gameObject.SetActive(false);
        }
    }
}