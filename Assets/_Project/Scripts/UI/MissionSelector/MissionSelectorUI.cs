using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DacicZero.Data;
using EventBus;

namespace DacicZero.UI.MissionSelector {
    /// <summary> fired when map opens/closes </summary>
    public readonly struct MapStateChangedEvent : IEvent {
        /// <summary> true if map opened, false if closed </summary>
        public readonly bool IsOpen;
        public MapStateChangedEvent(bool isOpen) { IsOpen = isOpen; }
    }
    
    /// <summary> handles map cursor, selecting nodes, and map overlays </summary>
    public class MissionSelectorUI : MonoBehaviour, IClosable {
        [Header("UI References")]
        /// <summary> root container </summary>
        [SerializeField] private GameObject _mapCanvas;
        /// <summary> cursor moved by the player </summary>
        [SerializeField] private RectTransform _crosshair;
        /// <summary> menu for clicking empty space </summary>
        [SerializeField] private MapContextMenuUI _contextMenuUI;
        /// <summary> confirmation popup for launching a mission </summary>
        [SerializeField] private MissionLaunchOverlay _launchOverlay;

        [Header("Input")]
        [SerializeField] private PlayerController.InputReader _inputReader;

        [Header("Settings")]
        /// <summary> cursor move speed </summary>
        [SerializeField] private float _cursorSpeed = 500f;
        /// <summary> snap distance for selecting a node </summary>
        [SerializeField] private float _selectionToleranceRadius = 50f;

        [Header("Nodes")]
        /// <summary> active mission locations on the map </summary>
        [SerializeField] private List<MissionNode> _availableNodes;

        private bool _isOpen = false;
        private Vector2 _moveInput;

        private void Awake() {
            EventBus<UI.Dialog.DialogActionFiredEvent>.AddActions(0, OnDialogActionFired);
            _availableNodes = new List<MissionNode>(GetComponentsInChildren<MissionNode>(true));
        }

        private void OnEnable() {
            if (_inputReader != null) {
                _inputReader.Move += SetMoveInput;
                _inputReader.Interact += OnSelectAction;
            }
        }

        private void Start() => Close();

        private void OnDisable() {
            if (_inputReader != null) {
                _inputReader.Move -= SetMoveInput;
                _inputReader.Interact -= OnSelectAction;
            }
        }

        private void OnDestroy() => EventBus<UI.Dialog.DialogActionFiredEvent>.RemoveActions(0, OnDialogActionFired);

        private void OnDialogActionFired(UI.Dialog.DialogActionFiredEvent evt) {
            if (string.Equals(evt.ActionID, "OpenMap", System.StringComparison.OrdinalIgnoreCase))
                OpenMap();
        }

        private void Update() {
            if (!_isOpen) return;

            if (_contextMenuUI != null && _contextMenuUI.IsOpen) return;
            if (_launchOverlay != null && _launchOverlay.IsOpen) return;

            if (_moveInput != Vector2.zero)
                _crosshair.anchoredPosition += _moveInput * _cursorSpeed * Time.deltaTime;
        }

        public bool IsOpen => _isOpen;

        public void OpenMap() {
            _isOpen = true;
            if (_mapCanvas != null) _mapCanvas.SetActive(true);

            if (_contextMenuUI != null) _contextMenuUI.Close();
            if (_launchOverlay != null) _launchOverlay.Close();

            UIManager.RegisterMenu(this);
            EventBus<MapStateChangedEvent>.Raise(0, new MapStateChangedEvent(true));
        }

        public void Close() {
            _isOpen = false;
            if (_mapCanvas != null) _mapCanvas.SetActive(false);

            if (_contextMenuUI != null) _contextMenuUI.Close();
            if (_launchOverlay != null) _launchOverlay.Close();

            UIManager.UnregisterMenu(this);
            EventBus<MapStateChangedEvent>.Raise(0, new MapStateChangedEvent(false));
        }

        public void SetMoveInput(Vector2 input) => _moveInput = input;

        public void OnSelectAction() {
            if (!_isOpen || (_contextMenuUI != null && _contextMenuUI.IsOpen) || (_launchOverlay != null && _launchOverlay.IsOpen)) return;

            MissionNode closestNode = GetClosestValidNode();

            if (closestNode != null)
                _launchOverlay.Show(closestNode.MissionData);
            else
                _contextMenuUI.Show();
        }

        private MissionNode GetClosestValidNode() {
            MissionNode closest = null;
            float minDistance = Mathf.Infinity;

            foreach (var node in _availableNodes) {
                if (!node.TryGetComponent(out RectTransform rt)) continue;

                float dist = Vector2.Distance(_crosshair.anchoredPosition, rt.anchoredPosition);

                if (dist <= _selectionToleranceRadius && dist < minDistance) {
                    minDistance = dist;
                    closest = node;
                }
            }

            return closest;
        }

        public void ReturnToRoom() => Close();
    }
}
