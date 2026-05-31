using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossEmergencyView : MonoBehaviour
{
    [SerializeField] private StageManager stageManager;
    [SerializeField] private Image redOverlay;
    [SerializeField] private TMP_Text emergencyText;
    [SerializeField] private float flashInterval = 0.12f; // “_–ÅŠÔŠu

    private void Start()
    {
        if (stageManager == null) stageManager = FindAnyObjectByType<StageManager>();        // StageManagerŽæ“¾
        if (stageManager != null) stageManager.BossEntranceStarted += OnBossEntranceStarted;

        SetActive(false);
        SetAlpha(0.0f);
    }

    private void OnDestroy()
    {
        if (stageManager != null) stageManager.BossEntranceStarted -= OnBossEntranceStarted;
    }

    private void OnBossEntranceStarted(BossController boss, float duration)
    {
        StartCoroutine(ShowCoroutine(duration));
    }

    private IEnumerator ShowCoroutine(float duration)
    {
        float elapsed = 0.0f;

        SetActive(true);

        AudioManager.Instance?.PlaySe(SeId.Emergency);

        while (elapsed < duration)
        {
            float flash = Mathf.PingPong(elapsed / flashInterval, 1.0f);
            SetAlpha(flash);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        AudioManager.Instance?.StopSe(duration);

        SetAlpha(0.0f);
        SetActive(false);
    }

    private void SetActive(bool visible)
    {
        if (redOverlay != null) redOverlay.gameObject.SetActive(visible);
        if (emergencyText != null) emergencyText.gameObject.SetActive(visible);
    }

    private void SetAlpha(float flash)
    {
        if (redOverlay != null)
        {
            redOverlay.color = new Color(1.0f, 0.0f, 0.0f, flash * 0.45f);
        }

        if (emergencyText != null)
        {
            Color color = emergencyText.color;
            color.a = flash;
            emergencyText.color = color;
        }
    }
}