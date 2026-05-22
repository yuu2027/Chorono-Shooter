using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;

    [Header("Start Settings")]
    [SerializeField] private bool startGameOnStart = true;

    public GameState CurrentState { get; private set; } = GameState.Ready;
    public int Score { get; private set; }
    public int killCount {  get; private set; }
    public float PlayTime {  get; private set; }

    public event Action<GameState> StateChanged;
    public event Action<int> ScoreChanged;
    public event Action<int> killCountChanged;
    public event Action<float> PlayTimeChanged;

    private void Awake()
    {
        // GameManagerオブジェクトが2つあるとき片方を削除
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if(enemySpawner == null)
        {
            enemySpawner = FindAnyObjectByType<EnemySpawner>();
        }
    }

    private void Start()
    {
        if(startGameOnStart)
        {
            StartGame(); // ゲームをスタートさせる
        }
    }

    // プレイ中の確認と時間更新
    private void Update()
    {
        if (CurrentState != GameState.Playing) return;

        PlayTime += Time.deltaTime;
        PlayTimeChanged?.Invoke(PlayTime);
    }

    // スコアなどの初期化とゲーム状態をプレイングに変更し敵のスポーンを開始
    public void StartGame()
    {
        Time.timeScale = 1.0f;

        Score = 0;
        killCount = 0;
        PlayTime = 0.0f;

        ScoreChanged?.Invoke(Score);
        killCountChanged?.Invoke(killCount);
        PlayTimeChanged?.Invoke(PlayTime);

        ChangeState(GameState.Playing);

        if (enemySpawner != null)
        {
            enemySpawner.StartSpawning();
        }
    }

    // ポーズ状態にする
    public void PauseGame()
    {
        if (CurrentState != GameState.Playing) return;

        Time.timeScale = 0.0f;
        ChangeState(GameState.Paused);
    }

    // ゲームを再開させる
    public void ResumeGame()
    {
        if(CurrentState != GameState.Paused) return;

        Time.timeScale = 1.0f;
        ChangeState(GameState.Playing);
    }

    // 現在の状態がポーズか確認する
    public void TogglePause()
    {
        if(CurrentState == GameState.Playing)
        {
            PauseGame(); // ポーズ状態
            return;
        }

        if(CurrentState == GameState.Paused)
        {
            ResumeGame(); // ゲーム再開
        }
    }
    
    // シーンを最初に戻しリトライする
    public void RetryGame()
    {
        Time.timeScale = 1.0f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ゲームオーバー状態にして敵スポーンを止める
    public void GameOver()
    {
        if (CurrentState == GameState.GameOver) return;
        if (CurrentState == GameState.GameClear) return;

        Time.timeScale = 1.0f;

        if (enemySpawner != null)
        {
            enemySpawner.StopSpawning();
        }

        ChangeState(GameState.GameOver);
    }

    // ゲームクリア状態にて敵スポーンを止める
    public void GameClear()
    {
        if (CurrentState == GameState.GameClear) return;
        if (CurrentState == GameState.GameOver) return;

        Time.timeScale = 1.0f;

        if(enemySpawner != null)
        {
            enemySpawner.StopSpawning();
        }

        ChangeState(GameState.GameClear);
    }

    // スコアを加算する
    public void AddScore(int value)
    {
        if (value <= 0) return;
        if (CurrentState != GameState.Playing) return;

        Score += value;
        ScoreChanged?.Invoke(Score);
    }

    // キル数を加算する
    public void AddKillCount()
    {
        if (CurrentState != GameState.Playing) return;

        killCount++;
        killCountChanged?.Invoke(killCount);
    }

    // ゲーム状態を変更する
    private void ChangeState(GameState nextState)
    {
        if (CurrentState == nextState) return;

        CurrentState = nextState;
        StateChanged?.Invoke(CurrentState);
    }

    // 現在のGamaManegerが破棄されるときに、Instanceの参照を消す処理
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

}
