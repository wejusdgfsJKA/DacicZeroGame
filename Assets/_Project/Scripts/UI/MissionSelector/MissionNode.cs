using UnityEngine;
using DacicZero.Data;

namespace DacicZero.UI.MissionSelector {
    [RequireComponent(typeof(RectTransform))]
    public class MissionNode : MonoBehaviour {
        
        [Tooltip("the data defining this mission node")]
        [SerializeField] private MissionDataSO _missionData;

        [Header("UI Elements")]
        [Tooltip("the visual effect shown when this node is selected or hovered")]
        [SerializeField] private GameObject _highlightEffect;

        public MissionDataSO MissionData => _missionData;

        private void Awake() {
            if (_highlightEffect != null) _highlightEffect.SetActive(false);

            if (_missionData == null) Debug.LogError($"[MissionNode] '{gameObject.name}' is missing its MissionDataSO! Please assign it in the Inspector.");
        }

        public void SetHighlight(bool active) { if (_highlightEffect != null) _highlightEffect.SetActive(active); }
    }
}