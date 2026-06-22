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
        CheckButtonReferences();
    }

    private void CheckButtonReferences()
    {
        if (mainMenuButton == null)
            Debug.LogError("Main Menu Button reference is NOT assigned in Inspector!");
        else
            Debug.Log("Main Menu Button reference is good.");

        if (restartButton == null)
            Debug.LogError("Restart Button reference is NOT assigned in Inspector!");
        else
            Debug.Log("Restart Button reference is good.");
    }

    private void ShowFinalScore()
    {
        if (scoreDisplay != null)
        {
            int finalScore = Mathf.RoundToInt(GameManager.Instance.Score);
            scoreDisplay.text = $"FINAL SCORE: {finalScore}";
        }
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