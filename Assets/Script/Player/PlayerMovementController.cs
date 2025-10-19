using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;
    public float maxSlopeAngle;
    public RaycastHit slopeHit;

    public bool onSlope;
    public float angle;

    /*[Header("State Check")]
    public float playerHieght;
    public LayerMask IsGround;
    bool isGrounded;*/


    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Vector2 inputDir;
    Rigidbody rb;

    private void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onMovePressed += MovePressed;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onMovePressed -= MovePressed;

    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.linearDamping = groundDrag;
    }
    private void Update()
    {
        //HandleMovementInput();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * inputDir.y + orientation.right * inputDir.x;

        if (OnSlope())
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 10f, ForceMode.Force);
            if(rb.linearVelocity.magnitude > moveSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
            }
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }

        rb.useGravity = !OnSlope();
    }

    private void MovePressed(Vector2 moveDir)
    {
        inputDir = moveDir;
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, 1.8f))
        {
            angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            onSlope = angle < maxSlopeAngle && angle != 0;
            return onSlope;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Vector3 endPos = transform.position + Vector3.down * 1.8f;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, endPos);

        Gizmos.DrawSphere(slopeHit.point, 0.1f);
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

}
