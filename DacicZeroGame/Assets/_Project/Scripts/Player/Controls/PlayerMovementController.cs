using KBCore.Refs;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovementController : ValidatedMonoBehaviour
{
    #region Fields
    [SerializeField, Anywhere] InputReader inputReader;
    [SerializeField, Self] Rigidbody rb;
    [SerializeField, Anywhere] Transform groundCheckPoint;
    [field: SerializeField] public bool Grounded { get; protected set; }
    [SerializeField] bool onSlope;
    RaycastHit slopeHit;
    Vector2 inputVector;
    #endregion

    private void Awake()
    {
        rb.useGravity = false;
        rb.freezeRotation = true;
    }

    private void OnEnable()
    {
        inputReader.EnablePlayerActions();
        inputReader.Jump += OnJump;
        inputReader.Move += OnMove;
    }

    private void OnDisable()
    {
        inputReader.Move -= OnMove;
        inputReader.Jump -= OnJump;
        inputReader.DisablePlayerActions();
    }

    private void Update()
    {
        //ground and slope check
        GroundCheck();
    }
    void GroundCheck()
    {
        Grounded = Physics.CheckSphere(groundCheckPoint.position, transform.localScale.x / 2, GlobalPlayerConfig.GroundLayerMask);
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
        Physics.Raycast(groundCheckPoint.position, -groundCheckPoint.up, out slopeHit, transform.localScale.x / 2, GlobalPlayerConfig.GroundLayerMask);
        onSlope = slopeHit.normal != Vector3.up;
    }
    private void FixedUpdate()
    {
        if (!Grounded)
        {
            //apply gravity
            rb.velocity -= transform.up * GlobalPlayerConfig.Gravity * Time.fixedTime;
        }
        Vector3 dir = (transform.forward * inputVector.y + transform.right * inputVector.x).normalized * GlobalPlayerConfig.PlayerSpeed;
        if (onSlope)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0) + Vector3.ProjectOnPlane(dir, slopeHit.normal);
        }
        else
        {
            rb.velocity = new Vector3(dir.x, rb.velocity.y, dir.z);
        }
    }
    public void OnMove(Vector2 inputVector)
    {
        this.inputVector = inputVector;
    }

    public void OnJump()
    {
        if (Grounded)
        {
            rb.AddForce(transform.up * GlobalPlayerConfig.JumpForce, ForceMode.Impulse);
        }
    }
}