using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string gameSceneName = "WIP_Map";

    public void StartGame()
    {
        
        Time.timeScale = 1f;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMusic(AudioManager.Instance.gameMusic);

        SceneManager.LoadScene(gameSceneName);
    }

    public void ViewMetrics()
    {
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