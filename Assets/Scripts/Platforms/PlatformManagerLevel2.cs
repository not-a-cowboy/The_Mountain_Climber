using UnityEngine;
using System.Collections.Generic;

public class PlatformManagerLevel2 : MonoBehaviour
{
    public static PlatformManagerLevel2 Instance { get; private set; }

    [Header("Snow Platform Prefabs")]
    [Tooltip("The very first snow platform — spawned once to bridge from Level 1.")]
    public GameObject snowPlatformStarterPrefab;
    [Tooltip("All subsequent Level 2 platforms.")]
    public GameObject snowPlatformRunnerPrefab;

    [Header("Snow Boss")]
    public GameObject snowBossPrefab;
    [Header("Level 2 Audio")]
    [SerializeField] private AudioSource blizzardAudio;

    private bool active = false;
    private bool starterSpawned = false;
    private bool snowBossPending = false;
    private bool snowBossAlive = false;
    private SnowBossController activeSnowBoss = null;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevel2Started += Activate;
            LevelManager.Instance.OnLevel2BossThreshold += HandleSnowBossThreshold;
        }
        else
        {
            Debug.LogError("[PlatformManagerLevel2] LevelManager.Instance is null.");
        }
    }

    private void OnDisable()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevel2Started -= Activate;
            LevelManager.Instance.OnLevel2BossThreshold -= HandleSnowBossThreshold;
        }
    }

    private void Activate()
    {
        active = true;
        if (blizzardAudio != null) blizzardAudio.Play();
        Debug.Log("[PlatformManagerLevel2] Activated — snow platforms will spawn next.");
    }

    public GameObject SpawnNext(Transform generationPoint)
    {
        if (!active) return null;

        GameObject prefab;

        if (!starterSpawned)
        {
            starterSpawned = true;
            prefab = snowPlatformStarterPrefab;
            Debug.Log("[PlatformManagerLevel2] Spawning SnowPlatformStarter.");
        }
        else if (snowBossPending)
        {
            snowBossPending = false;
            snowBossAlive = true;
            prefab = snowPlatformRunnerPrefab;
            Debug.Log("[PlatformManagerLevel2] Spawning Snow Boss platform.");
        }
        else
        {
            prefab = snowPlatformRunnerPrefab;
        }

        if (prefab == null)
        {
            Debug.LogWarning("[PlatformManagerLevel2] Prefab is null — check Inspector assignments.");
            return null;
        }

        GameObject platform = Object.Instantiate(prefab, generationPoint.position, Quaternion.identity);

        ObstacleSpawner spawner = platform.GetComponent<ObstacleSpawner>();
        if (spawner != null)
        {
            if (snowBossAlive && activeSnowBoss == null)
            {
                SpawnSnowBoss(generationPoint, spawner);
                spawner.SpawnBossObstacles();
            }
            else if (snowBossAlive)
            {
                spawner.SpawnBossObstacles();
            }
            else
            {
                spawner.SpawnObstacles();
            }
        }

        return platform;
    }

    private void HandleSnowBossThreshold()
    {
        snowBossPending = true;
        Debug.Log("[PlatformManagerLevel2] Snow boss platform pending.");
    }

    private void SpawnSnowBoss(Transform generationPoint, ObstacleSpawner spawner)
    {
        if (snowBossPrefab == null)
        {
            Debug.LogWarning("[PlatformManagerLevel2] snowBossPrefab not assigned!");
            return;
        }

        float farZ = spawner != null ? spawner.farZOffset : -20f;
        float height = spawner != null ? spawner.spawnHeightOffset : 1f;

        Vector3 bossPos = new Vector3(
            generationPoint.position.x,
            generationPoint.position.y + height,
            generationPoint.position.z + farZ
        );

        GameObject bossGO = Object.Instantiate(snowBossPrefab, bossPos, Quaternion.identity);
        activeSnowBoss = bossGO.GetComponent<SnowBossController>();
        activeSnowBoss?.InitSpawnOrigin();

        if (activeSnowBoss != null)
            activeSnowBoss.OnDefeated += HandleSnowBossDefeated;

        Debug.Log($"[PlatformManagerLevel2] Snow boss spawned at {bossPos}.");
    }

    private void HandleSnowBossDefeated()
    {
        snowBossAlive = false;
        activeSnowBoss = null;
        Debug.Log("[PlatformManagerLevel2] Snow boss defeated — normal snow spawning resumes.");
    }

    public bool IsActive => active;
}