using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Helpers;

public class ActionUIManager : MonoBehaviour
{
    private PartyManager pm;
    public UIActionMode mode;
    [SerializeField] private Button heavyAttackBtn;
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
                            heavyAttackBtn.gameObject.SetActive(true);
                            healBtn.gameObject.SetActive(false);
                            stunBtn.gameObject.SetActive(false);
                            bombBtn.gameObject.SetActive(false);
                            break;
                        case AllyType.Strawberry:
                            heavyAttackBtn.gameObject.SetActive(false);
                            healBtn.gameObject.SetActive(true);
                            stunBtn.gameObject.SetActive(false);
                            bombBtn.gameObject.SetActive(false);
                            break;
                        case AllyType.Lemon:
                            heavyAttackBtn.gameObject.SetActive(false);
                            healBtn.gameObject.SetActive(false);
                            stunBtn.gameObject.SetActive(true);
                            bombBtn.gameObject.SetActive(false);
                            break;
                        case AllyType.Blueberry:
                            heavyAttackBtn.gameObject.SetActive(false);
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
                    heavyAttackBtn.gameObject.SetActive(false);
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
            case UIActionMode.HeavyAttack:
                heavyAttackBtn.gameObject.SetActive(true);
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
                heavyAttackBtn.gameObject.SetActive(false);
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
                heavyAttackBtn.gameObject.SetActive(false);
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
                heavyAttackBtn.gameObject.SetActive(false);
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
                heavyAttackBtn.gameObject.SetActive(false);
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
                heavyAttackBtn.gameObject.SetActive(false);
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
                heavyAttackBtn.gameObject.SetActive(false);
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
                heavyAttackBtn.gameObject.SetActive(false);
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
                heavyAttackBtn.gameObject.SetActive(false);
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
                heavyAttackBtn.gameObject.SetActive(false);
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
            case UIActionMode.HeavyAttack:
                //
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
    }

    public void ActionPressed(string btnType)
    {
        UIActionMode btnActionMode = UIActionMode.None;
        switch (btnType.ToLower().Trim())
        {
            case "heavy attack":
            case "heavyattack":
            case "heavyatk":
            case "heavy atk":
            case "apple":
                btnActionMode = UIActionMode.HeavyAttack;
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
    HeavyAttack,
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
