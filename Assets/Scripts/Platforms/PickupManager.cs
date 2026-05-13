using UnityEngine;

public enum PowerUpType
{
    HigherJump,
    Invulnerability,
    ScoreMultiplier
}

public class PowerUp : MonoBehaviour
{
    [Header("Power-Up Settings")]
    public PowerUpType type;

    public float duration = 5f;
    public float jumpMultiplier = 1.5f;
    public float scoreMultiplier = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (InventoryManager.Instance != null)
            InventoryManager.Instance.AddToSlot(type);

        Destroy(gameObject);
    }
}
