using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DacicZero.Data;
using DacicZero.UI;

namespace DacicZero.UI.MissionSelector {
    public class MissionLaunchOverlay : MonoBehaviour {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _missionTitleText;
        [SerializeField] private TextMeshProUGUI _missionDescriptionText;
        [SerializeField] private Image _missionImageDisplay;
        [SerializeField] private Button _playButton;

        private MissionDataSO _currentMission;

        private void OnEnable() { 
            if (_playButton != null) {
                _playButton.onClick.AddListener(PlayMission); 
                _playButton.interactable = true;
            }
        }

        private void OnDisable() { 
            if (_playButton != null) _playButton.onClick.RemoveListener(PlayMission); 
        }

        public void Show(MissionDataSO missionData) {
            if (missionData == null) {
                Debug.LogError("MissionLaunchOverlay received null MissionDataSO");
                return;
            }

            gameObject.SetActive(true);
            _currentMission = missionData;

            if (_missionTitleText != null) _missionTitleText.text = missionData.MissionTitle;
            if (_missionDescriptionText != null) _missionDescriptionText.text = missionData.MissionDescription;

            if (_missionImageDisplay != null) {
                bool hasImage = missionData.MissionImage != null;
                _missionImageDisplay.gameObject.SetActive(hasImage);
                if (hasImage) _missionImageDisplay.sprite = missionData.MissionImage;
            }
        }

        public void Close() {
            if (TryGetComponent(out UIAnimator animator)) animator.Hide();
            else gameObject.SetActive(false);
        }

        public void PlayMission() {
            if (_currentMission == null) return;
            if (_playButton != null) _playButton.interactable = false;

            if (_currentMission.SceneBuildIndex >= 0) 
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_currentMission.SceneBuildIndex);
            else if (!string.IsNullOrEmpty(_currentMission.SceneName)) 
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_currentMission.SceneName);
            else {
                Debug.LogError("Mission Data has no valid Scene reference");
                if (_playButton != null) _playButton.interactable = true;
            }
        }
    }
}