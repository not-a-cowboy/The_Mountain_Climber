using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerInputActions playerInput;
    [SerializeField] private float speed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float forwardSpeed;
    [SerializeField] private float laneSlideSpeed = 10f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;
    private InputAction moveAction;
    private bool isGrounded;

    private int currentLane = 0;
    private float targetX = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = new PlayerInputActions();
    }

    private void OnEnable()
    {
        playerInput.Player.Jump.performed += OnJump;
        playerInput.Player.Move.performed += OnMove;
        moveAction = playerInput.Player.Move;
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Player.Jump.performed -= OnJump;
        playerInput.Player.Move.performed -= OnMove;
        playerInput.Disable();
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (!isGrounded) return;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, 0f); // clear Y and Z before launch
        rb.AddForce(new Vector3(0f, jumpSpeed * 0.4f, jumpSpeed), ForceMode.Impulse);
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();

        if (input.x > 0.5f && currentLane < 1)
        {
            currentLane++;
            targetX = currentLane * 3f;
        }
        else if (input.x < -0.5f && currentLane > -1)
        {
            currentLane--;
            targetX = currentLane * 3f;
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        float newX = Mathf.MoveTowards(rb.position.x, targetX, laneSlideSpeed * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector3(
            (newX - rb.position.x) / Time.fixedDeltaTime,
            rb.linearVelocity.y,
            forwardSpeed
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}