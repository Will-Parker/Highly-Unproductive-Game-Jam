using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider musicVolume;
    public Slider sfxVolume;
    public Slider ambientVolume;

    private void OnEnable()
    {
        StartCoroutine(WaitOneFrame());
    }

    public void MusicSlider()
    {
        AudioManager.instance.SetVolume(SoundType.Music, musicVolume.value);
    }

    public void SFXSlider()
    {
        AudioManager.instance.SetVolume(SoundType.SFX, sfxVolume.value);
    }

    public void AmbientSlider()
    {
        AudioManager.instance.SetVolume(SoundType.Ambient, ambientVolume.value);
    }

    private IEnumerator WaitOneFrame()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        if (musicVolume.isActiveAndEnabled)
        {
            musicVolume.value = AudioManager.instance.volumes[SoundType.Music];
        }
        if (sfxVolume.isActiveAndEnabled)
        {
            sfxVolume.value = AudioManager.instance.volumes[SoundType.SFX];
        }
        if (ambientVolume.isActiveAndEnabled)
        {
            ambientVolume.value = AudioManager.instance.volumes[SoundType.Ambient];
        }
    }
}
