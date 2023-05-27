using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : Valuebar
{
    public void SetMaxHealth(float maxHealth)
    {
        SetMaxValue(maxHealth);
        string tip = GetComponent<HoverTip>().tipToShow;
        string[] splitTip = tip.Split('/');
        splitTip[1] = Mathf.CeilToInt(maxHealth).ToString();
        GetComponent<HoverTip>().tipToShow = splitTip[0] + "/" + splitTip[1];
    }

    public void SetHealth(float health)
    {
        SetValue(health);
        string tip = GetComponent<HoverTip>().tipToShow;
        string[] splitTip = tip.Split(new char[] { ':', '/' } );
        splitTip[1] = " " + Mathf.CeilToInt(health).ToString();
        GetComponent<HoverTip>().tipToShow = splitTip[0] + ":" + splitTip[1] + "/" + splitTip[2];
    }
}
