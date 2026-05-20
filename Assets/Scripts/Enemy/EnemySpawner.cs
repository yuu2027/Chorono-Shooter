using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private sealed class EnemySpawnRuntimeRule // このクラスを継承できないようにせって
    {
        public string enemyId;         // 敵のID
        public GameObject enemyPrefab; // 敵のプレハブ

        public float startTime;     // 敵が出現が始まる時間
        public float endTime;       // 敵の出現が終わる時間
        public float spawnInterval; // 出現間隔
        public int maxAlive;        // 最大何体まで出すか

        public float minViewportX;   // 出現する領域X軸の最小値
        public float maxViewportX;   // 出現する領域X軸の最大値
        public float spawnViewportY; // 出現するY軸座標

        public float timer; // 出現するY軸座標
        public readonly List<EnemyBase> aliveEnemies = new List<EnemyBase>(); // 生きている敵のリスト

        public EnemySpawnRuntimeRule(EnemySpawnRuleData data) // コンストラクタ
        {
            enemyId = data.enemyId;
            enemyPrefab = data.enemyPrefab;

            startTime = Mathf.Max(0.0f, data.startTime);
            endTime = Mathf.Max(0.0f, data.endTime);
            spawnInterval = Mathf.Max(0.01f, data.spawnInterval);
            maxAlive = Mathf.Max(1, data.maxAlive);

            minViewportX = Mathf.Clamp01(data.minViewportX);
            maxViewportX = Mathf.Clamp01(data.maxViewportX);
            spawnViewportY = data.spawnViewportY;

            if (minViewportX > maxViewportX)
            {
                float temp = minViewportX;
                minViewportX = maxViewportX;
                maxViewportX = temp;
            }
        }
    }

    [Header("Spawn Settings")]
    [SerializeField] private bool spawnOnStart = true;                   // 出現開始のフラグ
    [SerializeField] private bool destroySpawnedEnemiesOnRestart = true; // Spawnerを再スタートしたとき、すでに出現済みの敵を削除するかどうか
    [SerializeField] private Camera targetCamera;                        // 敵の出現位置を計算するために使うカメラ
    [SerializeField] private Transform enemyParent;                      // 生成した敵をどのオブジェクトの子にするか
    [SerializeField] private float spawnZ = 0.0f;                        // 敵を生成するZ座標。ViewportToWorldPoint() を使うと、カメラとの距離の関係でZ座標も関係するため

    [Header("Stage Data")]
    [SerializeField] private StageSpawnData stageSpawnData; // ScriptableObject

    [Header("Fallback Spawn Area")]
    [SerializeField] private Vector2 fallbackSpawnXRange = new Vector2(-7.5f, 7.5f); // targetCameraがない場合に使う、敵のX座標の範囲
    [SerializeField] private float fallbackSpawnY = 5.5f;                            // targetCamera がない場合に使う、敵のY座標

    private readonly List<EnemySpawnRuntimeRule> spawnRules = new List<EnemySpawnRuntimeRule>();

    private float elapsedTime; // 経過時間
    private bool isSpawning;   // 現在、敵の出現処理を動かしているかどうか

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        BuildSpawnRules();
    }

    private void Start()
    {
        if (spawnOnStart)
        {
            StartSpawning(); // スポーン開始
        }
    }

    private void Update()
    {
        if (!isSpawning) return;

        elapsedTime += Time.deltaTime; // 経過時間計算

        for (int i = 0; i < spawnRules.Count; i++) 
        {
            TickRule(spawnRules[i]);
        }
    }

    // 敵の出現処理を開始する
    public void StartSpawning()
    {
        if (isSpawning) // すでに出現中なら、出現済みの敵をリセットする
        {
            ResetSpawnedEnemies();
        }

        isSpawning = true;
        elapsedTime = 0.0f;

        for (int i = 0; i < spawnRules.Count; i++) // 各ルールのtimerを0に戻す
        {
            EnemySpawnRuntimeRule rule = spawnRules[i];
            if (rule == null) continue;

            rule.timer = 0.0f;
            CleanupAliveEnemies(rule);
        }
    }

    // スポーンを止める関数
    public void StopSpawning()
    {
        isSpawning = false;
    }

    // ScriptableObjectに保存されている出現データを、ゲーム実行中に使うためのルール一覧に変換
    private void BuildSpawnRules()
    {
        spawnRules.Clear();

        if (stageSpawnData == null)
        {
            Debug.LogWarning("EnemySpawner: StageSpawnDataが設定されていません。", this);
            return;
        }

        IReadOnlyList<EnemySpawnRuleData> rules = stageSpawnData.Rules;
        for(int i = 0; i < rules.Count; i++)
        {
            EnemySpawnRuleData data = rules[i];
            if (data == null) continue;

            spawnRules.Add(new EnemySpawnRuntimeRule(data));
        }
    }

    // 1つの出現ルールを毎フレーム処理する
    private void TickRule(EnemySpawnRuntimeRule rule)
    {
        if (rule == null) return;
        if (rule.enemyPrefab == null) return;
        if (!IsRuleActive(rule)) return; // 経過時間が過ぎていたら実行しない

        CleanupAliveEnemies(rule); //  消えた敵をaliveEnemiesから削除

        if (rule.aliveEnemies.Count >= rule.maxAlive) return;

        rule.timer -= Time.deltaTime;
        if (rule.timer > 0.0f) return; // 次の出現タイミングまで待つ

        Spawn(rule);
        rule.timer = rule.spawnInterval;
    }

    // 今、この出現ルールが有効な時間帯かどうかを判定する
    private bool IsRuleActive(EnemySpawnRuntimeRule rule)
    {
        if (elapsedTime < rule.startTime) return false;
        if (rule.endTime > 0.0f && elapsedTime > rule.endTime) return false;

        return true;
    }

    // 実際に敵を生成する
    private void Spawn(EnemySpawnRuntimeRule rule)
    {
        EnemyBase prefabEnemy = rule.enemyPrefab.GetComponent<EnemyBase>();
        if (prefabEnemy == null)
        {
            Debug.LogWarning($"EnemySpawner: {rule.enemyId} prefab に EnemyBase がありません。", rule.enemyPrefab);
            return;
        }

        EnemyBase enemy = Instantiate(
            prefabEnemy,
            GetRandomSpawnPosition(rule),
            Quaternion.identity,
            enemyParent
        );

        rule.aliveEnemies.Add(enemy);
    }

    // 敵を出すランダムな位置を計算する
    private Vector3 GetRandomSpawnPosition(EnemySpawnRuntimeRule rule)
    {
        if (targetCamera != null) // targetCameraがある場合
        {
            float viewportX = UnityEngine.Random.Range(rule.minViewportX, rule.maxViewportX); // 画面内のX座標をランダムに決定
            float cameraDistance = spawnZ - targetCamera.transform.position.z;

            if (cameraDistance <= 0.0f)
            {
                cameraDistance = Mathf.Abs(targetCamera.transform.position.z);
            }

            if (cameraDistance <= 0.0f)
            {
                cameraDistance = 10.0f;
            }

            // Viewport座標をWorld座標に変換
            Vector3 position = targetCamera.ViewportToWorldPoint(
                new Vector3(viewportX, rule.spawnViewportY, cameraDistance)
            );

            position.z = spawnZ;
            return position;
        }

        // targetCameraがない場合
        float x = UnityEngine.Random.Range(fallbackSpawnXRange.x, fallbackSpawnXRange.y);
        return new Vector3(x, fallbackSpawnY, spawnZ);
    }

    // Spawnerが管理している敵をリセットする
    // Destroyされた後、リストの中に参照だけ残っていることがあるため
    private void ResetSpawnedEnemies()
    {
        for (int i = 0; i < spawnRules.Count; i++)
        {
            EnemySpawnRuntimeRule rule = spawnRules[i];
            if (rule == null) continue;

            if (destroySpawnedEnemiesOnRestart)
            {
                DestroyAliveEnemies(rule);
            }
            else
            {
                CleanupAliveEnemies(rule);
            }
        }
    }

    // そのルールで生成された生存中の敵をすべて削除する
    private void DestroyAliveEnemies(EnemySpawnRuntimeRule rule)
    {
        for (int i = rule.aliveEnemies.Count - 1; i >= 0; i--)
        {
            if (rule.aliveEnemies[i] != null)
            {
                Destroy(rule.aliveEnemies[i].gameObject);
            }
        }

        rule.aliveEnemies.Clear();
    }

    // aliveEnemiesの中から、すでに消えた敵を取り除く
    private void CleanupAliveEnemies(EnemySpawnRuntimeRule rule)
    {
        for (int i = rule.aliveEnemies.Count - 1; i >= 0; i--)
        {
            if (rule.aliveEnemies[i] == null)
            {
                rule.aliveEnemies.RemoveAt(i); // Listの指定した位置の要素を削除するメソッド
            }
        }
    }
}
