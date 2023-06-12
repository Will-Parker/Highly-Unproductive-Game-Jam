using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private RectTransform partyGroup;
    [SerializeField] private GameObject settings;
    [SerializeField] private Canvas tutorialCanvas;
    private PartyManager pm;
    private void Awake()
    {
        pm = FindObjectOfType<PartyManager>();
    }

    private void OnEnable()
    {
        settings.SetActive(false);

        for (int i = 0; i < 4; i++)
        {
            GameObject obj = null;
            switch (pm.allies[i].type)
            {
                case AllyType.Apple:
                    obj = Instantiate(Resources.Load("Prefabs/UI/Pause/Apple Group", typeof(GameObject)), partyGroup) as GameObject;
                    break;
                case AllyType.Strawberry:
                    obj = Instantiate(Resources.Load("Prefabs/UI/Pause/Strawberry Group", typeof(GameObject)), partyGroup) as GameObject;
                    break;
                case AllyType.Lemon:
                    obj = Instantiate(Resources.Load("Prefabs/UI/Pause/Lemon Group", typeof(GameObject)), partyGroup) as GameObject;
                    break;
                case AllyType.Blueberry:
                    obj = Instantiate(Resources.Load("Prefabs/UI/Pause/Blueberry Group", typeof(GameObject)), partyGroup) as GameObject;
                    break;
            }
            if (obj != null)
            {
                AllyPauseGroup apg = obj.GetComponent<AllyPauseGroup>();
                apg.id = i;
                apg.SetText();
            }
        }

    }

    private void OnDisable()
    {
        foreach (Transform child in partyGroup)
        {
            Destroy(child.gameObject);
        }
    }

    public void Settings()
    {
        AudioManager.instance.Play("Button");
        if (settings.activeSelf)
        {
            settings.SetActive(false);
        }
        else
        {
            settings.SetActive(true);
        }
    }

    public void Help()
    {
        AudioManager.instance.Play("Button");
        Time.timeScale = 1f;
        CharacterControl.instance.UnsubFromEverything();
        tutorialCanvas.gameObject.SetActive(true);
        CharacterControl.instance.SubToAllDialogueActions();
        StoryMenu sm = FindObjectOfType<StoryMenu>();
        if (sm != null)
            sm.StartHelp();
    }

    public void Retry()
    {
        AudioManager.instance.Play("Button");
        SceneChanger.instance.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        AudioManager.instance.Play("Button");
        Application.Quit();
    }

    public void ReturnToTile()
    {
        AudioManager.instance.Play("Button");
        AudioManager.instance.Stop("Gameplay Music");
        CharacterControl.instance.UnsubFromEverything();
        SceneChanger.instance.ReturnToTitle();
    }
}
