using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private ActionUIManager auim;
    public PauseManager pausem;
    private MatchManager mm;
    private ReorderPartyManager rpm;
    private CharacterControl cc;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        auim = FindObjectOfType<ActionUIManager>();
        pausem = FindObjectOfType<PauseManager>();
        mm = FindObjectOfType<MatchManager>();
        rpm = FindObjectOfType<ReorderPartyManager>();
        cc = CharacterControl.instance;
    }
    private void Start()
    {
        pausem.gameObject.SetActive(false);
        mm.gameObject.SetActive(false);
        if (GameStateManager.instance.levelID != 0)
        {
            rpm.gameObject.SetActive(true);
            StoryMenu sm = FindObjectOfType<StoryMenu>();
            if (sm != null)
                sm.transform.parent.gameObject.SetActive(false);
            else
                Debug.LogError("No Tutorial in scene");
        }
        else
        {
            rpm.gameObject.SetActive(false);
            StoryMenu sm = FindObjectOfType<StoryMenu>();
            if (sm != null)
            {
                CharacterControl.instance.UnsubFromEverything();
                sm.transform.parent.gameObject.SetActive(true);
                CharacterControl.instance.SubToAllDialogueActions();
                sm.StartHelp();
            }
            else
                Debug.LogError("No Tutorial in scene");
        }
        Time.timeScale = 1f;
        UpdateUI();
    }

    public void UpdateUI()
    {
        auim.UpdateActionUI(UIActionMode.None);
    }

    public void Pause()
    {
        if (mm.gameObject.activeSelf)
        {
            return;
            //mm.gameObject.SetActive(false);
            //Time.timeScale = 1f;
            //cc.SubToAllGameplayActions();
        }
        AudioManager.instance.Play("Button");
        if (pausem.gameObject.activeSelf)
        {
            pausem.gameObject.SetActive(false);
            Time.timeScale = 1f;
            cc.SubToAllGameplayActions();
        }
        else if (rpm.gameObject.activeSelf)
        {
            rpm.SetOut();
        }
        else
        {
            pausem.gameObject.SetActive(true);
            Time.timeScale = 0f;
            cc.UnsubFromAllGameplayActions();
        }
    }

    public void BeginMatch()
    {
        AudioManager.instance.Stop("Gameplay Music");
        AudioManager.instance.Play("Battle Victory");
        Destroy(GameStateManager.instance.gameObject);
        mm.gameObject.SetActive(true);
        Time.timeScale = 0f;
        cc.UnsubFromAllGameplayActions();
    }

    public void OpenReorderPartyMenu()
    {
        rpm.gameObject.SetActive(true);
    }
}
