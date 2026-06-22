using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SQLite4Unity3d;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    private SQLiteConnection _connection;
    private const string DB_FILE_NAME = "highscores.db";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        string dbPath = Path.Combine(Application.persistentDataPath, DB_FILE_NAME);

        try
        {
            _connection = new SQLiteConnection(dbPath);
            _connection.CreateTable<ScoreEntry>();
            Debug.Log($"[DatabaseManager] Connected to DB at: {dbPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[DatabaseManager] Failed to initialize database: {e.Message}");
        }
    }

    public void SaveScore(string playerName, int score, int levelsBeaten)
    {
        if (_connection == null)
        {
            Debug.LogError("[DatabaseManager] No active connection — cannot save score.");
            return;
        }

        var entry = new ScoreEntry
        {
            PlayerName = string.IsNullOrWhiteSpace(playerName) ? "Player" : playerName,
            Score = score,
            LevelsBeaten = levelsBeaten,
            DateRecorded = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        };

        try
        {
            _connection.Insert(entry);
            Debug.Log($"[DatabaseManager] Saved score: {entry.PlayerName} - {entry.Score}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[DatabaseManager] Failed to save score: {e.Message}");
        }
    }

    public List<ScoreEntry> GetTopScores(int count = 10)
    {
        if (_connection == null)
        {
            Debug.LogError("[DatabaseManager] No active connection — cannot fetch scores.");
            return new List<ScoreEntry>();
        }

        try
        {
            return _connection.Table<ScoreEntry>()
                               .OrderByDescending(s => s.Score)
                               .Take(count)
                               .ToList();
        }
        catch (Exception e)
        {
            Debug.LogError($"[DatabaseManager] Failed to fetch scores: {e.Message}");
            return new List<ScoreEntry>();
        }
    }

    public void ClearAllScores()
    {
        if (_connection == null) return;
        _connection.DeleteAll<ScoreEntry>();
        Debug.Log("[DatabaseManager] Cleared all scores.");
    }

    private void OnApplicationQuit()
    {
        _connection?.Close();
    }
}
public class ScoreEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string PlayerName { get; set; }
    public int Score { get; set; }
    public int LevelsBeaten { get; set; }
    public string DateRecorded { get; set; }
}