using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float baseForwardSpeed = 8f; 
    [SerializeField] private float horizontalLaneDistance = 3f;
    [SerializeField] private float laneSwitchDuration = 0.15f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float laneLimit = 3f;    

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Jump Settings")]
    [SerializeField] private float coyoteTime = 0.15f;

    private Rigidbody rb;
    private bool isGrounded;
    private float coyoteTimeCounter;

    private float targetXPosition = 0f;
    private float currentLaneSwitchTime = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // Prevent tumbling
    }

    private void Start()
    {
        targetXPosition = 0f; // Start in middle lane
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && coyoteTimeCounter > 0f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            coyoteTimeCounter = 0f;
        }

        // Lane switching (A/D or Left/Right arrows)
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            targetXPosition -= horizontalLaneDistance;
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            targetXPosition += horizontalLaneDistance;
        }

        targetXPosition = Mathf.Clamp(targetXPosition, -laneLimit, laneLimit);
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return; // Stop movement when game is over

        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // Coyote time
        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.fixedDeltaTime;

        // Get current forward speed from GameManager
        float currentForwardSpeed = baseForwardSpeed;
        if (GameManager.Instance != null)
        {
            currentForwardSpeed = baseForwardSpeed + (GameManager.Instance.Score * 0.05f); // Gradual increase
        }

        // Forward movement
        Vector3 forwardMovement = Vector3.forward * currentForwardSpeed * Time.fixedDeltaTime;

        // Smooth lane switching
        currentLaneSwitchTime += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(currentLaneSwitchTime / laneSwitchDuration);
        float newX = Mathf.Lerp(rb.position.x, targetXPosition, t);

        if (t >= 1f)
            currentLaneSwitchTime = 0f; // Reset timer after reaching target

        // Apply movement using MovePosition
        Vector3 newPosition = rb.position + forwardMovement;
        newPosition.x = newX;
        newPosition.y = rb.position.y;           

        rb.MovePosition(newPosition);
    }

    // Death handling - called from collision or trigger
    public void Die()
    {
        gameObject.SetActive(false);

        if (GameManager.Instance != null)
            GameManager.Instance.PlayerDied();
        else
            Debug.LogError("GameManager.Instance is missing!");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Die();
        }
    }
}