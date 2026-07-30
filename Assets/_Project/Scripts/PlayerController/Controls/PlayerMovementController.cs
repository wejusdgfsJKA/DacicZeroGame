using UnityEditor;
using UnityEngine;
namespace PlayerController
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerMovementController : MonoBehaviour
    {
        #region Fields
        [SerializeField] InputReader inputReader;
        [SerializeField] Rigidbody PlayerBody;
        [SerializeField] CapsuleCollider capsuleCollider;
        [SerializeField] Transform camPivot;
        [SerializeField] Camera playerCamera;
        [SerializeField] public Transform groundCheckPoint;
        public bool Grounded { get; protected set; }
        bool onSlope;
        RaycastHit slopeHit;
        Vector2 inputVector;
        #endregion

        private void Awake()
        {
            PlayerBody = GetComponent<Rigidbody>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            PlayerBody.useGravity = false;
            PlayerBody.freezeRotation = true;
        }

        private void OnEnable()
        {
            inputReader.EnablePlayerActions();
            inputReader.Jump += OnJump;
            inputReader.Move += OnMove;
            inputReader.Crouch += OnCrouch;
            inputReader.Sprint += OnSprint;
            
        }

        private void OnDisable()
        {
            inputReader.Move -= OnMove;
            inputReader.Jump -= OnJump;
            inputReader.DisablePlayerActions();
            inputReader.Crouch -= OnCrouch;
            inputReader.Sprint -= OnSprint;
        }

        private void Update()
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, 8f * Time.deltaTime);
        }
        private void FixedUpdate()
        {
            GroundCheck();
            ApplyGravity();
            Vector3 targetVelocity = CalculateTargetVelocity();
            UpdateCrouchAndSlideState();
            ApplyMovement(targetVelocity);
        }


        void GroundCheck()
        {
            Grounded = Physics.CheckSphere(groundCheckPoint.position, GlobalPlayerConfig.PlayerGroundCheckRadius, GlobalPlayerConfig.GroundLayerMask);
            if (Grounded)
            {
                SlopeCheck();
            }
            else
            {
                onSlope = false;
            }
        }
        void SlopeCheck()
        {
            Physics.Raycast(groundCheckPoint.position, -groundCheckPoint.up, out slopeHit, GlobalPlayerConfig.PlayerGroundCheckRadius, GlobalPlayerConfig.GroundLayerMask);
            onSlope = slopeHit.normal != Vector3.up;
        }
        private void ApplyGravity()
        {
            if (!Grounded)
            {
                PlayerBody.linearVelocity -= transform.up * GlobalPlayerConfig.Gravity * Time.fixedDeltaTime;
            }
        }

        private Vector3 CalculateTargetVelocity()
        {
            Vector3 dir = (transform.forward * inputVector.y + transform.right * inputVector.x).normalized;
            Vector3 target = dir * GlobalPlayerConfig.PlayerSpeed;

            if (isCrouching && !isSprinting)
                target *= GlobalPlayerConfig.PlayerCrouchSpeedMultiplier;
            else if (isSprinting)
                target *= GlobalPlayerConfig.PlayerSprintSpeedMultiplier;

            if (onSlope)
                target = Vector3.ProjectOnPlane(target, slopeHit.normal);

            return target;
        }

        private void UpdateCrouchAndSlideState()
        {
            if (!isCrouching) return;

            if (!Grounded && !isSliding && PlayerBody.linearVelocity.y <= 0)
            {
                // groundpound! (might not make the final cut)
                PlayerBody.linearVelocity = new Vector3(0, GlobalPlayerConfig.GroundPoundForce, 0);
                isSprinting = false;
            }

            if (isSliding && PlayerBody.linearVelocity.magnitude < GlobalPlayerConfig.PlayerSpeed)
            {
                isSliding = false; // stop the slide if you're too slow
                isSprinting = false;
            }
        }

        private void ApplyMovement(Vector3 targetVelocity)
        {
            float accel = isSliding ? GlobalPlayerConfig.PlayerAcceleration * GlobalPlayerConfig.SlidingControlMultiplier
                        : Grounded ? GlobalPlayerConfig.PlayerAcceleration
                        : GlobalPlayerConfig.PlayerAcceleration * GlobalPlayerConfig.AirControlMultiplier;

            Vector3 currentHorizontal = new Vector3(PlayerBody.linearVelocity.x, 0f, PlayerBody.linearVelocity.z);
            Vector3 newHorizontal = Vector3.MoveTowards(currentHorizontal, targetVelocity, accel * Time.fixedDeltaTime);

            PlayerBody.linearVelocity = new Vector3(newHorizontal.x, PlayerBody.linearVelocity.y, newHorizontal.z);
        }
        public void OnMove(Vector2 inputVector)
        {
            this.inputVector = inputVector;
        }

        public void OnJump()
        {
            if (Grounded)
            {
                PlayerBody.AddForce(transform.up * GlobalPlayerConfig.JumpForce, ForceMode.Impulse);

                if (isSliding)
                    PlayerBody.AddForce(transform.forward * GlobalPlayerConfig.JumpForce, ForceMode.Impulse);
            }
        }


        bool isCrouching;
        bool isSliding;
        public void OnCrouch(bool isHeld)
        {
            isCrouching = isHeld;

            if (isHeld)
            {
                capsuleCollider.height = GlobalPlayerConfig.PlayerCrouchingHeight;
                camPivot.localPosition = new Vector3(camPivot.localPosition.x, GlobalPlayerConfig.PlayerCameraCrouchingHeight, camPivot.localPosition.z);
            }
            else
            {
                capsuleCollider.height = GlobalPlayerConfig.PlayerStandingHeight;
                camPivot.localPosition = new Vector3(camPivot.localPosition.x, GlobalPlayerConfig.PlayerCameraStandingHeight, camPivot.localPosition.z);
            }
            isSliding = isHeld && isSprinting && Grounded;
            Physics.SyncTransforms();
        }

        bool isSprinting;
        float targetFOV = 60;
        public void OnSprint(bool isHeld)
        {
            if (!isCrouching) // cant start sprinting while crouched
                isSprinting = isHeld;
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(PlayerMovementController))]
    public class PlayerMovementDebug : Editor
    {
        public void OnSceneGUI()
        {
            var t = (PlayerMovementController)target;
            Handles.color = Color.yellow;
            Handles.DrawWireArc(t.groundCheckPoint.position, Vector3.up, Vector3.forward,
                360, GlobalPlayerConfig.PlayerGroundCheckRadius);
            Handles.DrawWireArc(t.groundCheckPoint.position, Vector3.forward, Vector3.up,
                360, GlobalPlayerConfig.PlayerGroundCheckRadius);
            Handles.DrawWireArc(t.groundCheckPoint.position, Vector3.right, Vector3.forward,
                360, GlobalPlayerConfig.PlayerGroundCheckRadius);
            Handles.color = Color.blue;
            Handles.DrawLine(t.groundCheckPoint.position, t.groundCheckPoint.position -
                new Vector3(0, GlobalPlayerConfig.PlayerGroundCheckRadius, 0));
        }
    }
#endif
}