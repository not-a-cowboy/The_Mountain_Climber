using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Follow Settings")]
    [SerializeField] private float zOffset = -5f;
    [SerializeField] private float xFollowSpeed = 10f;

    private float fixedY;
    private bool isDead = false;

    private void Start()
    {
        fixedY = transform.position.y;
    }

    private void LateUpdate()
    {
        if (isDead || player == null) return;

        float targetX = Mathf.Lerp(transform.position.x, player.position.x, xFollowSpeed * Time.deltaTime);
        float targetZ = player.position.z + zOffset;

        transform.position = new Vector3(targetX, fixedY, targetZ);
    }

    public void TriggerDeathSequence()
    {
        isDead = true;
    }
}