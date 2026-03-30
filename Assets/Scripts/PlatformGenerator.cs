using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public GameObject[] platforms;
    public Transform platformGenerationPoint;

    //random number of platforms

    [SerializeField] public int minRandomPlatform;
    [SerializeField] public int maxRandomPlatform;


    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;

        // Critical safety checks
        if (platforms == null || platforms.Length == 0)
        {
            Debug.LogError("Platforms array is empty or not assigned in the Inspector!");
            return;
        }

        // Simple and reliable random index (0 to Length-1)
        int randomIndex = Random.Range(0, platforms.Length);

        GameObject platform = Instantiate(platforms[randomIndex],
                                          platformGenerationPoint.position,
                                          Quaternion.identity) as GameObject;

        platform.name = "Platform";
    }
}
