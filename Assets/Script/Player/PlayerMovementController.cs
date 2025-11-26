using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

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
        rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Force);
    }

    private void MovePressed(Vector2 moveDir)
    {
        inputDir = moveDir;
    }

}
