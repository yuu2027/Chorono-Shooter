using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private const string TitleSceneName = "TitleScene";
    private const string GameSceneName = "GameScene";
    private const string GameOverSceneName = "GameOverScene";
    private const string GameClearSceneName = "GameClearScene";

    public void StartGame()
    {
        LoadScene(GameSceneName);
    }

    public void RetryGame()
    {
        LoadScene(GameSceneName);
    }

    public void LoadTitleScene()
    {
        LoadScene(TitleSceneName);
    }

    public void LoadGameScene()
    {
        LoadScene(GameSceneName);
    }

    public void LoadGameOverScene()
    {
        LoadScene(GameOverSceneName);
    }

    public void LoadGameClearScene()
    {
        LoadScene(GameClearSceneName);
    }

    private void LoadScene(string sceneName)
    {
        ResetTimeScale();
        SceneManager.LoadScene(sceneName);
    }

    private void ResetTimeScale()
    {
        Time.timeScale = 1.0f;
    }

}
