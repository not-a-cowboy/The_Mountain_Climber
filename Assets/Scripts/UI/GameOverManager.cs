using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreDisplay;

    public string mainMenuSceneName = "Start_Menu";
    public string gameSceneName = "WIP_Map";

    private void Start()
    {
        int finalScore = PlayerPrefs.GetInt("FinalScore", 0);

        if (scoreDisplay != null)
        {
            scoreDisplay.text = $"FINAL SCORE: {finalScore}";
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }
}