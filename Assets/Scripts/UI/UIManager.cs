using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private ActionUIManager auim;

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
    }

    public void UpdateUI()
    {
        auim.UpdateActionUI(UIActionMode.None);
    }
}
