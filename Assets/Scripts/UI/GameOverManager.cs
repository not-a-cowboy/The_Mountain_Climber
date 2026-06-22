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
        Debug.Log("GameOverManager Start() called");

        // Score Display
        if (scoreDisplay != null)
        {
            int finalScore = Mathf.RoundToInt(GameManager.Instance.Score);
            scoreDisplay.text = $"FINAL SCORE: {finalScore}";
            Debug.Log($"Score display set to: {finalScore}");
        }
        else
        {
            Debug.LogError("Score Display reference is missing in GameOverManager!");
        }

        SetupInputField();
        SetupButtons();

        // Auto-save on Game Over
        AutoSaveCurrentScore();
    }

    private void SetupInputField()
    {
        if (nameInputField != null)
        {
            Debug.Log("InputField found and assigned");
            nameInputField.text = GameManager.Instance.PlayerName;
            nameInputField.onEndEdit.AddListener(OnNameChanged);

            // Force activation
            nameInputField.ActivateInputField();
        }
        else
        {
            Debug.LogError("Name InputField reference is NULL in GameOverManager!");
        }
    }

    private void SetupButtons()
    {
        if (saveNameButton != null)
        {
            saveNameButton.onClick.AddListener(SaveCurrentScore);
            Debug.Log("Save Button listener added");
        }
        else Debug.LogError("Save Button reference is missing!");

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
            Debug.Log("Main Menu Button listener added");
        }
        else Debug.LogError("Main Menu Button reference is missing!");

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
            Debug.Log("Restart Button listener added");
        }
        else Debug.LogError("Restart Button reference is missing!");
    }

    private void OnNameChanged(string newName)
    {
        GameManager.Instance.PlayerName = newName;
        Debug.Log($"Name changed to: {newName}");
    }

    private void AutoSaveCurrentScore()
    {
        string playerName = string.IsNullOrWhiteSpace(nameInputField?.text) ? "Player" : nameInputField.text.Trim();
        int finalScore = Mathf.RoundToInt(GameManager.Instance.Score);

        DatabaseManager.Instance.SaveScore(playerName, finalScore, GameManager.Instance.levelsCompleted);
        Debug.Log($"Auto-saved: {playerName} - {finalScore}");
    }

    public void SaveCurrentScore()
    {
        Debug.Log("SAVE button clicked!");

        string playerName = string.IsNullOrWhiteSpace(nameInputField?.text) ? "Player" : nameInputField.text.Trim();
        int finalScore = Mathf.RoundToInt(GameManager.Instance.Score);

        DatabaseManager.Instance.SaveScore(playerName, finalScore, GameManager.Instance.levelsCompleted);

        Debug.Log($"Score manually saved: {playerName} - {finalScore}");

        if (saveNameButton != null)
            saveNameButton.interactable = false;
    }

    public void GoToMainMenu()
    {
        Debug.Log("MAIN MENU button clicked!");
        Time.timeScale = 1f;
        SceneManager.LoadScene("Start_Menu");
    }

    public void RestartGame()
    {
        Debug.Log("RESTART button clicked!");
        Time.timeScale = 1f;
        SceneManager.LoadScene("WIP_Map");
    }
}