using UnityEngine;
using SQLite4Unity3d;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    private SQLiteConnection _db;
    private const int MaxHighScores = 10;
    private const string DB_NAME = "highscores.db";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitDatabase();
    }

    private void InitDatabase()
    {
        string dbPath = Path.Combine(Application.persistentDataPath, DB_NAME);
        Debug.Log($"[Database] Path: {dbPath}");

        _db = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        _db.CreateTable<HighScoreEntry>();

        _db.CreateIndex("HighScores", "Score", false);
    }

    public void SaveScore(string playerName, int score, int levelsCompleted = 0)
    {
        if (string.IsNullOrWhiteSpace(playerName))
            playerName = "Player";

        playerName = playerName.Trim();

        var entry = new HighScoreEntry
        {
            PlayerName = playerName,
            Score = score,
            LevelsCompleted = levelsCompleted,
            Date = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        };

        try
        {
            _db.Insert(entry);
            Debug.Log($"[Database] Score saved: {playerName} - {score}");
        }
        catch (SQLiteException ex)
        {
            Debug.LogWarning($"[Database] Insert failed: {ex.Message}");
            if (ex.Result == SQLite3.Result.Constraint)
            {
                _db.InsertOrReplace(entry);
                Debug.Log("[Database] Used InsertOrReplace");
            }
            else
            {
                Debug.LogError($"[Database] Unexpected error: {ex}");
            }
        }

        TrimToTop10();
    }

    private void TrimToTop10()
    {
        var allScores = _db.Table<HighScoreEntry>()
                           .OrderByDescending(x => x.Score)
                           .ThenByDescending(x => x.Id)
                           .ToList();

        if (allScores.Count <= MaxHighScores)
            return;

        var toDelete = allScores.Skip(MaxHighScores).ToList();

        foreach (var item in toDelete)
        {
            _db.Delete<HighScoreEntry>(item.Id);
            Debug.Log($"[Database] Deleted old score: {item.PlayerName} - {item.Score}");
        }
        PrintAllScores();
    }

    public List<HighScoreEntry> GetTopScores(int count = 10)
    {
        PrintAllScores();
        return _db.Table<HighScoreEntry>()
                  .OrderByDescending(x => x.Score)
                  .ThenByDescending(x => x.Id)
                  .Take(count)
                  .ToList();
    }

    public void ClearAllScores()
    {
        _db.DeleteAll<HighScoreEntry>();
        Debug.Log("[Database] All scores cleared.");
    }

    private void OnDestroy()
    {
        _db?.Close();
    }

    public void PrintAllScores()
    {
        var all = _db.Table<HighScoreEntry>()
                     .OrderByDescending(x => x.Score)
                     .ThenByDescending(x => x.Id)
                     .ToList();

        Debug.Log($"Total scores in DB: {all.Count}");
        foreach (var s in all)
            Debug.Log($"#{s.Id} | {s.PlayerName} | {s.Score}");
    }
}