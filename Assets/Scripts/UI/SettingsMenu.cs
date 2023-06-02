using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider musicVolume;
    private void OnEnable()
    {
        StartCoroutine(WaitOneFrame());
    }

    public void MusicSlider()
    {
        AudioManager.instance.SetVolume(SoundType.Music, musicVolume.value);
    }

    private IEnumerator WaitOneFrame()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        if (musicVolume.isActiveAndEnabled)
        {
            musicVolume.value = AudioManager.instance.volumes[SoundType.Music];
        }
    }
}
