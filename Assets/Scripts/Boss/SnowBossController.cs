using System;
using System.Collections;
using UnityEngine;

public class SnowBossController : MonoBehaviour
{
    public event Action OnDefeated;

    [Header("Bird Prefab")]
    [SerializeField] private GameObject birdPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnBehindDistance = 25f;
    [SerializeField] private float spawnHeight = 8f;

    [Header("Boss Timer")]
    [SerializeField] private float bossDuration = 45f;

    [Header("Lane Positions")]
    [SerializeField] private float[] laneXPositions = { -3f, 0f, 3f };

    private float bossTimer;
    private bool defeated;
    private bool defeatStarted;
    private bool active;

    private void Awake()
    {
        bossTimer = bossDuration;
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
        if (GameManager.Instance?.IsGameOver == true) return;

        bossTimer -= Time.deltaTime;

        if (bossTimer <= 0f && !defeatStarted)
        {
            defeatStarted = true;
            StartCoroutine(DefeatSequence());
        }
    }

    private void SpawnBird()
    {
        if (defeated || !active || birdPrefab == null) return;

        Vector3 playerPos = PlayerController.Instance.RigidbodyPosition;
        int laneIndex = GetClosestLane(playerPos.x);
        float spawnX = laneXPositions[laneIndex];

        Vector3 spawnPos = new Vector3(spawnX, playerPos.y + spawnHeight, playerPos.z - spawnBehindDistance);

        GameObject birdGO = Instantiate(birdPrefab, spawnPos, Quaternion.LookRotation(Vector3.forward));
        BirdController bird = birdGO.GetComponent<BirdController>();

        if (bird != null)
        {
            bird.Init(this, laneIndex);
            bird.OnBirdFinished += OnBirdFinished;
            Debug.Log($"[SnowBoss] Bird spawned in lane {laneIndex} at X={spawnX}");
        }
        else
        {
            Debug.LogError("[SnowBoss] Bird prefab missing BirdController!");
            Destroy(birdGO);
        }
    }

    private void OnBirdFinished()
    {
        if (!defeated && !defeatStarted)
        {
            SpawnBird();
        }
    }

    private IEnumerator DefeatSequence()
    {
        defeated = true;
        Debug.Log("[SnowBoss] Timer expired - defeating boss");
        GameManager.Instance?.NotifyBossDefeated();
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
            if (d < minDist)
            {
                minDist = d;
                closest = i;
            }
        }
        return closest;
    }

    // Public getter for lane positions
    public float GetLaneX(int index)
    {
        if (index >= 0 && index < laneXPositions.Length)
            return laneXPositions[index];
        return 0f;
    }

    // Added missing methods
    public void NotifyGrabStart() => Debug.Log("[SnowBoss] Player grabbed.");
    public void NotifyGrabEnd() => Debug.Log("[SnowBoss] Player released.");
}