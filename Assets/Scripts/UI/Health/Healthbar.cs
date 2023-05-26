using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void SetMaxHealth(float maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.GetComponent<RectTransform>().sizeDelta = new Vector2(maxHealth * 20, 30);
    }

    public void SetHealth(float health)
    {
        slider.value = health;
    }
}
