using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DacicZero.Data;
using DacicZero.Global;

namespace DacicZero.UI.MissionSelector {
    /// <summary> confirmation overlay before launching a mission. </summary>
    public class MissionLaunchOverlay : MonoBehaviour, IClosable {
        [Header("UI References")]
        [SerializeField] private GameObject _overlayPanel;
        [SerializeField] private TextMeshProUGUI _missionTitleText;
        [SerializeField] private TextMeshProUGUI _missionDescriptionText;
        [SerializeField] private Image _missionImageDisplay;

        private MissionDataSO _currentMission;

        public void Show(MissionDataSO missionData) {
            _currentMission = missionData;

            _missionTitleText.text = missionData.MissionTitle;
            _missionDescriptionText.text = missionData.MissionDescription;

            if (_missionImageDisplay != null) {
                _missionImageDisplay.sprite = missionData.MissionImage;
                _missionImageDisplay.gameObject.SetActive(missionData.MissionImage != null);
            }

            if (_overlayPanel != null) _overlayPanel.SetActive(true);
            UIManager.RegisterMenu(this);
        }

        public void Close() {
            if (_overlayPanel != null) _overlayPanel.SetActive(false);
            _currentMission = null;
            UIManager.UnregisterMenu(this);
        }

        public bool IsOpen => _overlayPanel != null && _overlayPanel.activeSelf;

        public void PlayMission() {
            if (_currentMission == null) return;

            if (_currentMission.SceneBuildIndex >= 0)
                SceneManager.LoadScene(_currentMission.SceneBuildIndex);
            else if (!string.IsNullOrEmpty(_currentMission.SceneName))
                SceneManager.LoadScene(_currentMission.SceneName);
            else
                Debug.LogError("Mission Data has no valid Scene reference!");
        }
    }
}
