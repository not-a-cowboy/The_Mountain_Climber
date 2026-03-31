using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float Score { get; private set; }
    public bool IsGameOver { get; private set; }

    public event System.Action OnGameOver;

    private void Awake()
    {
        // Proper singleton with destruction of duplicates
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        ResetGame();
    }

    private void Update()
    {
        if (!IsGameOver)
        {
            Score += Time.deltaTime;
        }
    }

    public void PlayerDied()
    {
        if (IsGameOver) return;

        IsGameOver = true;
        OnGameOver?.Invoke();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ResetGame()
    {
        Score = 0f;
        IsGameOver = false;
    }
}