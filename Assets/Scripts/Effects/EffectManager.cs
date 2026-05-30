using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    [SerializeField] private EffectCueLibrary library; // エフェクトライブラリ

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Play(EffectCueId id, Vector3 position)
    {
        Play(id, position, Quaternion.identity);
    }

    public void Play(EffectCueId id, Vector3 position, Quaternion rotation)
    {
        if (library == null) return;
        if (!library.TryGetCue(id, out EffectCueLibrary.EffectCue cue)) return;
        if (cue.prefab == null) return;

        Quaternion finalRotation = cue.useRotation ? rotation : Quaternion.identity;
        GameObject effect = Instantiate(cue.prefab, position + cue.offset, finalRotation);
        effect.transform.localScale = cue.scale;

        if (cue.lifeTime > 0.0f)
        {
            Destroy(effect, cue.lifeTime);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
