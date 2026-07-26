using UnityEngine;
using PlayerController;
using Interaction;
using EventBus;

namespace DacicZero.Prep {
    /// <summary> handles 2d prep phase movement and interaction. </summary>
    public class PrepPlayerController : MonoBehaviour {
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

        // Calculam CanMove dinamic. Eliminam starea redundanta _canMove si functia UpdateCanMove()
        public bool CanMove => !_isDialogOpen && !_isMapOpen;

        private Vector2 _currentMoveInput;
        private bool _isDialogOpen;
        private bool _isMapOpen;
        private int _frameDialogEnded = -1;
        private readonly Collider[] _overlapResults = new Collider[10];

        private void OnEnable() {
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
                _inputReader.DisablePlayerActions();
            }
            EventBus<NPC.StartDialogEvent>.RemoveActions(0, OnStartDialog);
            EventBus<UI.Dialog.EndDialogEvent>.RemoveActions(0, OnEndDialog);
            EventBus<UI.MissionSelector.MapStateChangedEvent>.RemoveActions(0, OnMapStateChanged);
        }

        // Folosim Expression-Bodied Methods pentru evenimentele scurte
        private void OnStartDialog(NPC.StartDialogEvent evt) => _isDialogOpen = true;
        private void OnEndDialog(UI.Dialog.EndDialogEvent evt) {
            _isDialogOpen = false;
            _frameDialogEnded = Time.frameCount;
        }

        private void OnMapStateChanged(UI.MissionSelector.MapStateChangedEvent evt) {
            _isMapOpen = evt.IsOpen;
            if (!evt.IsOpen) _frameDialogEnded = Time.frameCount;
        }

        private void OnMove(Vector2 input) => _currentMoveInput = input;

        private void Update() {
            // Micro-optimizare: Daca nu ne miscam, nu facem inmultiri de vectori inutile
            if (!CanMove || _currentMoveInput == Vector2.zero) return;

            Vector3 movement = new Vector3(_currentMoveInput.x, _currentMoveInput.y, 0f);
            transform.Translate(movement * MoveSpeed * Time.deltaTime);
        }

        private void OnInteract() {
            if (!CanMove || Time.frameCount == _frameDialogEnded) return;

            // 1. Optimizare Fizica: Cautam doar pe layer-ul specificat
            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, InteractionRadius, _overlapResults, InteractionLayer);

            float closestSqrDistance = float.MaxValue;
            Interactable closestInteractable = null;

            for (int i = 0; i < hitCount; i++) {
                if (_overlapResults[i].TryGetComponent(out Interactable interactable)) {
                    // 2. Optimizare Matematica: sqrMagnitude e mult mai rapid decat Vector3.Distance
                    float sqrDist = (transform.position - _overlapResults[i].transform.position).sqrMagnitude;

                    if (sqrDist < closestSqrDistance) {
                        closestSqrDistance = sqrDist;
                        closestInteractable = interactable;
                    }
                }
            }

            if (closestInteractable != null) {
                EventBus<InteractionEvent>.Raise(closestInteractable.transform.GetInstanceID(), new InteractionEvent(transform));
            }
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, InteractionRadius);
        }
    }
}
