using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Helpers;

public class ActionUIManager : MonoBehaviour
{
    private PartyManager pm;
    public UIActionMode mode;
    [SerializeField] private Button commandBtn;
    [SerializeField] private Button healBtn;
    [SerializeField] private Button stunBtn;
    [SerializeField] private Button bombBtn;
    [SerializeField] private Button moveBtn;
    [SerializeField] private Button attackBtn;
    [SerializeField] private Button swapBtn;
    [SerializeField] private Button readBtn;
    [SerializeField] private Button detonateBtn;
    private void Awake()
    {
        pm = FindObjectOfType<PartyManager>();
    }

    private void Start()
    {
        UpdateActionUI(UIActionMode.None);
    }

    public void UpdateActionUI(UIActionMode newMode)
    {
        switch (newMode)
        {
            case UIActionMode.None:
                Ally leader = pm.allies[0];
                if (leader.Health > 0)
                {
                    switch (leader.type)
                    {
                        case AllyType.Apple:
                            commandBtn.gameObject.SetActive(true);
                            healBtn.gameObject.SetActive(false);
                            stunBtn.gameObject.SetActive(false);
                            bombBtn.gameObject.SetActive(false);
                            break;
                        case AllyType.Strawberry:
                            commandBtn.gameObject.SetActive(false);
                            healBtn.gameObject.SetActive(true);
                            stunBtn.gameObject.SetActive(false);
                            bombBtn.gameObject.SetActive(false);
                            break;
                        case AllyType.Lemon:
                            commandBtn.gameObject.SetActive(false);
                            healBtn.gameObject.SetActive(false);
                            stunBtn.gameObject.SetActive(true);
                            bombBtn.gameObject.SetActive(false);
                            break;
                        case AllyType.Blueberry:
                            commandBtn.gameObject.SetActive(false);
                            healBtn.gameObject.SetActive(false);
                            stunBtn.gameObject.SetActive(false);
                            bombBtn.gameObject.SetActive(true);
                            break;
                    }
                    moveBtn.gameObject.SetActive(true);
                    attackBtn.gameObject.SetActive(true);
                    swapBtn.gameObject.SetActive(true);
                    readBtn.gameObject.SetActive(false); // for now just always disable read btn since signs are not added yet
                    if (FindObjectsOfType<Bomb>().Length > 0)
                    {
                        detonateBtn.gameObject.SetActive(true);
                    } 
                    else
                    {
                        detonateBtn.gameObject.SetActive(false);
                    }
                }
                else
                {
                    commandBtn.gameObject.SetActive(false);
                    healBtn.gameObject.SetActive(false);
                    stunBtn.gameObject.SetActive(false);
                    bombBtn.gameObject.SetActive(false);
                    moveBtn.gameObject.SetActive(false);
                    attackBtn.gameObject.SetActive(false);
                    swapBtn.gameObject.SetActive(false);
                    readBtn.gameObject.SetActive(false);
                    detonateBtn.gameObject.SetActive(false);
                }
                break;
            case UIActionMode.Command:
                commandBtn.gameObject.SetActive(true);
                healBtn.gameObject.SetActive(false);
                stunBtn.gameObject.SetActive(false);
                bombBtn.gameObject.SetActive(false);
                moveBtn.gameObject.SetActive(false);
                attackBtn.gameObject.SetActive(false);
                swapBtn.gameObject.SetActive(false);
                readBtn.gameObject.SetActive(false);
                detonateBtn.gameObject.SetActive(false);
                break;
            case UIActionMode.Heal:
                commandBtn.gameObject.SetActive(false);
                healBtn.gameObject.SetActive(true);
                stunBtn.gameObject.SetActive(false);
                bombBtn.gameObject.SetActive(false);
                moveBtn.gameObject.SetActive(false);
                attackBtn.gameObject.SetActive(false);
                swapBtn.gameObject.SetActive(false);
                readBtn.gameObject.SetActive(false);
                detonateBtn.gameObject.SetActive(false);
                break;
            case UIActionMode.Stun:
                commandBtn.gameObject.SetActive(false);
                healBtn.gameObject.SetActive(false);
                stunBtn.gameObject.SetActive(true);
                bombBtn.gameObject.SetActive(false);
                moveBtn.gameObject.SetActive(false);
                attackBtn.gameObject.SetActive(false);
                swapBtn.gameObject.SetActive(false);
                readBtn.gameObject.SetActive(false);
                detonateBtn.gameObject.SetActive(false);
                break;
            case UIActionMode.Bomb:
                commandBtn.gameObject.SetActive(false);
                healBtn.gameObject.SetActive(false);
                stunBtn.gameObject.SetActive(false);
                bombBtn.gameObject.SetActive(true);
                moveBtn.gameObject.SetActive(false);
                attackBtn.gameObject.SetActive(false);
                swapBtn.gameObject.SetActive(false);
                readBtn.gameObject.SetActive(false);
                detonateBtn.gameObject.SetActive(false);
                break;
            case UIActionMode.Move:
                commandBtn.gameObject.SetActive(false);
                healBtn.gameObject.SetActive(false);
                stunBtn.gameObject.SetActive(false);
                bombBtn.gameObject.SetActive(false);
                moveBtn.gameObject.SetActive(true);
                attackBtn.gameObject.SetActive(false);
                swapBtn.gameObject.SetActive(false);
                readBtn.gameObject.SetActive(false);
                detonateBtn.gameObject.SetActive(false);
                break;
            case UIActionMode.Attack:
                commandBtn.gameObject.SetActive(false);
                healBtn.gameObject.SetActive(false);
                stunBtn.gameObject.SetActive(false);
                bombBtn.gameObject.SetActive(false);
                moveBtn.gameObject.SetActive(false);
                attackBtn.gameObject.SetActive(true);
                swapBtn.gameObject.SetActive(false);
                readBtn.gameObject.SetActive(false);
                detonateBtn.gameObject.SetActive(false);
                break;
            case UIActionMode.Swap:
                commandBtn.gameObject.SetActive(false);
                healBtn.gameObject.SetActive(false);
                stunBtn.gameObject.SetActive(false);
                bombBtn.gameObject.SetActive(false);
                moveBtn.gameObject.SetActive(false);
                attackBtn.gameObject.SetActive(false);
                swapBtn.gameObject.SetActive(true);
                readBtn.gameObject.SetActive(false);
                detonateBtn.gameObject.SetActive(false);
                break;
            case UIActionMode.Rest:
                commandBtn.gameObject.SetActive(false);
                healBtn.gameObject.SetActive(false);
                stunBtn.gameObject.SetActive(false);
                bombBtn.gameObject.SetActive(false);
                moveBtn.gameObject.SetActive(false);
                attackBtn.gameObject.SetActive(false);
                swapBtn.gameObject.SetActive(false);
                readBtn.gameObject.SetActive(false);
                detonateBtn.gameObject.SetActive(false);
                break;
            case UIActionMode.Read:
                commandBtn.gameObject.SetActive(false);
                healBtn.gameObject.SetActive(false);
                stunBtn.gameObject.SetActive(false);
                bombBtn.gameObject.SetActive(false);
                moveBtn.gameObject.SetActive(false);
                attackBtn.gameObject.SetActive(false);
                swapBtn.gameObject.SetActive(false);
                readBtn.gameObject.SetActive(true);
                detonateBtn.gameObject.SetActive(false);
                break;
            case UIActionMode.Detonate:
                commandBtn.gameObject.SetActive(false);
                healBtn.gameObject.SetActive(false);
                stunBtn.gameObject.SetActive(false);
                bombBtn.gameObject.SetActive(false);
                moveBtn.gameObject.SetActive(false);
                attackBtn.gameObject.SetActive(false);
                swapBtn.gameObject.SetActive(false);
                readBtn.gameObject.SetActive(false);
                detonateBtn.gameObject.SetActive(true);
                break;
        }
        UpdateActionControls(newMode);
    }

