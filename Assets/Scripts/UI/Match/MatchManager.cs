using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    [Header("Drop Slots")]
    [SerializeField] private DropSlot slot1;
    [SerializeField] private DropSlot slot2;

    private void Start()
    {
        
    }

    public void Match()
    {
        AudioManager.instance.Play("Button");
        if (slot1.GetAllyInSlot() != null && slot2.GetAllyInSlot() != null)
        {
            FindObjectOfType<PartyManager>().LevelUpExperience();
            Time.timeScale = 1f;
            DialogueManager.GetInstance().BeginDialogueWith(slot1.GetAllyInSlot().Value, slot2.GetAllyInSlot().Value);
            CharacterControl.instance.UnsubFromAllGameplayActions();
            CharacterControl.instance.SubToAllDialogueActions();
            slot1.EmptySlot();
            slot2.EmptySlot();
            foreach (Ally ally in FindObjectOfType<PartyManager>().allies)
            {
                ally.Heal(9999999);
            }
            gameObject.SetActive(false);
        }
    }
}
