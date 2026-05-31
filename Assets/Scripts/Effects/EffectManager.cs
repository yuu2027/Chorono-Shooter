using UnityEngine;
using UnityEngine.UIElements;

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

    public GameObject Play(EffectCueId id, Vector3 position, float scaleMultiplier = 1.0f)
    {
        return Play(id, position, Quaternion.identity, scaleMultiplier);
    }

    public GameObject Play(EffectCueId id, Vector3 position, Quaternion rotation, float scaleMultiplier = 1.0f)
    {
        if (library == null) return null;
        if (!library.TryGetCue(id, out EffectCueLibrary.EffectCue cue)) return null;
        if (cue.prefab == null) return null;

        Quaternion finalRotation = cue.useRotation ? rotation : Quaternion.identity;
        GameObject effect = Instantiate(cue.prefab, position + cue.offset, finalRotation);

        Vector2 scale = cue.scale;
        if (scale.x <= 0.0f) scale.x = 1.0f;
        if (scale.y <= 0.0f) scale.y = 1.0f;

        float finalMultiplier = Mathf.Max(0.0f, scaleMultiplier); 
        effect.transform.localScale = new Vector3(scale.x * finalMultiplier, scale.y * finalMultiplier, 1.0f);

        if (cue.lifeTime > 0.0f)
        {
            Destroy(effect, cue.lifeTime);
        }

        return effect;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
