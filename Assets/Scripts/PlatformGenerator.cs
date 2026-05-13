
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    public Transform platformGenerationPoint;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            PlatformManager.Instance.SpawnNext(platformGenerationPoint);
    }
}