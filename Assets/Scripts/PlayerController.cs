using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float baseForwardSpeed = 8f;
    [SerializeField] private float horizontalLaneDistance = 3f;
    [SerializeField] private float laneSwitchDuration = 0.15f;
    [SerializeField] private float laneLimit = 3f;

    [Header("Jump Settings")]
    [SerializeField] private float baseJumpForce = 8f;
    [SerializeField] private float coyoteTime = 0.15f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;
    private bool isGrounded;
    private float coyoteTimeCounter;
    private float targetXPosition = 0f;
    private float currentLaneSwitchTime = 0f;

    // Power-up variables
    private float currentJumpForce;
    private bool isInvulnerable = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        currentJumpForce = baseJumpForce;
    }

    private void Start()
    {
        targetXPosition = 0f;
    }

    private void Update()
    {
        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && coyoteTimeCounter > 0f)
        {
            rb.AddForce(Vector3.up * currentJumpForce, ForceMode.Impulse);
            coyoteTimeCounter = 0f;
        }

        // Lane switching
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            targetXPosition -= horizontalLaneDistance;
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            targetXPosition += horizontalLaneDistance;

        targetXPosition = Mathf.Clamp(targetXPosition, -laneLimit, laneLimit);
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return;

        // Ground check + coyote time
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.fixedDeltaTime;

        // Forward movement
        float currentForwardSpeed = baseForwardSpeed;
        if (GameManager.Instance != null)
            currentForwardSpeed = baseJumpForce + (GameManager.Instance.Score * 0.05f);

        Vector3 forwardMovement = Vector3.forward * currentForwardSpeed * Time.fixedDeltaTime;

        currentLaneSwitchTime += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(currentLaneSwitchTime / laneSwitchDuration);
        float newX = Mathf.Lerp(rb.position.x, targetXPosition, t);
        if (t >= 1f) currentLaneSwitchTime = 0f;

        Vector3 newPosition = rb.position + forwardMovement;
        newPosition.x = newX;
        newPosition.y = rb.position.y;
        rb.MovePosition(newPosition);
    }

    public void ActivatePowerUp(PowerUpType type, float duration, float jumpMult, float scoreMult)
    {
        switch (type)
        {
            case PowerUpType.HigherJump:
                StartCoroutine(ApplyHigherJump(duration, jumpMult));
                break;

            case PowerUpType.Invulnerability:
                StartCoroutine(ApplyInvulnerability(duration));
                break;

            case PowerUpType.ScoreMultiplier:
                if (GameManager.Instance != null)
                    GameManager.Instance.ActivateScoreMultiplier(duration, scoreMult);
                break;
        }
    }

    private IEnumerator ApplyHigherJump(float duration, float multiplier)
    {
        float original = currentJumpForce;
        currentJumpForce = baseJumpForce * multiplier;
        yield return new WaitForSeconds(duration);
        currentJumpForce = original;
    }

    private IEnumerator ApplyInvulnerability(float duration)
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(duration);
        isInvulnerable = false;
    }

    // Death only happens if NOT invulnerable
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") && !isInvulnerable)
        {
            Die();
        }
    }

    public void Die()
    {
        gameObject.SetActive(false);
        if (GameManager.Instance != null)
            GameManager.Instance.PlayerDied();
    }
}