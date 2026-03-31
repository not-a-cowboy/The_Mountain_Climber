using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    [Header("Lane Generation Points")]
    public Transform leftGenerationPoint;
    public Transform centerGenerationPoint;
    public Transform rightGenerationPoint;

    [Header("Platform Prefabs - Separate per Lane")]
    public GameObject[] leftPlatformPrefabs;
    public GameObject[] centerPlatformPrefabs;
    public GameObject[] rightPlatformPrefabs;

    [Header("Power-Ups (Drag your 3 PowerUp prefabs here)")]
    public GameObject[] powerUpPrefabs;

    [SerializeField] private float powerUpSpawnChance = 0.35f;
    [SerializeField] private float powerUpHeightAbovePlatform = 1.2f;

    [Header("Randomness")]
    [SerializeField] private int minPlatformsPerCall = 1;
    [SerializeField] private int maxPlatformsPerCall = 3;

    [SerializeField] private float segmentLength = 30f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (leftPlatformPrefabs == null || leftPlatformPrefabs.Length == 0 ||
            centerPlatformPrefabs == null || centerPlatformPrefabs.Length == 0 ||
            rightPlatformPrefabs == null || rightPlatformPrefabs.Length == 0)
        {
            Debug.LogError("One or more platform prefab arrays are empty!");
            return;
        }

        GameObject leftPlat = Instantiate(leftPlatformPrefabs[Random.Range(0, leftPlatformPrefabs.Length)],
                                          leftGenerationPoint.position, Quaternion.identity);

        GameObject centerPlat = Instantiate(centerPlatformPrefabs[Random.Range(0, centerPlatformPrefabs.Length)],
                                            centerGenerationPoint.position, Quaternion.identity);

        GameObject rightPlat = Instantiate(rightPlatformPrefabs[Random.Range(0, rightPlatformPrefabs.Length)],
                                           rightGenerationPoint.position, Quaternion.identity);

        if (powerUpPrefabs != null && powerUpPrefabs.Length > 0 && Random.value < powerUpSpawnChance)
        {
            GameObject[] platforms = { leftPlat, centerPlat, rightPlat };
            GameObject chosenPlatform = platforms[Random.Range(0, platforms.Length)];

            Vector3 spawnPos = chosenPlatform.transform.position + Vector3.up * powerUpHeightAbovePlatform;

            GameObject powerUpInstance = Instantiate(powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)],
                                                     spawnPos, Quaternion.identity);

            powerUpInstance.transform.SetParent(chosenPlatform.transform);
        }

        leftGenerationPoint.position += Vector3.forward * segmentLength;
        centerGenerationPoint.position += Vector3.forward * segmentLength;
        rightGenerationPoint.position += Vector3.forward * segmentLength;
    }
}