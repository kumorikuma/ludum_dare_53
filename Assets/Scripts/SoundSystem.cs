using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundSystem : Singleton<SoundSystem> {
    // A hack to get a hashmap-like thing in the inspector
    [Serializable]
    public struct NamedAudioClip {
        public string name;
        public AudioClip clip;
    }
    [SerializeField]
    private NamedAudioClip[] audioClips;

    [SerializeField]
    private AudioSource sfxAudioSource;
    [SerializeField]
    private AudioSource engineAudioSource;
    [SerializeField]
    private AudioSource bgmAudioSource;

    [Header("Engine sound")]
    [SerializeField]
    private float minEnginePitch = 1.0f;
    [SerializeField]
    private float maxEnginePitch = 2.0f;
    [SerializeField]
    private float minEngineVolume = 0.3f;
    [SerializeField]
    private float maxEngineVolume = 0.7f;

    // Private fields
    private Dictionary<string, AudioClip> clips;

    void OnValidate() {
        // Move sounds from the array into a hashmap
        clips = new Dictionary<string, AudioClip>();
        foreach (var clip in audioClips) {
            clips[clip.name] = clip.clip;
        }
    }

    public void PlayClip(string name) {
        sfxAudioSource.PlayOneShot(clips[name]);
    }

    public void SetEngineLevel(float level) {
        engineAudioSource.pitch = minEnginePitch + level * (maxEnginePitch - minEnginePitch);
        engineAudioSource.volume = minEngineVolume + level * (maxEngineVolume - minEngineVolume);
    }

}
