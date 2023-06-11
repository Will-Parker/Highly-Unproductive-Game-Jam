using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/**
 * Author(s): William Parker
 * 
 * Credit: https://www.youtube.com/watch?v=6OT43pvUyfY
 * 
 * Sound
 **/
[System.Serializable]
public class Sound
{
    public string name;

    public SoundType type;

    public AudioClip clip;

    [Range(0,1)]
    public float volume = 1;
    [Range(0.1f,3)]
    public float pitch = 1;

    public bool loop = false;

    public bool hasIntro = false;

    public AudioClip introClip;

    [HideInInspector]
    public AudioSource source;
}

public enum SoundType
{
    Music,
    SFX,
    Ambient
}