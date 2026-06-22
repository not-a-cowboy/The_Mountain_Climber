using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float Score { get; private set; }
    public float ScoreMultiplier { get; private set; } = 1f;
    public bool IsGameOver { get; private set; }

    public event System.Action OnGameOver;
    public event System.Action OnBossThreshold;
    public event System.Action OnBossWarning;
    public event System.Action OnBossDefeated;
    public event System.Action OnObstaclePassed;
    public event System.Action OnPickup1Activated;
    public event System.Action OnPickup2Activated;
    public event System.Action OnPickup3Activated;
    public event System.Action OnBoss1Spawned;
    public event System.Action OnBoss2Spawned;

    private bool bossThresholdFired = false;
    private bool bossWarningFired = false;

    private const float BossScoreThreshold = 80f;
    private const float BossWarningThreshold = 75f;

    [SerializeField] public int levelsCompleted = 0;

    private string playerName = "Player";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ResetGame();
    }

    private void Update()
    {
        if (IsGameOver) return;

        Score += Time.deltaTime * ScoreMultiplier;

        if (!bossThresholdFired && Score >= BossScoreThreshold)
        {
            bossThresholdFired = true;
            OnBossThreshold?.Invoke();
        }

        if (!bossWarningFired && Score >= BossWarningThreshold)
        {
            bossWarningFired = true;
            OnBossWarning?.Invoke();
        }
    }

    public string PlayerName
    {
        get => playerName;
        set => playerName = string.IsNullOrWhiteSpace(value) ? "Player" : value.Trim();
    }

    public void NotifyBossIncoming() => OnBossThreshold?.Invoke();
    public void NotifyBossWarning() => OnBossWarning?.Invoke();
    public void NotifyBossDefeated()
    {
        levelsCompleted++;
        OnBossDefeated?.Invoke();
    }

    public void NotifyObstaclePassed() => OnObstaclePassed?.Invoke();
    public void NotifyPickupActivated(int pickupNumber)
    {
        switch (pickupNumber)
        {
            case 1: OnPickup1Activated?.Invoke(); break;
            case 2: OnPickup2Activated?.Invoke(); break;
            case 3: OnPickup3Activated?.Invoke(); break;
            default: Debug.LogWarning($"[GameManager] Unknown pickup number: {pickupNumber}"); break;
        }
    }

    public void NotifyBossSpawned(int bossNumber)
    {
        switch (bossNumber)
        {
            case 1: OnBoss1Spawned?.Invoke(); break;
            case 2: OnBoss2Spawned?.Invoke(); break;
            default: Debug.LogWarning($"[GameManager] Unknown boss number: {bossNumber}"); break;
        }
    }

    public int GetNextLevelIndex()
    {
        if (levelsCompleted < 2)
        {
            return levelsCompleted;
        }

        return Random.Range(0, 2);
    }

    public void TriggerGameOver()
    {
        if (IsGameOver) return;

        IsGameOver = true;

        DatabaseManager.Instance?.SaveScore(PlayerName, Mathf.RoundToInt(Score), levelsCompleted);

        OnGameOver?.Invoke();
    }

    public void ActivateScoreMultiplier(float duration, float multiplier)
    {
        HUDManager.Instance?.TrackScoreTimer(duration);
        StartCoroutine(ApplyScoreMultiplier(duration, multiplier));
    }

    private IEnumerator ApplyScoreMultiplier(float duration, float multiplier)
    {
        float original = ScoreMultiplier;
        ScoreMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        ScoreMultiplier = original;
    }

    public void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    public void ResetGame()
    {
        Score = 0f;
        ScoreMultiplier = 1f;
        IsGameOver = false;
        bossThresholdFired = false;
        bossWarningFired = false;
        levelsCompleted = 0;
    }

    public void ResetPlayerName() => playerName = "Player";
}