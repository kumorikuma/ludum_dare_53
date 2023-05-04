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
    private AudioClip[] levelMusicClips;

    [SerializeField]
    private AudioSource sfxAudioSource;
    [SerializeField]
    private AudioSource engineAudioSource;
    [SerializeField]
    private AudioSource levelMusicSource;

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
    private float overallVolume = 1.0f;
    private float sfxVolume;
    private float sfxPitch;

    void Start() {
        // Move sounds from the array into a hashmap
        clips = new Dictionary<string, AudioClip>();
        foreach (var clip in audioClips) {
            clips[clip.name] = clip.clip;
        }

        sfxVolume = sfxAudioSource.volume;
        sfxPitch = sfxAudioSource.pitch;
    }

    void ResetSfxAudioSource() {
        sfxAudioSource.volume = sfxVolume;
        sfxAudioSource.pitch = sfxPitch;
    }

    public void PlayClip(string name) {
        if (!clips.ContainsKey(name)) {
            Debug.LogError($"SFX '{name}' not found");
            return;
        }
        if (name == "crash") {
            sfxAudioSource.pitch = 0.5f;
            sfxAudioSource.PlayOneShot(clips[name], 0.3f);
        } else {
            sfxAudioSource.PlayOneShot(clips[name]);
        }
        ResetSfxAudioSource();
    }

    public void PlayLevelMusic(int levelIndex) {
        int clipIndex = levelIndex - 1;
        if (clipIndex >= 0 && clipIndex < levelMusicClips.Length) {
            levelMusicSource.clip = levelMusicClips[clipIndex];
            levelMusicSource.Play();
        } else {
            levelMusicSource.Stop();
        }
    }

    public void SetEngineLevel(float level) {
        engineAudioSource.pitch = overallVolume * (minEnginePitch + level * (maxEnginePitch - minEnginePitch));
        engineAudioSource.volume = overallVolume * (minEngineVolume + level * (maxEngineVolume - minEngineVolume));
    }

    public void SetVolume(float level) {
        overallVolume = level;
        sfxAudioSource.volume = overallVolume;
        engineAudioSource.volume = overallVolume;
        levelMusicSource.volume = overallVolume;
    }
}
