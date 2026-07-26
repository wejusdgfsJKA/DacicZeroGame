using UnityEngine;
using UnityEngine.UI;
using DacicZero.Data;

namespace DacicZero.UI.MissionSelector {
    /// <summary> selectable mission location on the map ui. </summary>
    [RequireComponent(typeof(RectTransform))]
    public class MissionNode : MonoBehaviour {
        [SerializeField] private MissionDataSO _missionData;

        [Header("UI Elements")]
        [SerializeField] private GameObject _highlightEffect;

        public MissionDataSO MissionData => _missionData;

        public void SetHighlight(bool active) { if (_highlightEffect != null) _highlightEffect.SetActive(active); }
    }
}
