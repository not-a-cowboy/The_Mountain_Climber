using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Score UI")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver += ShowGameOverScreen;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver -= ShowGameOverScreen;
        }
    }

    private void Update()
    {
        // Only update live score when game is running
        if (GameManager.Instance != null && !GameManager.Instance.IsGameOver && scoreText != null)
        {
            scoreText.text = $"Score: {Mathf.FloorToInt(GameManager.Instance.Score)}";
        }
    }

    private void ShowGameOverScreen()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (finalScoreText != null && GameManager.Instance != null)
        {
            finalScoreText.text = $"Final Score: {Mathf.FloorToInt(GameManager.Instance.Score)}";
        }
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Call this from a Button OnClick in the Inspector
    public void RestartButtonPressed()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RestartGame();
    }
}