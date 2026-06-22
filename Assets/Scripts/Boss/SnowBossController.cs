using System;
using System.Collections;
using UnityEngine;

public class SnowBossController : MonoBehaviour
{
    public event Action OnDefeated;

    [Header("Bird Prefab")]
    [Tooltip("Prefab with BirdController attached.")]
    [SerializeField] private GameObject birdPrefab;

    [Header("Spawn Settings")]
    [Tooltip("Seconds between bird spawns (resets after player is dropped).")]
    [SerializeField] private float spawnInterval = 6f;
    [Tooltip("How far behind the player (in Z) birds spawn.")]
    [SerializeField] private float spawnBehindDistance = 20f;
    [Tooltip("Height above the platform the bird spawns at.")]
    [SerializeField] private float spawnHeight = 8f;

    [Header("Boss Timer")]
    [SerializeField] private float bossDuration = 45f;
    [SerializeField] private float powerUpTimeBonus = 5f;

    [Header("Lane Positions")]
    [SerializeField] private float[] laneXPositions = { -3f, 0f, 3f };

    private float bossTimer;
    private float spawnTimer;
    private bool playerGrabbed;
    private bool defeated;
    private bool defeatStarted;
    private bool spawnOriginSet;

    private void Awake()
    {
        bossTimer = bossDuration;
        spawnTimer = spawnInterval;
    }

    private void Start()
    {
        if (PlayerController.Instance == null)
            Debug.LogError("[SnowBoss] PlayerController.Instance is null.");
    }

    public void InitSpawnOrigin()
    {
        spawnOriginSet = true;
        spawnTimer = spawnInterval;
        Debug.Log("[SnowBoss] InitSpawnOrigin called — bird spawning active.");
    }

    private void Update()
    {
        if (defeated) return;
        if (!spawnOriginSet) return;
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
        if (PlayerController.Instance == null) return;

        if (!playerGrabbed)
        {
            bossTimer -= Time.deltaTime;
            if (bossTimer <= 0f && !defeatStarted)
            {
                defeatStarted = true;
                StartCoroutine(DefeatSequence());
                return;
            }

            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                SpawnBird();
                spawnTimer = spawnInterval;
            }
        }
    }

    private void SpawnBird()
    {
        if (birdPrefab == null)
        {
            Debug.LogError("[SnowBoss] birdPrefab is not assigned!");
            return;
        }

        if (PlayerController.Instance == null) return;

        Vector3 playerPos = PlayerController.Instance.RigidbodyPosition;
        int laneIndex = GetClosestLane(playerPos.x);
        float spawnX = laneXPositions[laneIndex];

        Vector3 spawnPos = new Vector3(
            spawnX,
            playerPos.y + spawnHeight,
            playerPos.z + spawnBehindDistance
        );

        GameObject birdGO = Instantiate(birdPrefab, spawnPos, Quaternion.identity);
        BirdController bird = birdGO.GetComponent<BirdController>();

        if (bird != null)
        {
            bird.Init(this);
            Debug.Log($"[SnowBoss] Bird spawned at {spawnPos}, lane {laneIndex}.");
        }
        else
        {
            Debug.LogError("[SnowBoss] birdPrefab is missing a BirdController component!");
        }
    }

    public void NotifyGrabStart()
    {
        playerGrabbed = true;
        Debug.Log("[SnowBoss] Grab started — timers paused.");
    }

    public void NotifyGrabEnd()
    {
        playerGrabbed = false;
        spawnTimer = spawnInterval;
        Debug.Log("[SnowBoss] Grab ended — timers resumed, spawn timer reset.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            bossTimer += powerUpTimeBonus;
            Destroy(other.gameObject);
            Debug.Log($"[SnowBoss] Absorbed power-up — boss timer now {bossTimer:F1}s");
        }
    }

    private IEnumerator DefeatSequence()
    {
        defeated = true;
        Debug.Log("[SnowBoss] Defeat sequence started.");

        if (GameManager.Instance != null)
            GameManager.Instance.NotifyBossDefeated();

        OnDefeated?.Invoke();

        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    private int GetClosestLane(float x)
    {
        int closest = 0;
        float minDist = Mathf.Infinity;
        for (int i = 0; i < laneXPositions.Length; i++)
        {
            float d = Mathf.Abs(laneXPositions[i] - x);
            if (d < minDist) { minDist = d; closest = i; }
        }
        return closest;
    }
}