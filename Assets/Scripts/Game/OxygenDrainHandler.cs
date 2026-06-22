using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
public class OxygenDrainHandler : MonoBehaviour
{
    [Header("Drain Settings")]
    [Tooltip("HP drained per second while in Level 2. Default 1 = 1% of 100 max HP.")]
    [SerializeField] private float drainPerSecond = 1f;

    private PlayerHealth playerHealth;
    private bool draining = false;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Start()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevel2Started += StartDrain;
        }
        else
        {
            Debug.LogError("[OxygenDrainHandler] LevelManager.Instance is null in Start. " +
                           "Make sure LevelManager is in the scene.");
        }
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance != null)
            LevelManager.Instance.OnLevel2Started -= StartDrain;
    }

    private void Update()
    {
        if (!draining) return;
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        playerHealth.DrainHP(drainPerSecond * Time.deltaTime);
    }

    private void StartDrain()
    {
        draining = true;
        Debug.Log("[OxygenDrainHandler] Oxygen drain started.");
    }

    public void PauseDrain(float seconds) => StartCoroutine(PauseDrainCoroutine(seconds));

    private System.Collections.IEnumerator PauseDrainCoroutine(float seconds)
    {
        draining = false;
        yield return new WaitForSeconds(seconds);
        if (LevelManager.Instance != null && LevelManager.Instance.CurrentLevel == 2)
            draining = true;
    }
}