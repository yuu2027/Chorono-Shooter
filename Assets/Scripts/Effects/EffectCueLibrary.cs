using System;
using System.Collections.Specialized;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectCueLibrary", menuName = "Data/Effects/Effect Cue Library")]
public class EffectCueLibrary : ScriptableObject
{
    [Serializable]
    public class EffectCue
    {
        public EffectCueId id;
        public GameObject prefab;
        public float lifeTime = 2.0f;
        public Vector3 offset;
        public Vector3 scale = Vector3.one;
        public bool useRotation;
    }

    [SerializeField] private EffectCue[] cues;

    public bool TryGetCue(EffectCueId id, out EffectCue cue)
    {
        if (cues != null)
        {
            for (int i = 0; i < cues.Length; i++)
            {
                if (cues[i] != null && cues[i].id == id)
                {
                    cue = cues[i];
                    return true;
                }
            }
        }

        cue = null;
        return false;
    }
}
