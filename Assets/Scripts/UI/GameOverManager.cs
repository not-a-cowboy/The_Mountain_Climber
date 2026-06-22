using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreDisplay;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button restartButton;

    private void Start()
    {
        Debug.Log($"GameOverManager Start() - Instance ID: {GetInstanceID()}");

        ShowFinalScore();
        SetupButtons();
    }

    private void ShowFinalScore()
    {
        if (scoreDisplay != null)
        {
            int finalScore = Mathf.RoundToInt(GameManager.Instance.Score);
            scoreDisplay.text = $"FINAL SCORE: {finalScore}";
        }
    }

    private void AutoSaveScore()
    {
        string playerName = GameManager.Instance.PlayerName;
        if (string.IsNullOrWhiteSpace(playerName))
            playerName = "Player";

        int finalScore = Mathf.RoundToInt(GameManager.Instance.Score);

        Debug.Log($"[AutoSave] About to save: {playerName} - {finalScore} | GameOverManager ID: {GetInstanceID()}");

        DatabaseManager.Instance.SaveScore(playerName, finalScore, GameManager.Instance.levelsCompleted);

        Debug.Log($"Auto-saved: {playerName} - {finalScore}");
    }

    private void SetupButtons()
    {
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        else
            Debug.LogError("Main Menu Button reference missing!");

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
        else
            Debug.LogError("Restart Button reference missing!");
    }

    public void GoToMainMenu()
    {
        Debug.Log("Going to Main Menu");
        Time.timeScale = 1f;
        SceneManager.LoadScene("Start_Menu");
    }

    public void RestartGame()
    {
        Debug.Log("Restarting Game");
        Time.timeScale = 1f;
        SceneManager.LoadScene("WIP_Map");
    }
}