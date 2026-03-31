using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    private GameManager game_manager;

    void Start()
    {
        game_manager = FindObjectOfType<GameManager>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            game_manager.PlayerDied();
            gameObject.SetActive(false);
        }
    }
}