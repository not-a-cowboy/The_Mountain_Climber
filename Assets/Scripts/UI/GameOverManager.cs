using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreDisplay;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button saveNameButton;
    [SerializeField] private Button leaderboardButton;

    private string currentPlayerName = "Player";

    private void Start()
    {
        int finalScore = PlayerPrefs.GetInt("FinalScore", 0);
        if (scoreDisplay != null)
            scoreDisplay.text = $"FINAL SCORE: {finalScore}";

        if (nameInputField != null)
        {
            nameInputField.text = currentPlayerName;
            nameInputField.onEndEdit.AddListener(OnNameChanged);
        }

        if (saveNameButton != null)
            saveNameButton.onClick.AddListener(SaveCurrentScore);

        if (leaderboardButton != null)
            leaderboardButton.onClick.AddListener(ShowLeaderboard);
    }

    private void OnNameChanged(string newName)
    {
        currentPlayerName = newName;
    }

    public void SaveCurrentScore()
    {
        int finalScore = Mathf.RoundToInt(GameManager.Instance.Score);
        DatabaseManager.Instance.SaveScore(currentPlayerName, finalScore, GameManager.Instance.levelsCompleted);
        Debug.Log($"Score saved for {currentPlayerName}: {finalScore}");
    }

    public void ShowLeaderboard() => MainMenuManager.Instance?.OpenLeaderboard();
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Start_Menu");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("WIP_Map");
    }
}