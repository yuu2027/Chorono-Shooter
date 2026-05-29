using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioCueLibrary", menuName = "Data/Audio Cue Library")]
public class AudioCueLibrary : ScriptableObject
{
    [Serializable]
    public class BgmCue
    {
        public BgmId id;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        public float fadeSeconds = 0.5f;
    }

    [Serializable]
    public class SeCue
    {
        public SeId id;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch = 1f;
        public float minInterval = 0.03f;
    }

    [SerializeField] private BgmCue[] bgmCues;
    [SerializeField] private SeCue[] seCues;

    public BgmCue GetBgm(BgmId id)
    {
        foreach (BgmCue cue in bgmCues)
        {
            if (cue != null && cue.id == id) return cue;
        }

        return null;
    }

    public SeCue GetSe(SeId id)
    {
        foreach (SeCue cue in seCues)
        {
            if (cue != null && cue.id == id) return cue;
        }

        return null;
    }
}