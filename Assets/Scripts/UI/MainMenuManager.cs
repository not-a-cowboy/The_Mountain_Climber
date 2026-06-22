using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
<<<<<<< Updated upstream
=======
    public static MainMenuManager Instance { get; private set; }

    [Header("Scene Settings")]
>>>>>>> Stashed changes
    public string gameSceneName = "WIP_Map";

    [Header("Leaderboard")]
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private LeaderboardUI leaderboardUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);
    }

    public void StartGame()
    {
<<<<<<< Updated upstream
        
=======
>>>>>>> Stashed changes
        Time.timeScale = 1f;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMusic(AudioManager.Instance.gameMusic);

        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenLeaderboard()
    {
<<<<<<< Updated upstream
        Debug.Log("Loading High Scores / Metrics...");
=======
        if (leaderboardPanel == null || leaderboardUI == null)
        {
            Debug.LogError("[MainMenuManager] Leaderboard references are not assigned in the Inspector!");
            return;
        }

        leaderboardPanel.SetActive(true);
        leaderboardUI.ShowLeaderboard();
    }

    public void CloseLeaderboard()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);
>>>>>>> Stashed changes
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