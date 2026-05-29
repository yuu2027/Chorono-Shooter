using UnityEngine;

public class SceneBgmPlayer : MonoBehaviour
{
    [SerializeField] private BgmId bgmId;
    
    void Start()
    {
        AudioManager.Instance?.PlayBgm(bgmId);    
    }
}
