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


    // Private fields
    private Dictionary<string, AudioClip> clips;
    private AudioSource audioSource;

    void OnValidate() {
        // Move sounds from the array into a hashmap
        clips = new Dictionary<string, AudioClip>();
        foreach (var clip in audioClips) {
            clips[clip.name] = clip.clip;
        }
    }

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayClip(string name) {
        audioSource.PlayOneShot(clips[name]);
    }

}
