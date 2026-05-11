using UnityEngine;

public class PlatformSetDestroyer : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            PlatformManager.Instance.DestroyTail();
    }
}
