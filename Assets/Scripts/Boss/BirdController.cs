using System;
using System.Collections;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    public event Action OnBirdFinished;

    [Header("Approach")]
    [SerializeField] private float approachSpeedBonus = 10f;
    [SerializeField] private float overheadYOffset = 4f;

    [Header("Descent")]
    [SerializeField] private float descentSpeed = 8f;
    [SerializeField] private float grabTargetY = 1f;
    [SerializeField] private float hoverAtBottomDuration = 1.5f;

    [Header("Carry")]
    [SerializeField] private float liftSpeed = 6f;
    [SerializeField] private float carryTargetY = 3f;
    [SerializeField] private float carryDuration = 3f;

    [Header("Exit")]
    [SerializeField] private float exitSpeed = 20f;
    [SerializeField] private float exitDuration = 3f;

    private enum State { Approaching, Descending, HoveringAtBottom, Carrying, Exiting }
    private State state = State.Approaching;

    private float stateTimer;
    private bool playerGrabbed;

    private SnowBossController boss;
    private PlayerController playerController;

    public void Init(SnowBossController ownerBoss)
    {
        boss = ownerBoss;

        if (PlayerController.Instance != null)
            playerController = PlayerController.Instance;
        else
            Debug.LogError("[Bird] PlayerController.Instance is null.");
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            if (playerGrabbed) ReleasePlayer();
            Destroy(gameObject);
            return;
        }

        if (playerController == null) return;

        switch (state)
        {
            case State.Approaching: UpdateApproaching(); break;
            case State.Descending: UpdateDescending(); break;
            case State.HoveringAtBottom: UpdateHoveringAtBottom(); break;
            case State.Carrying: UpdateCarrying(); break;
            case State.Exiting: UpdateExiting(); break;
        }
    }

    private void UpdateApproaching()
    {
        Vector3 playerPos = playerController.RigidbodyPosition;

        float playerSpeed = GetPlayerForwardSpeed();
        float moveSpeed = playerSpeed + approachSpeedBonus;

        Vector3 target = new Vector3(
            transform.position.x,
            playerPos.y + overheadYOffset,
            playerPos.z
        );

        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        bool zAligned = Mathf.Abs(transform.position.z - playerPos.z) < 0.3f;
        bool yAligned = Mathf.Abs(transform.position.y - target.y) < 0.3f;

        if (zAligned && yAligned)
        {
            state = State.Descending;
            Debug.Log("[Bird] Approaching - Descending");
        }
    }

    private void UpdateDescending()
    {
        Vector3 playerPos = playerController.RigidbodyPosition;

        transform.position = new Vector3(
            transform.position.x,
            Mathf.MoveTowards(transform.position.y, playerPos.y + grabTargetY, descentSpeed * Time.deltaTime),
            playerPos.z
        );

        if (Mathf.Abs(transform.position.y - (playerPos.y + grabTargetY)) < 0.05f)
        {
            state = State.HoveringAtBottom;
            stateTimer = hoverAtBottomDuration;
            Debug.Log("[Bird] Reached bottom — hovering to check for grab.");
        }
    }

    private void UpdateHoveringAtBottom()
    {
        Vector3 playerPos = playerController.RigidbodyPosition;

        transform.position = new Vector3(
            transform.position.x,
            playerPos.y + grabTargetY,
            playerPos.z
        );

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f && !playerGrabbed)
        {
            Debug.Log("[Bird] Hover expired — missed player, exiting.");
            StartExiting();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (state != State.Descending && state != State.HoveringAtBottom) return;
        if (playerGrabbed) return;
        if (!other.CompareTag("Player")) return;

        GrabPlayer();
    }

    private void UpdateCarrying()
    {
        float targetY = playerController.RigidbodyPosition.y + carryTargetY;

        transform.position = new Vector3(
            transform.position.x,
            Mathf.MoveTowards(transform.position.y, targetY, liftSpeed * Time.deltaTime),
            playerController.RigidbodyPosition.z
        );

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            ReleasePlayer();
            Debug.Log("[Bird] Carry time up — dropping player.");
            StartExiting();
        }
    }

    private void UpdateExiting()
    {
        transform.position += Vector3.forward * exitSpeed * Time.deltaTime;

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            Debug.Log("[Bird] Exit done — destroying self.");
            OnBirdFinished?.Invoke();
            Destroy(gameObject);
        }
    }

    private void GrabPlayer()
    {
        playerGrabbed = true;
        Debug.Log("[Bird] Player grabbed via trigger.");

        playerController.LockForBirdGrab(transform);
        boss?.NotifyGrabStart();

        state = State.Carrying;
        stateTimer = carryDuration;
    }

    private void ReleasePlayer()
    {
        if (!playerGrabbed) return;
        playerGrabbed = false;

        playerController.ReleaseFromBirdGrab();
        boss?.NotifyGrabEnd();
    }

    private void StartExiting()
    {
        state = State.Exiting;
        stateTimer = exitDuration;
    }

    private float GetPlayerForwardSpeed()
    {
        Rigidbody rb = playerController.GetComponent<Rigidbody>();
        return rb != null ? Mathf.Abs(rb.linearVelocity.z) : 10f;
    }
}












