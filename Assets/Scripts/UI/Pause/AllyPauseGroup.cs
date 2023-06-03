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
        int uniqueStat = GetStatTotal(StatType.Unique, thisAlly, upperNeighbor, lowerNeighbor);

        healthValue.text = healthStat.ToString();
        healthSum.text = GetStatString(StatType.MaxHealth, thisAlly, upperNeighbor, lowerNeighbor);
        attackValue.text = attackStat.ToString();
        attackSum.text = GetStatString(StatType.Attack, thisAlly, upperNeighbor, lowerNeighbor);
        uniqueValue.text = uniqueStat.ToString();
        uniqueSum.text = GetStatString(StatType.Unique, thisAlly, upperNeighbor, lowerNeighbor);
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
        return Mathf.FloorToInt(GameData.GetStatSum(thisAlly.type, upperNeighbor.type, lowerNeighbor.type, statType));
    }

    private string GetStatString(StatType statType, Ally thisAlly, Ally upperNeighbor, Ally lowerNeighbor)
    {
        string thisCT = GetColorTag(thisAlly);
        string upNeighborCT = GetColorTag(upperNeighbor);
        string loNeighborCT = GetColorTag(lowerNeighbor);
        string closeCT = "</color>";
        return "(" + thisCT + Mathf.FloorToInt(GameData.allyStats[thisAlly.type][thisAlly.type][statType]).ToString() + closeCT
            + (GameData.allyStats[thisAlly.type][upperNeighbor.type][StatType.MaxHealth] > 0
            ? "+" + upNeighborCT + Mathf.FloorToInt(GameData.allyStats[thisAlly.type][upperNeighbor.type][statType]).ToString() + closeCT : "")
            + (GameData.allyStats[thisAlly.type][lowerNeighbor.type][StatType.MaxHealth] > 0
            ? "+" + loNeighborCT + Mathf.FloorToInt(GameData.allyStats[thisAlly.type][lowerNeighbor.type][statType]).ToString() + closeCT : "")
            + ")";
    }
}
