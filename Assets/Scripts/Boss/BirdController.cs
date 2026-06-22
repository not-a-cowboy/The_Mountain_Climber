using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BirdController : MonoBehaviour
{
    public event Action OnBirdFinished;

    [Header("Timing")]
    [SerializeField] private float hoverDuration = 0.5f;
    [SerializeField] private float carryDuration = 3.0f;
    [SerializeField] private float descentSpeed = 8f;
    [SerializeField] private float liftSpeed = 5f;
    [SerializeField] private float carryHeight = 6f;

    [Header("Approach")]
    [SerializeField] private float approachSpeedBonus = 12f;
    [SerializeField] private float overheadYOffset = 2.5f;
    [SerializeField] private float laneTolerance = 1.0f;

    [Header("Exit")]
    [SerializeField] private float exitSpeed = 20f;

    [Header("Audio")]
    [SerializeField] private AudioSource birdAudio;
    [SerializeField] private AudioClip hawkSound;

    private enum State { Approaching, Hovering, Grabbing, Carrying, Dropping }
    private State state = State.Approaching;

    private float stateTimer = 0f;
    private bool playerLocked = false;
    private Transform carryPoint;
    private SnowBossController boss;
    private PlayerController playerController;
    private BirdAnimController birdAnim;

    private Rigidbody rb;
    private int currentLaneIndex;
    private bool hasFinished;

    public void Init(SnowBossController ownerBoss, int laneIndex)
    {
        boss = ownerBoss;
        currentLaneIndex = laneIndex;

        GameObject cp = new GameObject("BirdCarryPoint");
        cp.transform.SetParent(transform);
        cp.transform.localPosition = Vector3.zero;
        carryPoint = cp.transform;

        birdAnim = GetComponent<BirdAnimController>();
        if (birdAnim != null) birdAnim.PlayFlying();

        if (birdAudio != null && hawkSound != null)
            birdAudio.PlayOneShot(hawkSound);

        if (PlayerController.Instance != null)
        {
            playerController = PlayerController.Instance;
        }
        else
        {
            Debug.LogError("[Bird] PlayerController.Instance is null!");
        }

        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
    }

    private void FixedUpdate()
    {
        if (hasFinished || playerController == null) return;

        Vector3 pos = transform.position;
        pos.x = boss.GetLaneX(currentLaneIndex);
        transform.position = pos;

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

        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.fixedDeltaTime);

        if (transform.position.y > 1.5f)
        {
            Vector3 descendPos = transform.position;
            descendPos.y = Mathf.MoveTowards(descendPos.y, 1f, descentSpeed * 0.6f * Time.fixedDeltaTime);
            transform.position = descendPos;
        }

        float distZ = Mathf.Abs(transform.position.z - playerPos.z);
        float distY = Mathf.Abs(transform.position.y - (playerPos.y + overheadYOffset));

        if (distZ < 1.5f && distY < 2f)
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
            if (birdAnim != null) birdAnim.PlayDiving();
            Debug.Log("[Bird] - Grabbing");
        }
    }

    private void UpdateGrabbing()
    {
        Vector3 playerPos = playerController.RigidbodyPosition;
        Vector3 newPos = transform.position;
        newPos.z = playerPos.z;
        newPos.y -= descentSpeed * Time.fixedDeltaTime;

        transform.position = newPos;

        float distX = Mathf.Abs(transform.position.x - playerPos.x);
        float distY = transform.position.y - playerPos.y;

        if (distX <= laneTolerance && distY <= 0.8f)
        {
            GrabPlayer();
        }
    }

    private void UpdateCarrying()
    {
        Vector3 playerPos = playerController.RigidbodyPosition;
        float targetY = playerPos.y + carryHeight;

        Vector3 newPos = transform.position;
        newPos.y = Mathf.MoveTowards(newPos.y, targetY, liftSpeed * Time.fixedDeltaTime);
        newPos.z = playerPos.z;

        transform.position = newPos;

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            state = State.Dropping;
            ReleasePlayer();
            Debug.Log("[Bird] - Dropping player");
        }
    }

    private void UpdateDropping()
    {
        transform.position += (Vector3.forward + Vector3.up * 0.3f).normalized * exitSpeed * Time.deltaTime;
        stateTimer += Time.deltaTime;

        if (stateTimer > 3f)
            FinishBird();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (state == State.Grabbing && other.CompareTag("Player"))
        {
            GrabPlayer();
        }
    }

    private void GrabPlayer()
    {
        if (playerLocked) return;

        playerLocked = true;
        Debug.Log("[Bird] Player grabbed via trigger!");
        playerController.LockForBirdGrab(carryPoint);
        boss?.NotifyGrabStart();

        state = State.Carrying;
        stateTimer = carryDuration;

        if (birdAnim != null) birdAnim.PlayFlying();
    }

    private void ReleasePlayer()
    {
        if (!playerLocked) return;
        playerLocked = false;
        playerController.ReleaseFromBirdGrab();
        boss?.NotifyGrabEnd();
        stateTimer = 0f;
    }

    private float GetPlayerForwardSpeed()
    {
        if (playerController == null) return 10f;
        Rigidbody rbPlayer = playerController.GetComponent<Rigidbody>();
        return rbPlayer != null ? Mathf.Abs(rbPlayer.linearVelocity.z) : 10f;
    }

    private void FinishBird()
    {
        if (hasFinished) return;
        hasFinished = true;
        OnBirdFinished?.Invoke();
        Destroy(gameObject, 0.5f);
    }

    private void OnDestroy()
    {
        if (carryPoint != null)
            Destroy(carryPoint.gameObject);
    }
}