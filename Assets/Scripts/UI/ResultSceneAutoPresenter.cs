using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ResultSceneAutoPresenter
{
    private const string GameOverSceneName = "GameOverScene";
    private const string GameClearSceneName = "GameClearScene";
    private const string ScoreTextName = "Score";
    private const string KillCountTextName = "KillCount";
    private const string PlayTimeTextName = "PlayTime";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void RegisterSceneLoaded()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        ApplyToScene(SceneManager.GetActiveScene());
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyToScene(scene);
    }

    private static void ApplyToScene(Scene scene)
    {
        if (scene.name != GameOverSceneName && scene.name != GameClearSceneName) return;

        TMP_Text scoreText = null;
        TMP_Text killCountText = null;
        TMP_Text playTimeText = null;

        TMP_Text[] texts = Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Exclude);

        for (int i = 0; i < texts.Length; i++)
        {
            TMP_Text text = texts[i];
            if (text == null || text.gameObject.scene != scene) continue;

            switch (text.gameObject.name)
            {
                case ScoreTextName:
                    scoreText = text;
                    break;
                case KillCountTextName:
                    killCountText = text;
                    break;
                case PlayTimeTextName:
                    playTimeText = text;
                    break;
            }
        }

        if (scoreText != null)
        {
            scoreText.text = GameResultStore.Score.ToString();
        }

        if (killCountText != null)
        {
            killCountText.text = GameResultStore.KillCount.ToString();
        }

        if (playTimeText != null)
        {
            int totalSeconds = Mathf.FloorToInt(GameResultStore.PlayTime);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            playTimeText.text = $"{minutes:00}:{seconds:00}";
        }
    }
}
