using UnityEngine;

public class TileMenuView : MonoBehaviour
{
    [SerializeField] private GameObject howToPanel;
    [SerializeField] private GameObject settingPanel;

    private void Start()
    {
        CloseAllPanels();
    }

    public void CloseAllPanels()
    {
        if (howToPanel != null)
        {
            
            howToPanel.SetActive(false);
        }

        if (settingPanel != null)
        {
            
            settingPanel.SetActive(false);
        }
    }

    public void ShowHowToPanel()
    {
        CloseAllPanels();

        AudioManager.Instance?.PlaySe(SeId.Button);

        if (howToPanel != null)
        {
            howToPanel.SetActive(true);
        }
    }

    public void ShowSettingPanel()
    {
        CloseAllPanels();

        AudioManager.Instance?.PlaySe(SeId.Button);

        if (settingPanel != null)
        {
            settingPanel.SetActive(true);
        }
    }
}
