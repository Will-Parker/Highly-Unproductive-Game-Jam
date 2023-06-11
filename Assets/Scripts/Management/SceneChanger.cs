using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * Author(s): William Parker
 * 
 * Credit: https://forum.unity.com/threads/how-can-i-open-previous-scene.652507/, https://answers.unity.com/questions/1262342/how-to-get-scene-name-at-certain-buildindex.html
 * 
 * Audio Manager
 **/
public class SceneChanger : MonoBehaviour
{
    public static SceneChanger instance;

    private List<string> sceneHistory = new List<string>();  //running history of scenes
    //The last string in the list is always the current scene running

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
    }

    private void Start()
    {
        sceneHistory.Add(SceneManager.GetActiveScene().name);
    }

    //Call this whenever you want to load a new scene
    //It will add the new scene to the sceneHistory list
    public void LoadScene(string newScene)
    {
        sceneHistory.Add(newScene);
        SceneManager.LoadSceneAsync(newScene);
    }

    //Call this whenever you want to load a new scene
    //It will add the new scene to the sceneHistory list
    public void LoadScene(int newSceneIndex)
    {
        sceneHistory.Add(NameFromIndex(newSceneIndex));
        SceneManager.LoadSceneAsync(newSceneIndex);
    }

    //Call this whenever you want to load the previous scene
    //It will remove the current scene from the history and then load the new last scene in the history
    //It will return false if we have not moved between scenes enough to have stored a previous scene in the history
    public void PreviousScene()
    {
        if (sceneHistory.Count >= 2)  //Checking that we have actually switched scenes enough to go back to a previous scene
        {
            sceneHistory.RemoveAt(sceneHistory.Count - 1);
            SceneManager.LoadSceneAsync(sceneHistory[sceneHistory.Count - 1]);
        }
        else
        {
            Debug.LogWarning("No Scene to go back to");
        }
    }

    public void LoadNextScene()
    {
        int currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
        sceneHistory.Add(NameFromIndex(currentSceneBuildIndex + 1));

        if (NameFromIndex(currentSceneBuildIndex + 1).Equals("GameCompletion"))
        {
            // IDK
            // credits, back to title, and level scores?
            // to be continued
        }

        SceneManager.LoadSceneAsync(currentSceneBuildIndex + 1);
    }

    public void ReturnToTitle()
    {
        sceneHistory.Clear();
        sceneHistory.Add("TitleScreen");
        SceneManager.LoadSceneAsync(0);
    }



    private static string NameFromIndex(int BuildIndex)
    {
        string path = SceneUtility.GetScenePathByBuildIndex(BuildIndex);
        int slash = path.LastIndexOf('/');
        string name = path.Substring(slash + 1);
        int dot = name.LastIndexOf('.');
        return name.Substring(0, dot);
    }
}