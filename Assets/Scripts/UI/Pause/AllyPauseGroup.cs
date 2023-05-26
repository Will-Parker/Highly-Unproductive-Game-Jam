using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AllyPauseGroup : MonoBehaviour
{
    private PartyManager pm;
    public int id;
    [SerializeField] private TextMeshProUGUI healthValue;
    [SerializeField] private TextMeshProUGUI attackValue;
    [SerializeField] private TextMeshProUGUI uniqueValue;
    [SerializeField] private TextMeshProUGUI healthSum;
    [SerializeField] private TextMeshProUGUI attackSum;
    [SerializeField] private TextMeshProUGUI uniqueSum;
    private void Awake()
    {
        pm = FindObjectOfType<PartyManager>();
    }

    public void SetText()
    {
        Ally thisAlly = pm.allies[id];
        Ally upperNeighbor;
        
        Ally lowerNeighbor;
        if (id == 0)
        {
            upperNeighbor = pm.allies[3];
            lowerNeighbor = pm.allies[1];
        }
        else if (id == 3)
        {
            upperNeighbor = pm.allies[2];
            lowerNeighbor = pm.allies[0];
        }
        else
        {
            upperNeighbor = pm.allies[id - 1];
            lowerNeighbor = pm.allies[id + 1];
        }
        int healthStat = GetStatTotal(StatType.MaxHealth, thisAlly, upperNeighbor, lowerNeighbor);
        int attackStat = GetStatTotal(StatType.Attack, thisAlly, upperNeighbor, lowerNeighbor);
        StatType uniqueStatType = GetUniqueStatType(thisAlly);
        int uniqueStat = GetStatTotal(uniqueStatType, thisAlly, upperNeighbor, lowerNeighbor);

        string thisCT = GetColorTag(thisAlly);
        string upNeighborCT = GetColorTag(upperNeighbor);
        string loNeighborCT = GetColorTag(lowerNeighbor);
        string closeCT = "</color>";

        healthValue.text = healthStat.ToString();
        healthSum.text = "(" + thisCT + Mathf.FloorToInt(thisAlly.MaxHealth).ToString() + closeCT 
            + "+" + upNeighborCT + Mathf.FloorToInt(thisAlly.partnerBuffs[upperNeighbor.type][StatType.MaxHealth]).ToString() + closeCT
            + "+" + loNeighborCT + Mathf.FloorToInt(thisAlly.partnerBuffs[lowerNeighbor.type][StatType.MaxHealth]).ToString() + closeCT + ")";
        attackValue.text = attackStat.ToString();
        attackSum.text = "(" + thisCT + Mathf.FloorToInt(thisAlly.Attack).ToString() + closeCT
            + "+" + upNeighborCT + Mathf.FloorToInt(thisAlly.partnerBuffs[upperNeighbor.type][StatType.Attack]).ToString() + closeCT
            + "+" + loNeighborCT + Mathf.FloorToInt(thisAlly.partnerBuffs[lowerNeighbor.type][StatType.Attack]).ToString() + closeCT + ")";
        uniqueValue.text = uniqueStat.ToString();
        string uniqueSumText = "";
        switch (thisAlly.type)
        {
            case AllyType.Apple:
                uniqueSumText = "(" + thisCT + Mathf.FloorToInt(thisAlly.HeavyAttackStat).ToString() + closeCT;
                break;
            case AllyType.Strawberry:
                uniqueSumText = "(" + thisCT + Mathf.FloorToInt(thisAlly.HealStat).ToString() + closeCT;
                break;
            case AllyType.Lemon:
                uniqueSumText = "(" + thisCT + Mathf.FloorToInt(thisAlly.StunStat).ToString() + closeCT;
                break;
            case AllyType.Blueberry:
                uniqueSumText = "(" + thisCT + Mathf.FloorToInt(thisAlly.BombStat).ToString() + closeCT;
                break;
        }
        uniqueSumText += "+" + upNeighborCT + Mathf.FloorToInt(thisAlly.partnerBuffs[upperNeighbor.type][StatType.Attack]).ToString() + closeCT
                       + "+" + loNeighborCT + Mathf.FloorToInt(thisAlly.partnerBuffs[lowerNeighbor.type][StatType.Attack]).ToString() + closeCT + ")";
        uniqueSum.text = uniqueSumText;
    }

    private string GetColorTag(Ally ally)
    {
        switch (ally.type)
        {
            case AllyType.Apple:
                return "<color=#73FF5A>";
            case AllyType.Strawberry:
                return "<color=#FA5961>";
            case AllyType.Lemon:
                return "<color=#FFE25A>";
            case AllyType.Blueberry:
                return "<color=#5AD4FF>";
        }
        return null;
    }
    private int GetStatTotal(StatType statType, Ally thisAlly, Ally upperNeighbor, Ally lowerNeighbor)
    {
        float runningTotal = 0f;
        switch (statType)
        {
            case StatType.MaxHealth:
                runningTotal += thisAlly.MaxHealth;
                break;
            case StatType.Attack:
                runningTotal += thisAlly.Attack;
                break;
            case StatType.HeavyAttack:
                runningTotal += thisAlly.HeavyAttackStat;
                break;
            case StatType.Heal:
                runningTotal += thisAlly.HealStat;
                break;
            case StatType.Stun:
                runningTotal += thisAlly.StunStat;
                break;
            case StatType.Bomb:
                runningTotal += thisAlly.BombStat;
                break;
        }
        runningTotal += thisAlly.partnerBuffs[upperNeighbor.type][statType] + thisAlly.partnerBuffs[lowerNeighbor.type][statType];
        return Mathf.FloorToInt(runningTotal);
    }
    private StatType GetUniqueStatType(Ally ally)
    {
        switch (ally.type)
        {
            case AllyType.Apple:
                return StatType.HeavyAttack;
            case AllyType.Strawberry:
                return StatType.Heal;
            case AllyType.Lemon:
                return StatType.Stun;
            case AllyType.Blueberry:
                return StatType.Bomb;
            default:
                return StatType.MaxHealth; // this should never happen
        }
    }
}
