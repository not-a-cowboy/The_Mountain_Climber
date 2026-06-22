using SQLite4Unity3d;

[Table("HighScores")]
public class HighScoreEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Column("PlayerName"), NotNull]
    public string PlayerName { get; set; } = "Player";

    [Column("Score"), NotNull]
    public int Score { get; set; }

    [Column("LevelsCompleted")]
    public int LevelsCompleted { get; set; }

    [Column("Date")]
    public string Date { get; set; } = System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm");
}