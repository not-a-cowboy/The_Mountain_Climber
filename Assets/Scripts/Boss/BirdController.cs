using System.Collections;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float hoverDuration = 0.5f;
    [SerializeField] private float carryDuration = 3.0f;
    [SerializeField] private float descentSpeed = 8f;
    [SerializeField] private float liftSpeed = 5f;
    [SerializeField] private float carryHeight = 6f;

    [Header("Approach")]
    [SerializeField] private float approachSpeedBonus = 10f;
    [SerializeField] private float overheadYOffset = 2.5f;
    private float someLaneTolerance = 1.0f;

    [Header("Exit")]
    [SerializeField] private float exitSpeed = 20f;

    private enum State { Approaching, Hovering, Grabbing, Carrying, Dropping }
    private State state = State.Approaching;

    private float stateTimer = 0f;
    private bool playerLocked = false;

    private Transform carryPoint;

    private SnowBossController boss;

    private PlayerController playerController;
    private PlayerHealth playerHealth;

    public void Init(SnowBossController ownerBoss)
    {
        boss = ownerBoss;

        GameObject cp = new GameObject("BirdCarryPoint");
        cp.transform.SetParent(transform);
        cp.transform.localPosition = Vector3.zero;
        carryPoint = cp.transform;

        if (PlayerController.Instance != null)
        {
            playerController = PlayerController.Instance;
            playerHealth = playerController.GetComponent<PlayerHealth>();
        }
        else
        {
            Debug.LogError("[Bird] PlayerController.Instance is null — bird cannot function.");
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            if (playerLocked) ReleasePlayer();
            Destroy(gameObject);
            return;
        }

        if (playerController == null) return;

        switch (state)
        {
            case State.Approaching: UpdateApproaching(); break;
            case State.Hovering: UpdateHovering(); break;
            case State.Grabbing: UpdateGrabbing(); break;
            case State.Carrying: UpdateCarrying(); break;
            case State.Dropping: UpdateDropping(); break;
        }
    }

    private void UpdateApproaching()
    {
        Vector3 playerPos = playerController.RigidbodyPosition;

        Vector3 target = new Vector3(
            transform.position.x,
            playerPos.y + overheadYOffset,
            playerPos.z
        );

        float playerSpeed = GetPlayerForwardSpeed();
        float moveSpeed = playerSpeed + approachSpeedBonus;

        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        float distZ = Mathf.Abs(transform.position.z - playerPos.z);
        float distY = Mathf.Abs(transform.position.y - (playerPos.y + overheadYOffset));
        if (distZ < 0.5f && distY < 0.5f)
        {
            state = State.Hovering;
            stateTimer = hoverDuration;
            Debug.Log("[Bird] - Hovering");
        }
    }

    private void UpdateHovering()
    {
        Vector3 playerPos = playerController.RigidbodyPosition;

        transform.position = new Vector3(
            transform.position.x,
            playerPos.y + overheadYOffset,
            playerPos.z
        );

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            state = State.Grabbing;
            Debug.Log("[Bird] - Grabbing");
        }
    }

    private void UpdateGrabbing()
    {
        Vector3 playerPos = playerController.RigidbodyPosition;

        transform.position = new Vector3(
            transform.position.x,
            transform.position.y - descentSpeed * Time.deltaTime,
            playerPos.z
        );

        float distX = Mathf.Abs(transform.position.x - playerPos.x);
        float distY = transform.position.y - playerPos.y;
        if (distX <= someLaneTolerance && distY <= 0.6f)
        {
            GrabPlayer();
        }
    }

    private void UpdateCarrying()
    {
        Vector3 playerPos = playerController.RigidbodyPosition;

        float targetY = playerPos.y + carryHeight;
        if (transform.position.y < targetY)
        {
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y + liftSpeed * Time.deltaTime,
                transform.position.z
            );
        }

        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            playerController.RigidbodyPosition.z
        );

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            state = State.Dropping;
            ReleasePlayer();
            Debug.Log("[Bird] - Dropping (player released)");
        }
    }

    private void UpdateDropping()
    {
        transform.position += (Vector3.forward + Vector3.up).normalized * exitSpeed * Time.deltaTime;

        stateTimer += Time.deltaTime;
        if (stateTimer > 3f)
            Destroy(gameObject);
    }

    private void GrabPlayer()
    {
        if (playerLocked) return;
        playerLocked = true;

        Debug.Log("[Bird] Grabbed player.");

        playerController.LockForBirdGrab(carryPoint);

        boss?.NotifyGrabStart();

        state = State.Carrying;
        stateTimer = carryDuration;
    }

    private void ReleasePlayer()
    {
        if (!playerLocked) return;
        playerLocked = false;

        Debug.Log("[Bird] Released player.");

        playerController.ReleaseFromBirdGrab();

        boss?.NotifyGrabEnd();

        stateTimer = 0f;
    }

    private float GetPlayerForwardSpeed()
    {
        Rigidbody rb = playerController.GetComponent<Rigidbody>();
        if (rb != null) return Mathf.Abs(rb.linearVelocity.z);
        return 10f;
    }
}