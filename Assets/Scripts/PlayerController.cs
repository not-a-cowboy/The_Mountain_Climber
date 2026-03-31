using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 5f;
    [Tooltip("How far the player moves sideways with one swipe/keypress")]
    public float horizontalSwipeDistance = 4f;
    public float jumpForce = 7f;
    public float laneLimit = 4f;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Jump Settings")]
    public float coyoteTime = 0.15f;   // Grace period to jump after leaving ground

    private Rigidbody rb;

    private bool isGrounded;
    private float coyoteTimeCounter;

    private float timeSinceStart;
    private float nextSpeedMilestone = 10f;

    // Triggers for snappy left/right movement (simulating swipe)
    private bool moveLeft;
    private bool moveRight;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Optional: Freeze rotation so player doesn't tumble
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        // --- Input ---
        if (Input.GetKeyDown(KeyCode.Space) && coyoteTimeCounter > 0f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            coyoteTimeCounter = 0f;   // Prevent double jumps
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            moveLeft = true;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            moveRight = true;
        }

        // Speed progression over time
        timeSinceStart += Time.deltaTime;
        if (timeSinceStart >= nextSpeedMilestone)
        {
            forwardSpeed += 0.5f;
            nextSpeedMilestone += 10f;
        }
    }

    void FixedUpdate()
    {
        // Ground check (must be in FixedUpdate for reliability)
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // Coyote time handling
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }

        // Continuous forward movement
        Vector3 forwardMovement = Vector3.forward * forwardSpeed * Time.fixedDeltaTime;

        // Snappy horizontal swipe
        Vector3 horizontalMovement = Vector3.zero;
        if (moveLeft)
        {
            horizontalMovement = Vector3.left * horizontalSwipeDistance;
            moveLeft = false;
        }
        else if (moveRight)
        {
            horizontalMovement = Vector3.right * horizontalSwipeDistance;
            moveRight = false;
        }

        // Apply movement
        rb.MovePosition(rb.position + forwardMovement + horizontalMovement);

        // Clamp to lane limits
        Vector3 clampedPos = rb.position;
        clampedPos.x = Mathf.Clamp(clampedPos.x, -laneLimit, laneLimit);
        rb.position = clampedPos;
    }
}