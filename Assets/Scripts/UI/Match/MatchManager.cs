using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    [Header("Drop Slots")]
    [SerializeField] private DropSlot slot1;
    [SerializeField] private DropSlot slot2;

    [Header("Testing")]
    [SerializeField] private TextAsset inkJSON; // FOR TESTING ONLY

    private void Start()
    {
        
    }

    public void Match()
    {
        if (slot1.GetAllyInSlot() != null && slot2.GetAllyInSlot() != null)
        {
            // go to dialogue window with slot1.GetAllyInSlot() and slot2.GetAllyInSlot() plugged in
            Time.timeScale = 1f;
            DialogueManager.GetInstance().EnterDialogueMode(inkJSON); // temp
            FindObjectOfType<CharacterControl>().UnsubFromAllGameplayActions();
            FindObjectOfType<CharacterControl>().SubToAllDialogueActions();
            slot1.EmptySlot();
            slot2.EmptySlot();
            gameObject.SetActive(false);
            //FindObjectOfType<CharacterControl>().OpenDialogue(inkJSON);
        }
    }
}
