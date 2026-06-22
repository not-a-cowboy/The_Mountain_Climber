using System;
using System.Collections;
using UnityEngine;
public class SnowBossController : MonoBehaviour
{
    public event Action OnDefeated;
    [Header("Bird Prefab")]
    [Tooltip("Prefab with BirdController attached and an isTrigger collider.")]
    [SerializeField] private GameObject birdPrefab;
    [Header("Spawn Settings")]
    [Tooltip("How far behind the player (in Z) each bird spawns.")]
    [SerializeField] private float spawnBehindDistance = 20f;
    [Tooltip("Height above the player the bird spawns at.")]
    [SerializeField] private float spawnHeight = 8f;
    [Header("Boss Timer")]
    [SerializeField] private float bossDuration = 45f;
    [Header("Lane Positions")]
    [SerializeField] private float[] laneXPositions = { -3f, 0f, 3f };
    private float bossTimer;
    private bool defeated;
    private bool defeatStarted;
    private bool active;
    private bool birdAlive;
    private void Awake()
    {
        bossTimer = bossDuration;
    }
    private void Start()
    {
        if (PlayerController.Instance == null)
            Debug.LogError("[SnowBoss] PlayerController.Instance is null.");
    }
    public void InitSpawnOrigin()
    {
        active = true;
        SpawnBird();
        Debug.Log("[SnowBoss] Encounter started.");
    }
    private void Update()
    {
        if (!active || defeated) return;
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
        if (PlayerController.Instance == null) return;
        bossTimer -= Time.deltaTime;
        if (bossTimer <= 0f && !defeatStarted)
        {
            defeatStarted = true;
            StartCoroutine(DefeatSequence());
        }
    }
    private void SpawnBird()
    {
        if (defeated || !active) return;
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
            playerPos.z - spawnBehindDistance
        );
        GameObject birdGO = Instantiate(birdPrefab, spawnPos, Quaternion.identity);
        BirdController bird = birdGO.GetComponent<BirdController>();
        if (bird != null)
        {
            bird.Init(this);
            bird.OnBirdFinished += OnBirdFinished;
            birdAlive = true;
            Debug.Log($"[SnowBoss] Bird spawned at {spawnPos}, lane {laneIndex}.");
        }
        else
        {
            Debug.LogError("[SnowBoss] birdPrefab is missing a BirdController component!");
        }
    }
    private void OnBirdFinished()
    {
        birdAlive = false;
        if (!defeated && !defeatStarted)
        {
            Debug.Log("[SnowBoss] Bird done — spawning next.");
            SpawnBird();
        }
    }
    public void NotifyGrabStart()
    {
        Debug.Log("[SnowBoss] Player grabbed.");
    }
    public void NotifyGrabEnd()
    {
        Debug.Log("[SnowBoss] Player released.");
    }
    private IEnumerator DefeatSequence()
    {
        defeated = true;
        Debug.Log("[SnowBoss] Boss timer expired — defeat sequence.");
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