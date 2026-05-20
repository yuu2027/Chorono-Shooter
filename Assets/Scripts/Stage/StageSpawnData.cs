using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageSpawnerData", menuName = "Data/Stages")]
public class StageSpawnData : ScriptableObject
{
    [SerializeField] private List<EnemySpawnRuleData> rules = new List<EnemySpawnRuleData>();

    public IReadOnlyList<EnemySpawnRuleData> Rules => rules; // 外部からリストの中身を読み取ることはできるが、追加・削除はできない形で公開する型

    // Unityエディタ上で値が変更されたときに自動で呼ばれる関数
    private void OnValidate()
    {
        for(int i = 0; i < rules.Count; i++)
        {
            rules[i]?.Normalize();
        }
    }
}

[Serializable]
public class EnemySpawnRuleData
{
    [Header("Enemy")]
    public string enemyId = "NormalEnemy";
    public GameObject enemyPrefab;

    [Header("Timing")]
    public float startTime = 0.0f;
    public float endTime = 0.0f;
    public float spawnInterval = 2.0f;
    public int maxAlive = 8;

    [Header("Spawn Area")]
    [Range(0.0f, 1.0f)] public float minViewportX = 0.1f;
    [Range(0.0f, 1.0f)] public float maxViewportX = 0.9f;
    public float spawnViewportY = 1.1f;

    public void Normalize() 
    {
        startTime = Mathf.Max(0.0f, startTime);
        endTime = Mathf.Max(0.0f, endTime);
        spawnInterval = Mathf.Max(0.01f, spawnInterval);
        maxAlive = Mathf.Max(1, maxAlive);

        minViewportX = Mathf.Clamp01(minViewportX);
        maxViewportX = Mathf.Clamp01(maxViewportX);

        if (minViewportX > maxViewportX)
        {
            float temp = minViewportX;
            minViewportX = maxViewportX;
            maxViewportX = temp;
        }
    }


}
