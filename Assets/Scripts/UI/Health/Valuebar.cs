using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Valuebar : MonoBehaviour
{
    protected Slider slider;
    protected float barSegmentWidth = 20;
    protected float barSegmentHeight = 30;
    protected GameObject barSegment;
    [SerializeField] protected RectTransform barSegments;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        barSegment = Resources.Load("Prefabs/UI/BarSegment") as GameObject;
    }

    protected void SetMaxValue(float maxValue)
    {
        foreach (Transform child in barSegments)
        {
            Destroy(child.gameObject);
        }
        slider.maxValue = maxValue;
        slider.GetComponent<RectTransform>().sizeDelta = new Vector2(maxValue * barSegmentWidth, barSegmentHeight);
        Debug.Log("Max Health = " + Mathf.CeilToInt(maxValue));
        for (int i = 0; i < Mathf.CeilToInt(maxValue); i++)
        {
            Debug.Log("   Instatiate " + (i + 1));
            Instantiate(barSegment, barSegments);
        }
    }

    protected void SetValue(float value)
    {
        slider.value = value;
    }
}