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
        slider.GetComponent<RectTransform>().sizeDelta = new Vector2(maxValue * barSegmentWidth, barSegmentHeight);
        slider.maxValue = maxValue;
        for (int i = 0; i < Mathf.CeilToInt(maxValue); i++)
        {
            Instantiate(barSegment, barSegments);
        }
    }

    protected void SetValue(float value)
    {
        slider.value = value;
    }
}
