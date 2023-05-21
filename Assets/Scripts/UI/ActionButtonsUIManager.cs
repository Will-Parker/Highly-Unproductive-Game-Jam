using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionUIManager : MonoBehaviour
{
    private PartyManager pm;
    private UIActionMode mode;
    private void Awake()
    {
        pm = FindObjectOfType<PartyManager>();
    }

    private void Start()
    {
        mode = UIActionMode.None;
    }

    public void UpdateActionUI(UIActionMode newMode)
    {
        Ally leader = pm.allies[0];
        switch (leader.type)
        {
            case AllyType.Apple:
                break;
        }
    }

    public void AttackPressed()
    {
        if (mode != UIActionMode.Attack)
            EnterAttackView();
        else
            EnterRegularView();
    }

    public void HealPressed()
    {

    }

    public void StunPressed()
    {

    }

    public void BombPressed()
    {

    }

    public void MovePressed()
    {

    }

    public void SwapPressed()
    {

    }

    public void ReadPressed()
    {

    }

    public void RestPressed()
    {

    }

    private void EnterAttackView()
    {

    }

    private void EnterRegularView()
    {

    }
}

public enum UIActionMode
{
    None,
    Attack,
    Heal,
    Stun,
    Bomb,
    Move,
    Swap,
    Read,
    Rest
}
