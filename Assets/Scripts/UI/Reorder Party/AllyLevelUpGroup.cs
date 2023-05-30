using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AllyLevelUpGroup : MonoBehaviour
{
    private PartyManager pm;
    public AllyType allyType;
    public int id;
    public int oldId;
    public bool isLevelUp;
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

    private void Update()
    {
        id = transform.GetSiblingIndex();
        if (oldId != id)
        {
            FindObjectOfType<ReorderPartyManager>().UpdateAllAllyText();
            oldId = id;
        }
    }

    public void InitializeText()
    {
        oldId = id;
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

        string newHealthSumText = "(" + thisCT + Mathf.FloorToInt(thisAlly.MaxHealth).ToString() + closeCT
                + "+" + upNeighborCT + Mathf.FloorToInt(thisAlly.partnerBuffs[upperNeighbor.type][StatType.MaxHealth]).ToString() + closeCT
                + "+" + loNeighborCT + Mathf.FloorToInt(thisAlly.partnerBuffs[lowerNeighbor.type][StatType.MaxHealth]).ToString() + closeCT + ")";
        string newAttackSumText = "(" + thisCT + Mathf.FloorToInt(thisAlly.Attack).ToString() + closeCT
                + "+" + upNeighborCT + Mathf.FloorToInt(thisAlly.partnerBuffs[upperNeighbor.type][StatType.Attack]).ToString() + closeCT
                + "+" + loNeighborCT + Mathf.FloorToInt(thisAlly.partnerBuffs[lowerNeighbor.type][StatType.Attack]).ToString() + closeCT + ")";
        string newUniqueSumText = "";
        switch (thisAlly.type)
        {
            case AllyType.Apple:
                newUniqueSumText = "(" + thisCT + Mathf.FloorToInt(thisAlly.HeavyAttackStat).ToString() + closeCT;
                break;
            case AllyType.Strawberry:
                newUniqueSumText = "(" + thisCT + Mathf.FloorToInt(thisAlly.HealStat).ToString() + closeCT;
                break;
            case AllyType.Lemon:
                newUniqueSumText = "(" + thisCT + Mathf.FloorToInt(thisAlly.StunStat).ToString() + closeCT;
                break;
            case AllyType.Blueberry:
                newUniqueSumText = "(" + thisCT + Mathf.FloorToInt(thisAlly.BombStat).ToString() + closeCT;
                break;
        }
        newUniqueSumText += "+" + upNeighborCT + Mathf.FloorToInt(thisAlly.partnerBuffs[upperNeighbor.type][uniqueStatType]).ToString() + closeCT
                          + "+" + loNeighborCT + Mathf.FloorToInt(thisAlly.partnerBuffs[lowerNeighbor.type][uniqueStatType]).ToString() + closeCT + ")";

        if (isLevelUp)
        {
            int oldHealthStat = GetOldStatTotal(StatType.MaxHealth, thisAlly, upperNeighbor, lowerNeighbor);
            int oldAttackStat = GetOldStatTotal(StatType.Attack, thisAlly, upperNeighbor, lowerNeighbor);
            int oldUniqueStat = GetOldStatTotal(uniqueStatType, thisAlly, upperNeighbor, lowerNeighbor);

            healthValue.text = oldHealthStat.ToString() + " >> " + healthStat.ToString();
            healthSum.text = "(" + thisCT + Mathf.FloorToInt(thisAlly.MaxHealth).ToString() + closeCT
                + "+" + upNeighborCT + Mathf.FloorToInt(thisAlly.oldPartnerBuffs[upperNeighbor.type][StatType.MaxHealth]).ToString() + closeCT
                + "+" + loNeighborCT + Mathf.FloorToInt(thisAlly.oldPartnerBuffs[lowerNeighbor.type][StatType.MaxHealth]).ToString() + closeCT + ")"
                + " >> " + newHealthSumText;
            attackValue.text = oldAttackStat.ToString() + " >> " + attackStat.ToString();
            attackSum.text = "(" + thisCT + Mathf.FloorToInt(thisAlly.Attack).ToString() + closeCT
                + "+" + upNeighborCT + Mathf.FloorToInt(thisAlly.oldPartnerBuffs[upperNeighbor.type][StatType.Attack]).ToString() + closeCT
                + "+" + loNeighborCT + Mathf.FloorToInt(thisAlly.oldPartnerBuffs[lowerNeighbor.type][StatType.Attack]).ToString() + closeCT + ")"
                + " >> " + newAttackSumText;
            uniqueValue.text = oldUniqueStat.ToString() + " >> " + uniqueStat.ToString();
            string oldUniqueSumText = "";
            switch (thisAlly.type)
            {
                case AllyType.Apple:
                    oldUniqueSumText = "(" + thisCT + Mathf.FloorToInt(thisAlly.HeavyAttackStat).ToString() + closeCT;
                    break;
                case AllyType.Strawberry:
                    oldUniqueSumText = "(" + thisCT + Mathf.FloorToInt(thisAlly.HealStat).ToString() + closeCT;
                    break;
                case AllyType.Lemon:
                    oldUniqueSumText = "(" + thisCT + Mathf.FloorToInt(thisAlly.StunStat).ToString() + closeCT;
                    break;
                case AllyType.Blueberry:
                    oldUniqueSumText = "(" + thisCT + Mathf.FloorToInt(thisAlly.BombStat).ToString() + closeCT;
                    break;
            }
            oldUniqueSumText += "+" + upNeighborCT + Mathf.FloorToInt(thisAlly.oldPartnerBuffs[upperNeighbor.type][uniqueStatType]).ToString() + closeCT
                              + "+" + loNeighborCT + Mathf.FloorToInt(thisAlly.oldPartnerBuffs[lowerNeighbor.type][uniqueStatType]).ToString() + closeCT + ")";
            uniqueSum.text = oldUniqueSumText + " >> " + newUniqueSumText;
        }
        else
        {
            healthValue.text = healthStat.ToString();
            healthSum.text = newHealthSumText;
            attackValue.text = attackStat.ToString();
            attackSum.text = newAttackSumText;
            uniqueValue.text = uniqueStat.ToString();
            uniqueSum.text = newUniqueSumText;
        }
    }

    public void SetText()
    {
        Ally thisAlly = pm.GetAlly(allyType);
        Ally upperNeighbor;
        Ally lowerNeighbor;

        if (id == 0)
        {
            upperNeighbor = pm.GetAlly(transform.parent.GetChild(3).GetComponent<AllyLevelUpGroup>().allyType);
            lowerNeighbor = pm.GetAlly(transform.parent.GetChild(1).GetComponent<AllyLevelUpGroup>().allyType);
        }
        else if (id == 3)
        {
            upperNeighbor = pm.GetAlly(transform.parent.GetChild(2).GetComponent<AllyLevelUpGroup>().allyType);
            lowerNeighbor = pm.GetAlly(transform.parent.GetChild(0).GetComponent<AllyLevelUpGroup>().allyType);
        }
        else
        {
            upperNeighbor = pm.GetAlly(transform.parent.GetChild(id - 1).GetComponent<AllyLevelUpGroup>().allyType);
            lowerNeighbor = pm.GetAlly(transform.parent.GetChild(id + 1).GetComponent<AllyLevelUpGroup>().allyType);
        }
        int healthStat = GetStatTotal(StatType.MaxHealth, thisAlly, upperNeighbor, lowerNeighbor);
        int attackStat = GetStatTotal(StatType.Attack, thisAlly, upperNeighbor, lowerNeighbor);
        StatType uniqueStatType = GetUniqueStatType(thisAlly);
        int uniqueStat = GetStatTotal(uniqueStatType, thisAlly, upperNeighbor, lowerNeighbor);

        string thisCT = GetColorTag(thisAlly);
        string upNeighborCT = GetColorTag(upperNeighbor);
        string loNeighborCT = GetColorTag(lowerNeighbor);
        string closeCT = "</color>";

        string newHealthSumText = "(" + thisCT + Mathf.FloorToInt(thisAlly.MaxHealth).ToString() + closeCT
                + "+" + upNeighborCT + Mathf.FloorToInt(thisAlly.partnerBuffs[upperNeighbor.type][StatType.MaxHealth]).ToString() + closeCT
                + "+" + loNeighborCT + Mathf.FloorToInt(thisAlly.partnerBuffs[lowerNeighbor.type][StatType.MaxHealth]).ToString() + closeCT + ")";
        string newAttackSumText = "(" + thisCT + Mathf.FloorToInt(thisAlly.Attack).ToString() + closeCT
                + "+" + upNeighborCT + Mathf.FloorToInt(thisAlly.partnerBuffs[upperNeighbor.type][StatType.Attack]).ToString() + closeCT
                + "+" + loNeighborCT + Mathf.FloorToInt(thisAlly.partnerBuffs[lowerNeighbor.type][StatType.Attack]).ToString() + closeCT + ")";
        string newUniqueSumText = "";
        switch (thisAlly.type)
        {
            case AllyType.Apple:
                newUniqueSumText = "(" + thisCT + Mathf.FloorToInt(thisAlly.HeavyAttackStat).ToString() + closeCT;
                break;
            case AllyType.Strawberry:
                newUniqueSumText = "(" + thisCT + Mathf.FloorToInt(thisAlly.HealStat).ToString() + closeCT;
                break;
            case AllyType.Lemon:
                newUniqueSumText = "(" + thisCT + Mathf.FloorToInt(thisAlly.StunStat).ToString() + closeCT;
                break;
            case AllyType.Blueberry:
                newUniqueSumText = "(" + thisCT + Mathf.FloorToInt(thisAlly.BombStat).ToString() + closeCT;
                break;
        }
        newUniqueSumText += "+" + upNeighborCT + Mathf.FloorToInt(thisAlly.partnerBuffs[upperNeighbor.type][uniqueStatType]).ToString() + closeCT
                          + "+" + loNeighborCT + Mathf.FloorToInt(thisAlly.partnerBuffs[lowerNeighbor.type][uniqueStatType]).ToString() + closeCT + ")";

        if (isLevelUp)
        {
            int oldHealthStat = GetOldStatTotal(StatType.MaxHealth, thisAlly, upperNeighbor, lowerNeighbor);
            int oldAttackStat = GetOldStatTotal(StatType.Attack, thisAlly, upperNeighbor, lowerNeighbor);
            int oldUniqueStat = GetOldStatTotal(uniqueStatType, thisAlly, upperNeighbor, lowerNeighbor);

            healthValue.text = oldHealthStat.ToString() + " >> " + healthStat.ToString();
            healthSum.text = "(" + thisCT + Mathf.FloorToInt(thisAlly.MaxHealth).ToString() + closeCT
                + "+" + upNeighborCT + Mathf.FloorToInt(thisAlly.oldPartnerBuffs[upperNeighbor.type][StatType.MaxHealth]).ToString() + closeCT
                + "+" + loNeighborCT + Mathf.FloorToInt(thisAlly.oldPartnerBuffs[lowerNeighbor.type][StatType.MaxHealth]).ToString() + closeCT + ")"
                + " >> " + newHealthSumText;
            attackValue.text = oldAttackStat.ToString() + " >> " + attackStat.ToString();
            attackSum.text = "(" + thisCT + Mathf.FloorToInt(thisAlly.Attack).ToString() + closeCT
                + "+" + upNeighborCT + Mathf.FloorToInt(thisAlly.oldPartnerBuffs[upperNeighbor.type][StatType.Attack]).ToString() + closeCT
                + "+" + loNeighborCT + Mathf.FloorToInt(thisAlly.oldPartnerBuffs[lowerNeighbor.type][StatType.Attack]).ToString() + closeCT + ")"
                + " >> " + newAttackSumText;
            uniqueValue.text = oldUniqueStat.ToString() + " >> " + uniqueStat.ToString();
            string oldUniqueSumText = "";
            switch (thisAlly.type)
            {
                case AllyType.Apple:
                    oldUniqueSumText = "(" + thisCT + Mathf.FloorToInt(thisAlly.HeavyAttackStat).ToString() + closeCT;
                    break;
                case AllyType.Strawberry:
                    oldUniqueSumText = "(" + thisCT + Mathf.FloorToInt(thisAlly.HealStat).ToString() + closeCT;
                    break;
                case AllyType.Lemon:
                    oldUniqueSumText = "(" + thisCT + Mathf.FloorToInt(thisAlly.StunStat).ToString() + closeCT;
                    break;
                case AllyType.Blueberry:
                    oldUniqueSumText = "(" + thisCT + Mathf.FloorToInt(thisAlly.BombStat).ToString() + closeCT;
                    break;
            }
            oldUniqueSumText += "+" + upNeighborCT + Mathf.FloorToInt(thisAlly.oldPartnerBuffs[upperNeighbor.type][uniqueStatType]).ToString() + closeCT
                              + "+" + loNeighborCT + Mathf.FloorToInt(thisAlly.oldPartnerBuffs[lowerNeighbor.type][uniqueStatType]).ToString() + closeCT + ")";
            uniqueSum.text = oldUniqueSumText + " >> " + newUniqueSumText;
        }
        else
        {
            healthValue.text = healthStat.ToString();
            healthSum.text = newHealthSumText;
            attackValue.text = attackStat.ToString();
            attackSum.text = newAttackSumText;
            uniqueValue.text = uniqueStat.ToString();
            uniqueSum.text = newUniqueSumText;
        }
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

    private int GetOldStatTotal(StatType statType, Ally thisAlly, Ally upperNeighbor, Ally lowerNeighbor)
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
        runningTotal += thisAlly.oldPartnerBuffs[upperNeighbor.type][statType] + thisAlly.oldPartnerBuffs[lowerNeighbor.type][statType];
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
