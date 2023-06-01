using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public GameObject credits;
    public GameObject instructions;

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void Credits()
    {
        instructions.SetActive(false);
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
        credits.SetActive(false);
        if (instructions.activeSelf)
        {
            instructions.SetActive(false);
        }
        else
        {
            instructions.SetActive(true);
        }
    }
}
