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
        if (contentParent == null)
        {
            Debug.LogError("Content Parent not assigned!");
            return;
        }

        if (rowPrefab == null)
        {
            Debug.LogError("Row Prefab not assigned!");
            return;
        }

        ClearRows();

        var scores = DatabaseManager.Instance.GetTopScores(10);

        Debug.Log($" Loaded {scores.Count} high scores");

        for (int i = 0; i < scores.Count; i++)
        {
            var entry = scores[i];
            GameObject row = Instantiate(rowPrefab, contentParent);

            var texts = row.GetComponentsInChildren<TextMeshProUGUI>(true);

            if (texts.Length >= 3)
            {
                texts[0].text = (i + 1) + ".";
                texts[1].text = entry.PlayerName;
                texts[2].text = entry.Score.ToString();
            }
        }
    }

    private void ClearRows()
    {
        if (contentParent == null) return;

        for (int i = contentParent.childCount - 1; i >= 0; i--)
        {
            var child = contentParent.GetChild(i);
            if (child != null && child.gameObject != rowPrefab)
            {
                Destroy(child.gameObject);
            }
        }
    }
}