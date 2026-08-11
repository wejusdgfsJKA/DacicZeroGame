using EventBus;
using Interaction;
using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Timeline;


namespace PlayerController {
    public class CameraController : MonoBehaviour {
        #region Fields
        float currentXAngle;
        float currentYAngle;

        [Range(0f, 90f)] public float VerticalLimit = 35f;

        public float cameraSpeed = 0.01f;
        public bool smoothCameraRotation;
        [Range(1f, 50f)] public float cameraSmoothingFactor = 25f;
        [SerializeField] Transform cam;
        [SerializeField] InputReader inputReader;

        Mutex cameraMutex = new();
        bool justGotForceRotated = false;

        //The interactable we are currently able to interact with. DO NOT CHANGE!!!
        [SerializeField] protected Transform currentInteractable = null;
        protected Transform currentInteractableProperty {
            get {
                return currentInteractable;
            }
            set {
                if (currentInteractable != value) {
                    if (value == null) {
                        if (interactionPrompt != null) {
                            interactionPrompt.enabled = false;
                        }
                    }
                    else {
                        if (interactionPrompt != null) {
                            interactionPrompt.enabled = true;
                        }
                    }
                    currentInteractable = value;
                }
            }
        }
        protected RaycastHit hit;
        [SerializeField] TextMeshProUGUI interactionPrompt;
        #endregion

        void Awake() {
            currentXAngle = transform.localRotation.eulerAngles.x;
            currentYAngle = transform.localRotation.eulerAngles.y;
        }
        private void OnEnable() {
            currentInteractableProperty = null;
            interactionPrompt.enabled = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            inputReader.EnablePlayerActions();
            UpdateInteractionPrompt();
            inputReader.Interact += OnInteract;
            Debug.Log("CameraController enabled");
        }
        private void OnDisable() {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            inputReader.DisablePlayerActions();
            inputReader.Interact -= OnInteract;
            Debug.Log("CameraController disabled");
        }

        Quaternion TargetRotation;
        void LateUpdate()
        {
            if (!justGotForceRotated)
                RotateCamera(inputReader.LookDirection.x, -inputReader.LookDirection.y);
            else
            {
                SnapCameraToTargetRotation();
                justGotForceRotated = false;
            }
            InteractionCheck();
        }

        void RotateCamera(float horizontalInput, float verticalInput) {
            if (smoothCameraRotation) {
                horizontalInput = Mathf.Lerp(0, horizontalInput, Time.deltaTime * cameraSmoothingFactor);
                verticalInput = Mathf.Lerp(0, verticalInput, Time.deltaTime * cameraSmoothingFactor);
            }

            currentXAngle += verticalInput * cameraSpeed;
            currentYAngle += horizontalInput * cameraSpeed;

            currentXAngle = Mathf.Clamp(currentXAngle, -VerticalLimit, VerticalLimit);

            transform.localRotation = Quaternion.Euler(0, currentYAngle, 0);
            cam.localRotation = Quaternion.Euler(currentXAngle, 0, 0);
        }

        public void SetCameraRotation(Quaternion rotation)
        {
            justGotForceRotated = true;
            TargetRotation = rotation;
        }
        void SnapCameraToTargetRotation()
        {
            currentYAngle = TargetRotation.eulerAngles.y;

            currentXAngle = Mathf.Clamp(currentXAngle, -VerticalLimit, VerticalLimit);

            transform.localRotation = Quaternion.Euler(0, currentYAngle, 0);
            cam.localRotation = Quaternion.Euler(currentXAngle, 0, 0);
        }

        /// <summary>
        /// Updates the interaction prompt. Should only fire if the interact key changes.
        /// </summary>
        protected void UpdateInteractionPrompt() {
            if (interactionPrompt != null) {
                interactionPrompt.text = $"[{inputReader.InteractKey}] to interact.";
            }
        }

        /// <summary>
        /// Check if we are able to interact with anything. Updates currentInteractableProperty accordingly.
        /// </summary>
        protected void InteractionCheck() {
            if (Physics.SphereCast(transform.position, transform.localScale.x / 2,
                transform.forward, out hit, GlobalPlayerConfig.InteractionDistance, 1 << 5)) {
                if(!Physics.Raycast(transform.position,
                                    transform.forward,
                                    out _,
                                    GlobalPlayerConfig.InteractionDistance,
                                    LayerMask.GetMask("Default", "Bots")
                                    )
                    )
                {
                    currentInteractableProperty = hit.transform;
                    return;
                }
            }
            currentInteractableProperty = null;
        }

        public void OnInteract() {
            if (currentInteractable != null) {
                //attempt to interact with something in front of us
                EventBus<InteractionEvent>.Raise(currentInteractable.GetInstanceID(),
                    new InteractionEvent(transform));
            }
        }
    }
}
