using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public GameObject credits;
    public GameObject instructions;
    public GameObject settings;

    public void QuitGame()
    {
        Application.Quit();
    }

    private void Start()
    {
        AudioManager.instance.Play("Title Music");
        GameData.ResetAllyStats();
    }

    public void StartGame()
    {
        AudioManager.instance.Stop("Title Music");
        SceneChanger.instance.LoadScene(2);
    }

    public void Credits()
    {
        AudioManager.instance.Play("Button");
        instructions.SetActive(false);
        settings.SetActive(false);
        if (credits.activeSelf)
        {
            credits.SetActive(false);
        }
        else
        {
            credits.SetActive(true);
        }
    }

    public void Instructions()
    {
        AudioManager.instance.Play("Button");
        credits.SetActive(false);
        settings.SetActive(false);
        if (instructions.activeSelf)
        {
            instructions.SetActive(false);
        }
        else
        {
            instructions.SetActive(true);
        }
    }

    public void Settings()
    {
        AudioManager.instance.Play("Button");
        credits.SetActive(false);
        instructions.SetActive(false);
        if (settings.activeSelf)
        {
            settings.SetActive(false);
        }
        else
        {
            settings.SetActive(true);
        }
    }

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
}
