using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreDisplay;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button saveNameButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button restartButton;

    private void Start()
    {
        int finalScore = PlayerPrefs.GetInt("FinalScore", 0);
        if (scoreDisplay != null)
            scoreDisplay.text = $"FINAL SCORE: {Mathf.RoundToInt(GameManager.Instance.Score)}";

        SetupInputField();
        SetupButtons();
    }

    private void SetupInputField()
    {
        if (nameInputField != null)
        {
            nameInputField.text = GameManager.Instance.PlayerName;
            nameInputField.onEndEdit.AddListener(OnNameChanged);
        }
    }

    private void SetupButtons()
    {
        if (saveNameButton != null)
            saveNameButton.onClick.AddListener(SaveCurrentScore);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
    }

    private void OnNameChanged(string newName)
    {
        GameManager.Instance.PlayerName = newName;
    }

    public void SaveCurrentScore()
    {
        if (string.IsNullOrWhiteSpace(nameInputField?.text))
            GameManager.Instance.PlayerName = "Player";

        int finalScore = Mathf.RoundToInt(GameManager.Instance.Score);

        DatabaseManager.Instance.SaveScore(
            GameManager.Instance.PlayerName,
            finalScore,
            GameManager.Instance.levelsCompleted
        );

        Debug.Log($"Score saved: {GameManager.Instance.PlayerName} - {finalScore}");

        if (saveNameButton != null)
            saveNameButton.interactable = false;
    }

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