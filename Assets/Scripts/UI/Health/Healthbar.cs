using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : Valuebar
{
    [SerializeField] private Slider temporaryHealth;
    [HideInInspector] public float temporaryCumulativeDamage = 0f;
    private float realHealthVal;

    public void SetMaxHealth(float maxHealth)
    {
        temporaryHealth.gameObject.SetActive(false);
        SetMaxValue(maxHealth);
        string tip = GetComponent<HoverTip>().tipToShow;
        string[] splitTip = tip.Split('/');
        splitTip[1] = Mathf.CeilToInt(maxHealth).ToString();
        GetComponent<HoverTip>().tipToShow = splitTip[0] + "/" + splitTip[1];
    }

    public void SetHealth(float health)
    {
        temporaryHealth.gameObject.SetActive(false);
        realHealthVal = health;
        SetValue(health);
        string tip = GetComponent<HoverTip>().tipToShow;
        string[] splitTip = tip.Split(new char[] { ':', '/' } );
        splitTip[1] = " " + Mathf.CeilToInt(health).ToString();
        GetComponent<HoverTip>().tipToShow = splitTip[0] + ":" + splitTip[1] + "/" + splitTip[2];
        SetTemporaryDamage(0);
    }

    public void SetTemporaryDamage(float damage)
    {
        temporaryHealth.gameObject.SetActive(true);
        temporaryHealth.maxValue = slider.maxValue;
        temporaryHealth.value = slider.value;
        slider.value = Mathf.Max(0, temporaryHealth.value - damage);
    }

    public void SetTemporaryHeal(float heal)
    {
        temporaryHealth.gameObject.SetActive(true);
        temporaryHealth.maxValue = slider.maxValue;
        temporaryHealth.value = Mathf.Min(slider.maxValue, slider.value + heal);
    }

    public void EnableTemporaryCumulativeDamage()
    {
        if (temporaryCumulativeDamage > 0)
        {
            gameObject.SetActive(true);
            temporaryHealth.gameObject.SetActive(true);
            temporaryHealth.maxValue = slider.maxValue;
            temporaryHealth.value = slider.value;
            slider.value = Mathf.Max(0, temporaryHealth.value - temporaryCumulativeDamage);
        }
    }

    public void DisableTemporaryHeal()
    {
        temporaryHealth.gameObject.SetActive(false);
    }

    public void DisableTemporaryDamage()
    {
        temporaryCumulativeDamage = 0;
        slider.value = realHealthVal;
        temporaryHealth.gameObject.SetActive(false);
    }
}
