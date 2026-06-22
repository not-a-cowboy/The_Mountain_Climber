using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LeaderboardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject rowPrefab;

    public void ShowLeaderboard()
    {
        if (contentParent == null || rowPrefab == null)
        {
            Debug.LogError("LeaderboardUI: Missing references!");
            return;
        }

        ClearRows();

        var scores = DatabaseManager.Instance.GetTopScores(10);

        for (int i = 0; i < scores.Count; i++)
        {
            var entry = scores[i];
            GameObject row = Instantiate(rowPrefab, contentParent);

            var texts = row.GetComponentsInChildren<TextMeshProUGUI>(true);

            if (texts.Length >= 3)
            {
                texts[0].text = $"{i + 1}.";
                texts[1].text = entry.PlayerName;
                texts[2].text = entry.Score.ToString("N0");
            }
        }
    }

    private void ClearRows()
    {
        for (int i = contentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(contentParent.GetChild(i).gameObject);
        }
    }
}