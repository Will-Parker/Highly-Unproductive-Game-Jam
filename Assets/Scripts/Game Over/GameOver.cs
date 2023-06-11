using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    private void Start()
    {
        AudioManager.instance.Play("Game Over");
    }

    public void ReturnToTitle()
    {
        AudioManager.instance.Play("Button");
        AudioManager.instance.Stop("Game Over");
        SceneChanger.instance.ReturnToTitle();
    }

    public void QuitGame()
    {
        AudioManager.instance.Play("Button");
        Application.Quit();
    }

    public void Retry()
    {
        AudioManager.instance.Play("Button");
        AudioManager.instance.Stop("Game Over");
        SceneChanger.instance.PreviousScene();
    }
}
