using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI score_text;
    public TextMeshProUGUI final_score_text;
    public GameObject game_over_panel;

    private float score;
    private bool is_game_over;

    void Update()
    {
        if (!is_game_over)
        {
            score += Time.deltaTime;
            score_text.text = "Score: " + Mathf.FloorToInt(score);
        }
    }

    public void PlayerDied()
    {
        is_game_over = true;
        game_over_panel.SetActive(true);
        final_score_text.text = "Final Score: " + Mathf.FloorToInt(score);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}