using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LeaderboardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject rowPrefab;

    public void ShowLeaderboard()
    {
        // Clear old rows
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        List<HighScoreEntry> scores = DatabaseManager.Instance.GetTopScores(10);

        for (int i = 0; i < scores.Count; i++)
        {
            HighScoreEntry entry = scores[i];
            GameObject row = Instantiate(rowPrefab, contentParent);

            TextMeshProUGUI[] texts = row.GetComponentsInChildren<TextMeshProUGUI>();

            if (texts.Length >= 3)
            {
                texts[0].text = (i + 1).ToString() + ".";
                texts[1].text = entry.PlayerName;
                texts[2].text = entry.Score.ToString();
            }
        }
    }
}