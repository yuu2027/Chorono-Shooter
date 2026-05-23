using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Slider hpSlider;

    private void Start()
    {
        if(playerHealth == null)
        {
            playerHealth = FindAnyObjectByType<PlayerHealth>();
        }

        if (playerHealth == null) return;

        playerHealth.HealthChanged += OnHealthChanged;
        OnHealthChanged(playerHealth.CurrentHp, playerHealth.MaxHp);
    }

    private void OnDestroy()
    {
        if (playerHealth == null) return;

        playerHealth.HealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int current, int max)
    {
        if (hpSlider == null) return;

        hpSlider.maxValue = max;
        hpSlider.value = current;
    }
}
