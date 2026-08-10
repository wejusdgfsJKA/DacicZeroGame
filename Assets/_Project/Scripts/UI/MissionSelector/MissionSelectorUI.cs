using UnityEngine;
using System.Collections.Generic;
using DacicZero.Data;
using EventBus;

namespace DacicZero.UI.MissionSelector {
    
    #region Events
#warning when upgrading to c# 10, uncomment the record struct below and remove the standard struct to reduce boilerplate.
    // public readonly record struct MapStateChangedEvent(bool IsOpen) : IEvent;
    
    public readonly struct MapStateChangedEvent : IEvent {
        public readonly bool IsOpen;
        public MapStateChangedEvent(bool isOpen) { IsOpen = isOpen; }
    }
    #endregion

    public class MissionSelectorUI : MonoBehaviour {
        
        #region Variables & Properties
        [Header("UI References")]
        [SerializeField] private RectTransform _crosshair;
        [SerializeField] private MapContextMenuUI _contextMenuUI;
        [SerializeField] private MissionLaunchOverlay _launchOverlay;

        [Header("Input")]
        [SerializeField] private PlayerController.InputReader _inputReader;

        [Header("Settings")]
        [SerializeField] private float _cursorSpeed = 500f;
        [SerializeField] private float _selectionToleranceRadius = 50f;

        [Header("Nodes")]
        [Tooltip("If left empty, will auto-populate with child nodes on Awake.")]
        [SerializeField] private List<MissionNode> _availableNodes = new();

        private Vector2 _moveInput;
        private MissionNode _hoveredNode;
        #endregion

        #region Unity Lifecycle
        private void Awake() { 
            if (_availableNodes.Count == 0) 
                _availableNodes.AddRange(GetComponentsInChildren<MissionNode>(true)); 
        }

        private void OnEnable() {
            if (_crosshair != null) _crosshair.anchoredPosition = Vector2.zero;
            
            if (_contextMenuUI != null) _contextMenuUI.Close();
            if (_launchOverlay != null) _launchOverlay.gameObject.SetActive(false);

            if (_inputReader != null) {
                _inputReader.EnablePlayerActions();
                _inputReader.Move += SetMoveInput;
                _inputReader.Interact += OnSelectAction;
            }
        }

        private void OnDisable() {
            if (_inputReader != null) {
                _inputReader.Move -= SetMoveInput;
                _inputReader.Interact -= OnSelectAction;
            }
            if (_hoveredNode != null) {
                _hoveredNode.SetHighlight(false);
                _hoveredNode = null;
            }
        }

        private void Update() {
            if (_moveInput != Vector2.zero && _crosshair != null) {
                Vector2 dir = Vector2.ClampMagnitude(_moveInput, 1f);
                _crosshair.anchoredPosition += dir * _cursorSpeed * Time.deltaTime;

                if (_crosshair.parent is RectTransform parentRect) {
                    Vector2 pos = _crosshair.anchoredPosition;
                    pos.x = Mathf.Clamp(pos.x, parentRect.rect.xMin, parentRect.rect.xMax);
                    pos.y = Mathf.Clamp(pos.y, parentRect.rect.yMin, parentRect.rect.yMax);
                    _crosshair.anchoredPosition = pos;
                }
            }

            UpdateHoverState();
        }
        #endregion

        #region Map Logic
        public void OpenMap() => EventBus<MapStateChangedEvent>.Raise(0, new MapStateChangedEvent(true));
        public void CloseMap() => EventBus<MapStateChangedEvent>.Raise(0, new MapStateChangedEvent(false));

        public void SetMoveInput(Vector2 input) => _moveInput = input;

        private void UpdateHoverState() {
            MissionNode closestNode = GetClosestValidNode();

            if (_hoveredNode != closestNode) {
                if (_hoveredNode != null) _hoveredNode.SetHighlight(false);
                
                _hoveredNode = closestNode;
                
                if (_hoveredNode != null) _hoveredNode.SetHighlight(true);
            }
        }

        public void OnSelectAction() {
            if (_hoveredNode != null) _launchOverlay.Show(_hoveredNode.MissionData);
            else {
                if (_contextMenuUI != null) _contextMenuUI.Show();
                else Debug.LogWarning("[MissionSelectorUI] Context Menu is null! Please assign it in the Inspector.");
            }
        }

        private MissionNode GetClosestValidNode() {
            if (_crosshair == null) return null;

            MissionNode closest = null;
            
            float minSqrDistance = Mathf.Infinity;
            float toleranceSqr = _selectionToleranceRadius * _selectionToleranceRadius;

            Vector2 crosshairScreen = RectTransformUtility.WorldToScreenPoint(null, _crosshair.position);

            foreach (var node in _availableNodes) {
                if (node == null) continue;

                Vector2 nodeScreen = RectTransformUtility.WorldToScreenPoint(null, node.transform.position);
                float sqrDist = (crosshairScreen - nodeScreen).sqrMagnitude;

                if (sqrDist <= toleranceSqr && sqrDist < minSqrDistance) {
                    minSqrDistance = sqrDist;
                    closest = node;
                }
            }

            return closest;
        }
        #endregion
    }
}