    public void UpdateActionControls(UIActionMode newMode)
    {
        mode = newMode;
        switch (newMode)
        {
            case UIActionMode.None:
                // 
                break;
            case UIActionMode.Command:
                pm.AttemptCommand();
                break;
            case UIActionMode.Heal:
                //
                break;
            case UIActionMode.Stun:
                //
                break;
            case UIActionMode.Bomb:
                //
                break;
            case UIActionMode.Move:
                //
                break;
            case UIActionMode.Attack:
                //
                break;
            case UIActionMode.Swap:
                pm.AttemptSwap(Vec3ToVec2(pm.allies[1].transform.position - pm.allies[0].transform.position));
                break;
            case UIActionMode.Rest:
                //
                break;
            case UIActionMode.Read:
                //
                break;
            case UIActionMode.Detonate:
                pm.AttemptDetonate();
                break;
        }
        UpdateOverlayControls();
    }

    public void UpdateOverlayControls()
    {
        switch (mode)
        {
            case UIActionMode.None:
                FindObjectOfType<CursorTileDisplay>().ClearOverlay();
                break;
            case UIActionMode.Command:
                
                break;
            case UIActionMode.Heal:
                FindObjectOfType<CursorTileDisplay>().SetHealOverlay();
                break;
            case UIActionMode.Stun:
                FindObjectOfType<CursorTileDisplay>().SetStunOverlay();
                break;
            case UIActionMode.Bomb:
                FindObjectOfType<CursorTileDisplay>().SetBombOverlay();
                break;
            case UIActionMode.Move:
                FindObjectOfType<CursorTileDisplay>().SetMoveOverlay();
                break;
            case UIActionMode.Attack:
                FindObjectOfType<CursorTileDisplay>().SetAttackOverlay();
                break;
            case UIActionMode.Swap:
                
                break;
            case UIActionMode.Rest:
                //
                break;
            case UIActionMode.Read:
                //
                break;
            case UIActionMode.Detonate:
                
                break;
        }
    }

