using TMPro;
using UnityEngine;

public class ScoreView : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private string prefix = "Score";

    void Start()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.ScoreChanged += OnScoreChanged;
        OnScoreChanged(GameManager.Instance.Score);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.ScoreChanged -= OnScoreChanged;
    }

    private void OnScoreChanged(int score)
    {
        if (scoreText == null) return;

        scoreText.text = $"{prefix}: {score}";
    }
}
