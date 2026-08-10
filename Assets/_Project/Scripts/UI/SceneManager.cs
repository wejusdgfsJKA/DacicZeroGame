using UnityEngine;
using EventBus;

namespace DacicZero.UI {
    /// <summary> manages UI visibility, scene transitions and player controls </summary>
    public class SceneManager : MonoBehaviour {
        [Tooltip("main environment background")]
        [SerializeField] private GameObject pnl_Room;
        
        [Tooltip("mission selection map")]
        [SerializeField] private GameObject pnl_MissionSelector;
        
        [Tooltip("context menu for map (return to crosshair or return to room)")]
        [SerializeField] private UI.MissionSelector.MapContextMenuUI _contextMenu;
        
        [Tooltip("details of the selected mission")]
        [SerializeField] private GameObject pnl_MissionOverlay;
        
        [Tooltip("dialog container")]
        [SerializeField] private GameObject pnl_Dialog;
        
        [Tooltip("weapon upgrade interface")]
        [SerializeField] private GameObject pnl_WeaponUpgrade;


        private PlayerController.PlayerMovementController _playerMove;
        private PlayerController.CameraController _playerCam;

        private void Awake() {
            _playerMove = Object.FindFirstObjectByType<PlayerController.PlayerMovementController>(FindObjectsInactive.Include);
            _playerCam = Object.FindFirstObjectByType<PlayerController.CameraController>(FindObjectsInactive.Include);

            EventBus<NPC.StartDialogEvent>.AddActions(0, OnStartDialog);
            EventBus<UI.Dialog.EndDialogEvent>.AddActions(0, OnEndDialog);
            EventBus<UI.Dialog.DialogActionFiredEvent>.AddActions(0, OnDialogAction);
            EventBus<UI.MissionSelector.MapStateChangedEvent>.AddActions(0, OnMapStateChanged);
        }

        private void OnDestroy() {
            EventBus<NPC.StartDialogEvent>.RemoveActions(0, OnStartDialog);
            EventBus<UI.Dialog.EndDialogEvent>.RemoveActions(0, OnEndDialog);
            EventBus<UI.Dialog.DialogActionFiredEvent>.RemoveActions(0, OnDialogAction);
            EventBus<UI.MissionSelector.MapStateChangedEvent>.RemoveActions(0, OnMapStateChanged);
        }

        private void Start() { ReturnToRoom(); }

        private void Update() {
            if (Input.GetButtonDown("Cancel")) {
                if (_contextMenu != null && _contextMenu.gameObject.activeSelf) _contextMenu.Close(); // closes contextmenu without closing missionselector
                else if (pnl_MissionOverlay != null && pnl_MissionOverlay.activeSelf) ShowMissionSelector();
                else if (pnl_WeaponUpgrade != null && pnl_WeaponUpgrade.activeSelf) ReturnToRoom();
                else if (pnl_Dialog != null && pnl_Dialog.activeSelf) return;
                else if (pnl_MissionSelector != null && pnl_MissionSelector.activeSelf) 
                    EventBus<UI.MissionSelector.MapStateChangedEvent>.Raise(0, new UI.MissionSelector.MapStateChangedEvent(false));
            }
        }

        private void OnStartDialog(NPC.StartDialogEvent evt) => ShowDialog();
        
        /// <summary> returns to room if no other menu is active after dialog ends </summary>
        private void OnEndDialog(UI.Dialog.EndDialogEvent evt) {
            if (pnl_MissionSelector != null && pnl_MissionSelector.activeSelf) return;
            if (pnl_WeaponUpgrade != null && pnl_WeaponUpgrade.activeSelf) return;
            ReturnToRoom();
        }
        
        /// <summary> toggles mission map visibility </summary>
        private void OnMapStateChanged(UI.MissionSelector.MapStateChangedEvent evt) {
            if (evt.IsOpen) ShowMissionSelector();
            else ReturnToRoom();
        }
        
        /// <summary> executes specific actions requested by dialog options </summary>
        private void OnDialogAction(UI.Dialog.DialogActionFiredEvent evt) {
            if (string.Equals(evt.ActionID, "OpenUpgradeMenu", System.StringComparison.OrdinalIgnoreCase)) 
                ShowWeaponUpgrade();
            else if (string.Equals(evt.ActionID, "OpenMap", System.StringComparison.OrdinalIgnoreCase)) 
                EventBus<UI.MissionSelector.MapStateChangedEvent>.Raise(0, new UI.MissionSelector.MapStateChangedEvent(true));
        }

        public void ShowMissionSelector() => SwitchMenu(pnl_MissionSelector, false);
        public void ShowMissionOverlay() => SwitchMenu(pnl_MissionOverlay, false);
        public void ShowDialog() => SwitchMenu(pnl_Dialog, false, true);
        public void ShowWeaponUpgrade() => SwitchMenu(pnl_WeaponUpgrade, false);
        public void ReturnToRoom() => SwitchMenu(pnl_Room, true);
        
        /// <summary> centralizes ui state transitions </summary>
        private void SwitchMenu(GameObject targetPanel, bool enablePlayerControls, bool keepRoomActive = false) {
            SetPlayerControls(enablePlayerControls);
            
            if (targetPanel != null) targetPanel.SetActive(true);
            if (keepRoomActive && pnl_Room != null) pnl_Room.SetActive(true);
            if (!keepRoomActive && pnl_Room != null && pnl_Room != targetPanel) pnl_Room.SetActive(false);
            if (pnl_MissionSelector != null && pnl_MissionSelector != targetPanel) pnl_MissionSelector.SetActive(false);
            if (pnl_MissionOverlay != null && pnl_MissionOverlay != targetPanel) pnl_MissionOverlay.SetActive(false);
            if (pnl_Dialog != null && pnl_Dialog != targetPanel) pnl_Dialog.SetActive(false);
            if (pnl_WeaponUpgrade != null && pnl_WeaponUpgrade != targetPanel) pnl_WeaponUpgrade.SetActive(false);
        }

        /// <summary> toggles player input components </summary>
        private void SetPlayerControls(bool state) {
            if (_playerMove == null) _playerMove = Object.FindFirstObjectByType<PlayerController.PlayerMovementController>(FindObjectsInactive.Include);
            if (_playerCam == null) _playerCam = Object.FindFirstObjectByType<PlayerController.CameraController>(FindObjectsInactive.Include);
            
            if (_playerMove != null) _playerMove.enabled = state;
            if (_playerCam != null) _playerCam.enabled = state;
        }
    }
}
