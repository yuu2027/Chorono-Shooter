using System;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] Transform bossSpawnPoint;
    [SerializeField] private float bossEntranceWarningTime = 3.0f;

    private bool bossSpawned;

    public BossController ActiveBoss { get; private set; }
    public event Action<BossController> BossSpawned;
    public event Action<BossController, float> BossEntranceStarted;

    // 通常の敵が全てスポーンしたらボスを生成する
    private void Update()
    {
        if (bossSpawned) return;
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing) return;
        if (enemySpawner == null || !enemySpawner.IsFinished) return;
        if (bossPrefab == null || bossSpawnPoint == null) return;

        BulletPool.Instance.ClearAllActiveBullets();

        GameObject bossObject = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.Euler(0.0f, 0.0f, 180.0f));
        ActiveBoss = bossObject.GetComponent<BossController>();

        bossSpawned = true;

        if (ActiveBoss != null)
        {
            GameManager.Instance?.EnterCinematic();
            ActiveBoss.StartEntranceWait(bossEntranceWarningTime);

            BossSpawned?.Invoke(ActiveBoss);
            BossEntranceStarted?.Invoke(ActiveBoss, bossEntranceWarningTime);
        }
    }
}
