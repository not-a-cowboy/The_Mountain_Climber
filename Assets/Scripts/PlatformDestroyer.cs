using UnityEngine;
public class PlatformSetDestroyer : MonoBehaviour
{
    public GameObject platform;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Destroy(platform);
        }
    }
}

