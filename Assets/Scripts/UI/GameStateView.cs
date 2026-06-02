using System.Collections;
using UnityEngine;

public class GameStateView : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;

    [Header("Scene Loader")]
    [SerializeField] private SceneLoader sceneLoader;

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
        if (GameManager.Instance == null) return;

        AudioManager.Instance?.PlaySe(SeId.Button);
        GameManager.Instance.ResumeGame();
    }

    private void OnStateChanged(GameState state)
    {
        HideAll();

        if (state == GameState.Paused && pausePanel != null)
        {
            pausePanel.SetActive(true);
            return;
        }

        if (sceneLoader == null) return;

        if (state == GameState.GameOver)
        {
            StartCoroutine(LoadGameOverNextFrame());
            return;
        }

        if (state == GameState.GameClear)
        {
            sceneLoader.LoadGameClearScene();
        }
    }

    private IEnumerator LoadGameOverNextFrame()
    {
        yield return null;

        if (sceneLoader != null)
        {
            sceneLoader.LoadGameOverScene();
        }
    }

    private void HideAll()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
    }
}
