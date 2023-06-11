using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MatchManager : MonoBehaviour
{
    [Header("Turn Text")]
    [SerializeField] private TextMeshProUGUI turnText;
    [Header("Drop Slots")]
    [SerializeField] private DropSlot slot1;
    [SerializeField] private DropSlot slot2;
    [Header("Rank")]
    [SerializeField] private Image rankBadge;
    [SerializeField] private TextMeshProUGUI goldRequirements;
    [SerializeField] private TextMeshProUGUI silverRequirements;
    [SerializeField] private TextMeshProUGUI bronzeRequirements;

    private void Start()
    {
        goldRequirements.text = GameData.levelRankRequirements[GameStateManager.instance.levelID][0].ToString() + " Turns";
        silverRequirements.text = GameData.levelRankRequirements[GameStateManager.instance.levelID][1].ToString() + " Turns";
        bronzeRequirements.text = GameData.levelRankRequirements[GameStateManager.instance.levelID][2].ToString() + " Turns";
    }

    private void OnEnable()
    {
        if (GameStateManager.instance != null)
        {
            turnText.text = "Cleared in " + GameStateManager.instance.turn + " Turn" + (GameStateManager.instance.turn != 1 ? "s" : "");
            if (GameStateManager.instance.turn <= GameData.levelRankRequirements[GameStateManager.instance.levelID][0])
                rankBadge.color = new Color32(255, 189, 0, 255);
            else if (GameStateManager.instance.turn <= GameData.levelRankRequirements[GameStateManager.instance.levelID][1])
                rankBadge.color = new Color32(255, 255, 255, 255);
            else if (GameStateManager.instance.turn <= GameData.levelRankRequirements[GameStateManager.instance.levelID][2])
                rankBadge.color = new Color32(190, 105, 0, 255);
            else
                rankBadge.color = new Color32(120, 120, 120, 255);
            if (GameStateManager.instance.turn < GameData.bestLevelClear[GameStateManager.instance.levelID])
                GameData.bestLevelClear[GameStateManager.instance.levelID] = GameStateManager.instance.turn;
        }
    }

    public void Match()
    {
        AudioManager.instance.Play("Button");
        if (slot1.GetAllyInSlot() != null && slot2.GetAllyInSlot() != null)
        {
            AudioManager.instance.Stop("Battle Victory");
            AudioManager.instance.Play("Dialogue");
            Time.timeScale = 1f;
            CharacterControl.controls.Gameplay.Enable();
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

    public void Retry()
    {
        AudioManager.instance.Stop("Battle Victory");
        AudioManager.instance.Play("Button");
        SceneChanger.instance.LoadScene(SceneManager.GetActiveScene().name);
    }
}
