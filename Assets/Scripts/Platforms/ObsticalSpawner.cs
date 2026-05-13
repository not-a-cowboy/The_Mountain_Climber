using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Prefabs")]
    public GameObject jumpObsPrefab;
    public GameObject duckObsPrefab;
    public GameObject dodgeObsPrefab;

    [Header("Power-Up Prefabs")]
    public GameObject higherJumpPrefab;
    public GameObject invulnerabilityPrefab;
    public GameObject scoreMultiplierPrefab;

    [Header("Spawn Settings")]
    public float spawnHeightOffset = 1f;
    public float[] laneXPositions = { -3f, 0f, 3f };
    public float nearZOffset = 20f;
    public float centerZOffset = 0f;
    public float farZOffset = -20f;

    [Header("Spawn Chances")]
    [Range(0f, 1f)] public float powerUpSpawnChance = 0.3f;

    [Range(0f, 1f)] public float twoLaneBlockChance = 0.4f;  // chance 2 lanes are blocked
    [Range(0f, 1f)] public float threeLaneBlockChance = 0.15f; // chance all 3 lanes blocked (1 must be dodge)
    public bool enableMultiLaneBlocking = true;

    // Tracks obstacle types spawned per zone to cap dodge count
    private int dodgeCountThisSpawn = 0;

    public void SpawnObstacles()
    {
        dodgeCountThisSpawn = 0;
        SpawnZone(nearZOffset);
        SpawnZone(centerZOffset);
        SpawnZone(farZOffset);
    }

    float GetObstacleHeightOffset(GameObject prefab)
    {
        if (prefab == dodgeObsPrefab) return 1f;
        if (prefab == duckObsPrefab) return 2f;
        if (prefab == jumpObsPrefab) return 0.2f;
        return spawnHeightOffset; // fallback
    }


    void SpawnZone(float zOffset)
    {
        List<int> availableLanes = new List<int> { 0, 1, 2 };
        List<int> obstacleLanes = new List<int>();

        // --- Determine how many lanes to block ---
        int lanesToBlock = 1; // default: always at least 1 obstacle

        if (enableMultiLaneBlocking)
        {
            float roll = Random.value;
            if (roll < threeLaneBlockChance)
                lanesToBlock = 3;
            else if (roll < threeLaneBlockChance + twoLaneBlockChance)
                lanesToBlock = 2;
        }

        // --- Pick obstacle lanes ---
        List<int> shuffled = new List<int>(availableLanes);
        ShuffleList(shuffled);

        for (int i = 0; i < lanesToBlock; i++)
            obstacleLanes.Add(shuffled[i]);

        // Remove obstacle lanes from available so power-ups never overlap
        foreach (int lane in obstacleLanes)
            availableLanes.Remove(lane);

        // --- Spawn obstacles ---
        foreach (int lane in obstacleLanes)
        {
            GameObject prefab = GetObstaclePrefab();
            if (prefab != null)
            {
                Vector3 pos = new Vector3(
                    laneXPositions[lane],
                    transform.position.y + GetObstacleHeightOffset(prefab),
                    transform.position.z + zOffset
                );

                GameObject obs = Instantiate(prefab, pos, Quaternion.identity);
                obs.transform.SetParent(transform, worldPositionStays: true);
            }
        }


        // --- Spawn power-up in a remaining lane ---
        if (Random.value <= powerUpSpawnChance && availableLanes.Count > 0)
        {
            int powerUpLane = availableLanes[Random.Range(0, availableLanes.Count)];
            Vector3 powerUpPos = new Vector3(
                laneXPositions[powerUpLane],
                transform.position.y + spawnHeightOffset,
                transform.position.z + zOffset
            );

            GameObject powerUpPrefab = GetRandomPowerUpPrefab();
            if (powerUpPrefab != null)
            {
                GameObject pu = Instantiate(powerUpPrefab, powerUpPos, Quaternion.identity);
                pu.transform.SetParent(transform, worldPositionStays: true);
            }
        }
    }

    GameObject GetObstaclePrefab()
    {
        // Cap dodge obstacles at 2 per spawn cycle so there's always an escape
        List<int> options = new List<int> { 0, 1, 2 };
        if (dodgeCountThisSpawn >= 2)
            options.Remove(2); // remove dodge from pool

        int roll = options[Random.Range(0, options.Count)];

        if (roll == 2) dodgeCountThisSpawn++;

        return roll switch
        {
            0 => jumpObsPrefab,
            1 => duckObsPrefab,
            _ => dodgeObsPrefab,
        };
    }

    GameObject GetRandomPowerUpPrefab()
    {
        int roll = Random.Range(0, 3);
        return roll switch
        {
            0 => higherJumpPrefab,
            1 => invulnerabilityPrefab,
            _ => scoreMultiplierPrefab,
        };
    }

    void ShuffleList(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}