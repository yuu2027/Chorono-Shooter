using System;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] Transform bossSpawnPoint;

    private bool bossSpawned;

    public BossController ActiveBoss { get; private set; }
    public event Action<BossController> BossSpawned;

    // 通常の敵が全てスポーンしたらボスを生成する
    private void Update()
    {
        if (bossSpawned) return;
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing) return;
        if(enemySpawner == null || !enemySpawner.IsFinished) return;
        if (bossPrefab == null || bossSpawnPoint == null) return;

        GameObject bossObject = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
        ActiveBoss = bossObject.GetComponent<BossController>();

        bossSpawned = true;

        if(ActiveBoss != null)
        {
            BossSpawned?.Invoke(ActiveBoss);
        }
    }
}
