using UnityEngine;
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public int CurrentLevel { get; private set; } = 1;
    [Header("Level 2 Boss")]
    [Tooltip("Score points accumulated IN Level 2 before the snow boss spawns.")]
    [SerializeField] private float level2BossThreshold = 80f;
    private float level2ScoreAtEntry = 0f;
    private bool level2BossTriggered = false;
    public event System.Action OnLevel2Started;
    public event System.Action OnLevel2BossThreshold;
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void OnEnable()
    {
        // Subscribe after GameManager is guaranteed to exist (it uses the same
        // DontDestroyOnLoad pattern, so it will be alive by Start).
    }
    private void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnBossDefeated += HandleBossDefeated;
        else
            Debug.LogError("[LevelManager] GameManager.Instance is null in Start.");
    }
    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnBossDefeated -= HandleBossDefeated;
    }
    private void Update()
    {
        if (CurrentLevel != 2) return;
        if (level2BossTriggered) return;
        if (GameManager.Instance == null || GameManager.Instance.IsGameOver) return;
        float scoreInLevel2 = GameManager.Instance.Score - level2ScoreAtEntry;
        if (scoreInLevel2 >= level2BossThreshold)
        {
            level2BossTriggered = true;
            Debug.Log("[LevelManager] Level 2 boss threshold reached.");
            OnLevel2BossThreshold?.Invoke();

            if (GameManager.Instance != null)
                GameManager.Instance.NotifyBossWarning();
        }
    }
    private void HandleBossDefeated()
    {
        if (CurrentLevel == 1)
        {
            CurrentLevel = 2;
            level2ScoreAtEntry = GameManager.Instance != null ? GameManager.Instance.Score : 0f;
            Debug.Log($"[LevelManager] Entering Level 2. Score at entry: {level2ScoreAtEntry:F1}");
            OnLevel2Started?.Invoke();
        }
    }
}