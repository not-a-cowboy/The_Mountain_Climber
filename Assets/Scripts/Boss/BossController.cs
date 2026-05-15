using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("FallObs")]
    [SerializeField] private GameObject fallObsPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float[] laneXPositions = { -3f, 0f, 3f };
    [SerializeField] private float spawnHeightOffset = 1f;
    [SerializeField] private float spawnIntervalZ = 10f;

    [Header("Timing")]
    [SerializeField] private float xFollowDelay = 0.5f;
    [SerializeField] private float bossDuration = 30f;

    [Header("Powerup Bonus")]
    [SerializeField] private float powerUpTimeBonus = 5f;

    [Header("Activation")]
    [SerializeField] private float activationRange = 40f;

    [Header("Movement")]
    [SerializeField] private float playerSpeed;

    [Header("Defeat")]
    [SerializeField] private float fallSpeed = 9.81f;

    private bool activated = false;
    private float timer;
    private bool defeated = false;
    private bool defeatStarted = false;
    private float nextSpawnZ;
    private bool spawnOriginSet = false;

    private Queue<(float time, float x)> xHistory = new Queue<(float, float)>();

    private void Awake()
    {
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.freezeRotation = true;
        }

        timer = bossDuration;

        if (fallObsPrefab == null)
            Debug.LogError("[Boss] fallObsPrefab is NULL in Awake — assign it on the Boss prefab in the Inspector.");
    }

    private void Start()
    {
        if (PlayerController.Instance == null)
            Debug.LogError("[Boss] PlayerController.Instance is null in Start.");

        timer = bossDuration;

        if (!spawnOriginSet)
            Debug.LogWarning("[Boss] InitSpawnOrigin was never called — nextSpawnZ is 0. " +
                             "PlatformManager must call activeBoss.InitSpawnOrigin() after spawning the boss.");
    }

    public void InitSpawnOrigin()
    {
        nextSpawnZ = transform.position.z + spawnIntervalZ;
        spawnOriginSet = true;
        Debug.Log($"[Boss] InitSpawnOrigin called — bossStartZ: {transform.position.z:F1}, first nextSpawnZ: {nextSpawnZ:F1}");
    }


    private void Update()
    {
        if (defeated)
        {
            transform.position += Vector3.back * (fallSpeed * Time.deltaTime);
            return;
        }

        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
        if (PlayerController.Instance == null) return;

        Vector3 playerPos = PlayerController.Instance.RigidbodyPosition;

        if (!activated)
        {
            float distZ = Mathf.Abs(transform.position.z - playerPos.z);
            if (distZ > activationRange)
            {
                Debug.Log($"[Boss] Waiting to activate — player Z: {playerPos.z:F1}, " +
                          $"boss Z: {transform.position.z:F1}, distZ: {distZ:F1}, " +
                          $"activationRange: {activationRange:F1}");
                return;
            }

            activated = true;
            Debug.Log($"[Boss] Activated — boss Z: {transform.position.z:F1}, " +
                      $"player Z: {playerPos.z:F1}, nextSpawnZ: {nextSpawnZ:F1}");
        }

        playerSpeed = PlayerController.Instance.CurrentSpeed;
        transform.position += Vector3.forward * (playerSpeed * Time.deltaTime);

        xHistory.Enqueue((Time.time, playerPos.x));
        while (xHistory.Count > 1 && xHistory.Peek().time < Time.time - xFollowDelay)
            xHistory.Dequeue();

        float delayedX = xHistory.Count > 0 ? xHistory.Peek().x : transform.position.x;
        transform.position = new Vector3(delayedX, transform.position.y, transform.position.z);

        timer -= Time.deltaTime;
        if (timer <= 0f && !defeatStarted)
        {
            defeatStarted = true;
            StartCoroutine(DefeatSequence());
            return;
        }

        CheckAndSpawnFallObs();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            OnPowerUpAbsorbed();
            Destroy(other.gameObject);
        }
    }

    public void OnPowerUpAbsorbed()
    {
        timer += powerUpTimeBonus;
        Debug.Log($"[Boss] Absorbed powerup — timer now {timer:F1}s");
    }

    private void CheckAndSpawnFallObs()
    {
        if (fallObsPrefab == null)
        {
            Debug.LogError("[Boss] CheckAndSpawnFallObs — fallObsPrefab is NULL, cannot spawn.");
            return;
        }

        if (!spawnOriginSet)
        {
            Debug.LogWarning("[Boss] CheckAndSpawnFallObs — spawnOriginSet is false, skipping. " +
                             "InitSpawnOrigin was never called.");
            return;
        }

        Debug.Log($"[Boss] CheckAndSpawnFallObs — boss Z: {transform.position.z:F1}, nextSpawnZ: {nextSpawnZ:F1}");

        while (transform.position.z >= nextSpawnZ)
        {
            Debug.Log($"[Boss] Threshold reached — spawning at Z: {nextSpawnZ:F1}");
            SpawnFallObsAtZ(nextSpawnZ);
            nextSpawnZ += spawnIntervalZ;
            Debug.Log($"[Boss] Next spawn threshold: {nextSpawnZ:F1}");
        }
    }

    private void SpawnFallObsAtZ(float worldZ)
    {
        int count = Random.Range(1, 3);
        List<int> availableLanes = new List<int> { 0, 1, 2 };
        ShuffleList(availableLanes);

        Debug.Log($"[Boss] SpawnFallObsAtZ — worldZ: {worldZ:F1}, spawning {count} obstacle(s)");

        for (int i = 0; i < Mathf.Min(count, availableLanes.Count); i++)
        {
            Vector3 spawnPos = new Vector3(
                laneXPositions[availableLanes[i]],
                transform.position.y + spawnHeightOffset,
                worldZ
            );
            GameObject spawned = Instantiate(fallObsPrefab, spawnPos, Quaternion.identity);
            Debug.Log($"[Boss] Spawned FallObs '{spawned.name}' at {spawnPos}");
        }
    }

    private IEnumerator DefeatSequence()
    {
        defeated = true;
        Debug.Log("[Boss] DefeatSequence started.");

        if (GameManager.Instance != null)
            GameManager.Instance.NotifyBossDefeated();

        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    private void ShuffleList(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}