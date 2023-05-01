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
        engineAudioSource.pitch = 0.5f + level * 2f;
        engineAudioSource.volume = 0.2f + level * 0.4f;
    }

}