    public void ActionPressed(string btnType)
    {
        AudioManager.instance.Play("Button");
        UIActionMode btnActionMode = UIActionMode.None;
        switch (btnType.ToLower().Trim())
        {
            case "heavy attack":
            case "heavyattack":
            case "heavyatk":
            case "heavy atk":
            case "apple":
            case "command":
            case "cmd":
                btnActionMode = UIActionMode.Command;
                break;
            case "heal":
            case "strawberry":
            case "strawb":
                btnActionMode = UIActionMode.Heal;
                break;
            case "stun":
            case "lemon":
                btnActionMode = UIActionMode.Stun;
                break;
            case "bomb":
            case "blueberry":
            case "blue berry":
            case "blueb":
                btnActionMode = UIActionMode.Bomb;
                break;
            case "move":
                btnActionMode = UIActionMode.Move;
                break;
            case "attack":
            case "atk":
                btnActionMode = UIActionMode.Attack;
                break;
            case "swap":
                btnActionMode = UIActionMode.Swap;
                break;
            case "rest":
            case "picnic":
            case "blanket":
            case "fire":
            case "campfire":
            case "bonfire":
            case "bench":
                btnActionMode = UIActionMode.Rest;
                break;
            case "read":
                btnActionMode = UIActionMode.Read;
                break;
            case "detonate":
                btnActionMode = UIActionMode.Detonate;
                break;
            default:
                Debug.LogError("Invalid Button Type Passed Into Action Pressed");
                break;
        }
        if (mode != btnActionMode)
            UpdateActionUI(btnActionMode);
        else
            UpdateActionUI(UIActionMode.None);
    }
}

public enum UIActionMode
{
    None,
    Command,
    Heal,
    Stun,
    Bomb,
    Move,
    Attack,
    Swap,
    Rest,
    Read,
    Detonate
}
