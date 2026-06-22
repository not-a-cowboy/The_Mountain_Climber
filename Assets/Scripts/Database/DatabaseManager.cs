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
        Debug.Log($"[DB] Path: {dbPath}");

        _db = new SQLiteConnection(dbPath);
        _db.CreateTable<HighScoreEntry>();
    }

    public void SaveScore(string playerName, int score, int levelsCompleted = 0)
    {
        if (string.IsNullOrWhiteSpace(playerName)) playerName = "Player";

        var entry = new HighScoreEntry
        {
            PlayerName = playerName.Trim(),
            Score = score,
            LevelsCompleted = levelsCompleted
        };

        _db.Insert(entry);

        var all = _db.Table<HighScoreEntry>()
                     .OrderByDescending(x => x.Score)
                     .ThenByDescending(x => x.Id)
                     .ToList();

        if (all.Count > MaxHighScores)
        {
            var toDelete = all.Skip(MaxHighScores).ToList();
            foreach (var del in toDelete)
                _db.Delete<HighScoreEntry>(del.Id);
        }
    }

    public List<HighScoreEntry> GetTopScores(int count = 10)
    {
        return _db.Table<HighScoreEntry>()
                  .OrderByDescending(x => x.Score)
                  .ThenByDescending(x => x.Id)
                  .Take(count)
                  .ToList();
    }

    public void ClearAllScores() => _db.DeleteAll<HighScoreEntry>();

    private void OnDestroy()
    {
        _db?.Close();
    }
}