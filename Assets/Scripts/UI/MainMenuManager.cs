using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Make sure the string matches your actual game scene name exactly!
    public string gameSceneName = "WIP_Map";

    public void StartGame()
    {
        // Reset time scale just in case the player quit while paused previously
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void ViewMetrics()
    {
        // For now, this just logs. You will connect this to your Database UI later.
        Debug.Log("Loading High Scores / Metrics...");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}