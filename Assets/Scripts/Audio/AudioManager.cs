using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using UnityEngine.SceneManagement;

/**
 * Author(s): William Parker
 * 
 * Credit: https://www.youtube.com/watch?v=6OT43pvUyfY
 * 
 * Audio Manager
 **/
public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public Dictionary<SoundType, float> volumes { get; private set; }

    public static AudioManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    void Start()
    {
        volumes = new Dictionary<SoundType, float>()
            {
                { SoundType.Music, 1f },
                { SoundType.SFX, 1f },
                { SoundType.Ambient, 1f }
            };
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        if (!s.hasIntro)
            s.source.Play();
        else
        {
            s.source.PlayOneShot(s.introClip);
            s.source.PlayScheduled(AudioSettings.dspTime + s.introClip.length);
        }
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Stop();
    }

    public void SetVolume(SoundType soundType, float volume)
    {
        float clampedVolume = Mathf.Min(1f, Mathf.Max(0f, volume));
        volumes[soundType] = clampedVolume;
        foreach (Sound s in sounds)
        {
            if (s.type == soundType)
            {
                s.volume = clampedVolume;
                s.source.volume = clampedVolume;
            }
        }
    }
}
