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

        // Optional: Create index for faster sorting
        _db.CreateIndex("HighScores", "Score", true);
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

        _db.Insert(entry);

        // Keep only top 10
        var allScores = _db.Table<HighScoreEntry>()
                           .OrderByDescending(x => x.Score)
                           .ThenByDescending(x => x.Id)        // newer scores win ties
                           .ToList();

        if (allScores.Count > MaxHighScores)
        {
            var toDelete = allScores.Skip(MaxHighScores);
            foreach (var item in toDelete)
                _db.Delete<HighScoreEntry>(item.Id);
        }

        Debug.Log($"[Database] Score saved: {playerName} - {score}");
    }

    public List<HighScoreEntry> GetTopScores(int count = 10)
    {
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
}