using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private ActionUIManager auim;
    private PauseManager pausem;
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
        cc = FindObjectOfType<CharacterControl>();
    }
    private void Start()
    {
        Pause();
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
            cc.SubToAllActionsExceptPause();
        }
        else
        {
            pausem.gameObject.SetActive(true);
            Time.timeScale = 0f;
            cc.UnsubFromAllActionsExceptPause();
        }
    }
}
