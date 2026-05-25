using UnityEngine;

public class GameStateView : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameClearPanel;

    [Header("Result Views")]
    [SerializeField] private ResultSummaryView gameOverResultView;
    [SerializeField] private ResultSummaryView gameClearResultView;


    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StateChanged += OnStateChanged;
            OnStateChanged(GameManager.Instance.CurrentState);
        }
        else
        {
            HideAll();
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StateChanged -= OnStateChanged;
        }
    }

    public void ResumeGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
        }
    }

    private void OnStateChanged(GameState state)
    {
        HideAll();

        if (state == GameState.Paused && pausePanel != null)
        {
            pausePanel.SetActive(true);
            return;
        }

        if (state == GameState.GameOver && gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            gameOverResultView?.SetResult(GameManager.Instance);
            return;
        }

        if (state == GameState.GameClear && gameClearPanel != null)
        {
            gameClearPanel.SetActive(true);
            gameClearResultView?.SetResult(GameManager.Instance);
        }
    }

    private void HideAll()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (gameClearPanel != null) gameClearPanel.SetActive(false);
    }
}
