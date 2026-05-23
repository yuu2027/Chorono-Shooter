using TMPro;
using UnityEngine;

public class ResultSummaryView : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text killCountText;
    [SerializeField] private TMP_Text playTimeText;

    // スコア、キル数、時間の表示
    public void SetResult(GameManager gameManager)
    {
        if (gameManager == null) return;

        if (scoreText != null)
        {
            scoreText.text = $"Score: {gameManager.Score}";
        }

        if (killCountText != null)
        {
            killCountText.text = $"Kills: {gameManager.killCount}";
        }

        if (playTimeText != null)
        {
            int totalSeconds = Mathf.FloorToInt(gameManager.PlayTime);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            playTimeText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }
}
