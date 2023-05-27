using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Experiencebar : Valuebar
{
    private float maxExperience;
    private float currExperience;
    [SerializeField] private Button restBtn;

    public void SetMaxExperience(float maxExperience)
    {
        SetMaxValue(maxExperience);
        this.maxExperience = maxExperience;
        UpdateTip();
    }

    public void SetExperience(float experience)
    {
        SetValue(experience);
        currExperience = experience;
        bool isFull = currExperience >= maxExperience;
        restBtn.gameObject.SetActive(isFull);
        UpdateTip();
    }

    private void UpdateTip()
    {
        string tip = GetComponent<HoverTip>().tipToShow;
        string[] splitTip = tip.Split(':');
        splitTip[1] = " " + Mathf.CeilToInt(maxExperience - currExperience).ToString();
        GetComponent<HoverTip>().tipToShow = splitTip[0] + ":" + splitTip[1];
    }
}
