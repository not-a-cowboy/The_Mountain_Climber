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

    [Header("Randomness")]
    [SerializeField] private int minPlatformsPerCall = 1;
    [SerializeField] private int maxPlatformsPerCall = 3;

    // Called by the single trigger when the player passes through
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("SpawnNextSegment called!");

        // Safety checks
        if (leftPlatformPrefabs == null || leftPlatformPrefabs.Length == 0)
        {
            Debug.LogError("Left platform prefabs array is empty!");
            return;
        }
        if (centerPlatformPrefabs == null || centerPlatformPrefabs.Length == 0)
        {
            Debug.LogError("Center platform prefabs array is empty!");
            return;
        }
        if (rightPlatformPrefabs == null || rightPlatformPrefabs.Length == 0)
        {
            Debug.LogError("Right platform prefabs array is empty!");
            return;
        }

        // Choose a random prefab for each lane independently
        int leftIndex = Random.Range(0, leftPlatformPrefabs.Length);
        int centerIndex = Random.Range(0, centerPlatformPrefabs.Length);
        int rightIndex = Random.Range(0, rightPlatformPrefabs.Length);

        GameObject leftPrefab = leftPlatformPrefabs[leftIndex];
        GameObject centerPrefab = centerPlatformPrefabs[centerIndex];
        GameObject rightPrefab = rightPlatformPrefabs[rightIndex];

        // Spawn all three at exactly the same moment
        Instantiate(leftPrefab, leftGenerationPoint.position, Quaternion.identity);
        Instantiate(centerPrefab, centerGenerationPoint.position, Quaternion.identity);
        Instantiate(rightPrefab, rightGenerationPoint.position, Quaternion.identity);
    }
}