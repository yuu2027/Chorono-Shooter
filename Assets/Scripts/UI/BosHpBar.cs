using UnityEngine;
using UnityEngine.UI;

public class BosHpBar : MonoBehaviour
{
    [SerializeField] private StageManager stageManager;
    [SerializeField] private Slider hpSlider;

    private BossController currentBoss;

    private void Start()
    {
        if(stageManager == null)
        {
            stageManager = FindAnyObjectByType<StageManager>();
        }

        if(hpSlider != null)
        {
            hpSlider.gameObject.SetActive(false); // 最初は非表示にする
        }

        if (stageManager != null)
        {
            stageManager.BossSpawned += OnBossSpawned;
        }
    }

    private void OnDestroy()
    {
        if(stageManager != null)
        {
            stageManager.BossSpawned -= OnBossSpawned;
        }

        UnsubscribeBoss();
    }

    // ボスがスポーンしたとき
    private void OnBossSpawned(BossController boss)
    {
        UnsubscribeBoss();

        currentBoss = boss;
        currentBoss.HealthChanged += OnBossHealthChanged;
        currentBoss.BossDied += OnBossDied;

        if(hpSlider != null)
        {
            hpSlider.gameObject.SetActive(true);
        }

        OnBossHealthChanged(currentBoss.CurrentHp, currentBoss.MaxHp);
    }

    // HPバーを更新する
    private void OnBossHealthChanged(int current, int max)
    {
        if (hpSlider == null) return;

        hpSlider.maxValue = max;
        hpSlider.value = current;
    }

    // ボスが死亡したときHPバーを非表示にする
    private void OnBossDied()
    {
        if(hpSlider != null)
        {
            hpSlider.gameObject.SetActive(false);
        }

        UnsubscribeBoss();
    }

    // HP変更イベントと死亡時イベントを削除
    private void UnsubscribeBoss()
    {
        if (currentBoss == null) return;

        currentBoss.HealthChanged -= OnBossHealthChanged;
        currentBoss.BossDied -= OnBossDied;
        currentBoss = null;
    }
}
