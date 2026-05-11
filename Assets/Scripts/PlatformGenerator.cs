
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            PlatformManager.Instance.SpawnNext();
    }
}