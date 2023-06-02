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
        SceneManager.LoadSceneAsync(1);
    }
}
