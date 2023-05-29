using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    [Header("Drop Slots")]
    [SerializeField] private DropSlot slot1;
    [SerializeField] private DropSlot slot2;

    [Header("Stories")]
    [SerializeField] private readonly TextAsset[] AppleBlueberryStories;
    [SerializeField] private readonly TextAsset[] AppleLemonStories;
    [SerializeField] private readonly TextAsset[] AppleStrawberryStories;
    [SerializeField] private readonly TextAsset[] BlueberryLemonStories;
    [SerializeField] private readonly TextAsset[] BlueberryStrawberryStories;
    [SerializeField] private readonly TextAsset[] LemonStrawberryStories;
    private TextAsset[] unusedAppleBlueberryStories;
    private TextAsset[] unusedAppleLemonStories;
    private TextAsset[] unusedAppleStrawberryStories;
    private TextAsset[] unusedBlueberryLemonStories;
    private TextAsset[] unusedBlueberryStrawberryStories;
    private TextAsset[] unusedLemonStrawberryStories;

    [Header("Testing")]
    [SerializeField] private TextAsset inkJSON; // FOR TESTING ONLY

    private void Start()
    {
        unusedAppleBlueberryStories = AppleBlueberryStories;
        unusedAppleLemonStories = AppleLemonStories;
        unusedAppleStrawberryStories = AppleStrawberryStories;
        unusedBlueberryLemonStories = BlueberryLemonStories;
        unusedBlueberryStrawberryStories = BlueberryStrawberryStories;
        unusedLemonStrawberryStories = LemonStrawberryStories;
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
