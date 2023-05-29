using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private ActionUIManager auim;
    private PauseManager pausem;
    private MatchManager mm;
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

        DontDestroyOnLoad(gameObject);

        auim = FindObjectOfType<ActionUIManager>();
        pausem = FindObjectOfType<PauseManager>();
        mm = FindObjectOfType<MatchManager>();
        cc = FindObjectOfType<CharacterControl>();
    }
    private void Start()
    {
        pausem.gameObject.SetActive(false);
        mm.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void UpdateUI()
    {
        auim.UpdateActionUI(UIActionMode.None);
    }

    public void Pause()
    {
        if (pausem.gameObject.activeSelf)
        {
            pausem.gameObject.SetActive(false);
            Time.timeScale = 1f;
            cc.SubToAllGameplayActions();
        }
        else if (mm.gameObject.activeSelf)
        {
            mm.gameObject.SetActive(false);
            Time.timeScale = 1f;
            cc.SubToAllGameplayActions();
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
        mm.gameObject.SetActive(true);
        Time.timeScale = 0f;
        cc.UnsubFromAllGameplayActions();
    }
}
