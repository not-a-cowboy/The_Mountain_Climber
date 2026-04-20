using UnityEngine;

public enum PowerUpType
{
    HigherJump,
    Invulnerability,
    ScoreMultiplier
}

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Power-Up Settings")]
    public PowerUpType type;
    public float duration = 5f;
    public float jumpMultiplier = 1.5f;
    public float scoreMultiplier = 2f;

    // Optional: visual feedback when collected
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ActivatePowerUp(type, duration, jumpMultiplier, scoreMultiplier);
                gameObject.SetActive(false);
            }
        }
    }
}
