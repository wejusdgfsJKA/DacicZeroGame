using UnityEngine;
using PlayerController;
using Interaction;
using EventBus;

namespace DacicZero.Prep {
    public class PrepPlayerController : MonoBehaviour {
        
        #region Variables & Properties
        [Header("References")]
        [SerializeField] private InputReader _inputReader;

        [Header("Settings")]
        [Tooltip("movement speed (~500 for Canvas, ~5 for World Space).")]
        [SerializeField] private float _moveSpeed = 500f;

        [Tooltip("interaction radius for objects/NPCs.")]
        [SerializeField] private float _interactionRadius = 100f;

        [Tooltip("layer mask for interactable objects (optimizes physics).")]
        [SerializeField] private LayerMask _interactionLayer;

        public float MoveSpeed => _moveSpeed;
        public float InteractionRadius => _interactionRadius;
        public LayerMask InteractionLayer => _interactionLayer;

        public bool CanMove => !_isDialogOpen && !_isMapOpen;

        private Vector2 _currentMoveInput;
        private bool _isDialogOpen;
        private bool _isMapOpen;
        private int _frameMenuClosed = -1;
        private readonly Collider[] _overlapResults = new Collider[10];
        #endregion

        #region Unity Lifecycle
        private void OnEnable() {
            _isDialogOpen = false;
            _isMapOpen = false;
            _currentMoveInput = Vector2.zero;

            if (_inputReader != null) {
                _inputReader.EnablePlayerActions();
                _inputReader.Move += OnMove;
                _inputReader.Interact += OnInteract;
            }
            EventBus<NPC.StartDialogEvent>.AddActions(0, OnStartDialog);
            EventBus<UI.Dialog.EndDialogEvent>.AddActions(0, OnEndDialog);
            EventBus<UI.MissionSelector.MapStateChangedEvent>.AddActions(0, OnMapStateChanged);
        }

        private void OnDisable() {
            if (_inputReader != null) {
                _inputReader.Move -= OnMove;
                _inputReader.Interact -= OnInteract;
            }
            EventBus<NPC.StartDialogEvent>.RemoveActions(0, OnStartDialog);
            EventBus<UI.Dialog.EndDialogEvent>.RemoveActions(0, OnEndDialog);
            EventBus<UI.MissionSelector.MapStateChangedEvent>.RemoveActions(0, OnMapStateChanged);
        }

        private void Update() {
            if (!CanMove) return;

            Vector2 dir = (_currentMoveInput == Vector2.zero && _inputReader != null) ? _inputReader.Direction : _currentMoveInput;
            if (dir == Vector2.zero) return;

            Vector3 movement = Vector3.ClampMagnitude(new Vector3(dir.x, dir.y, 0f), 1f);
            
            transform.Translate(movement * MoveSpeed * Time.deltaTime);
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, InteractionRadius);
        }
        #endregion

        #region Event Handlers
        private void OnStartDialog(NPC.StartDialogEvent evt) {
            _isDialogOpen = true;
            _currentMoveInput = Vector2.zero;
        }
        
        private void OnEndDialog(UI.Dialog.EndDialogEvent evt) {
            _isDialogOpen = false;
            _frameMenuClosed = Time.frameCount;
        }

        private void OnMapStateChanged(UI.MissionSelector.MapStateChangedEvent evt) {
            _isMapOpen = evt.IsOpen;
            
            if (evt.IsOpen) _currentMoveInput = Vector2.zero;
            else _frameMenuClosed = Time.frameCount;
        }

        private void OnMove(Vector2 input) => _currentMoveInput = input;
        #endregion

        #region Interaction Logic
        private void OnInteract() {
            if (!CanMove || Time.frameCount == _frameMenuClosed) return;

            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, InteractionRadius, _overlapResults, InteractionLayer);

            float closestSqrDistance = float.MaxValue;
            Interactable closestInteractable = null;

            for (int i = 0; i < hitCount; i++) {
                if (_overlapResults[i].TryGetComponent(out Interactable interactable)) {
                    float sqrDist = (transform.position - _overlapResults[i].transform.position).sqrMagnitude;

                    if (sqrDist < closestSqrDistance) {
                        closestSqrDistance = sqrDist;
                        closestInteractable = interactable;
                    }
                }
            }

            if (closestInteractable != null) 
                EventBus<InteractionEvent>.Raise(closestInteractable.transform.GetInstanceID(), new InteractionEvent(transform));
        }
        #endregion
    }
}