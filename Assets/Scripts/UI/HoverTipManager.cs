using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class HoverTipManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tipText;
    [SerializeField] private RectTransform tipWindow;

    public static Action<string, Vector2> OnMouseHover;
    public static Action OnMouseLoseFocus;

    private void OnEnable()
    {
        OnMouseHover += ShowTip;
        OnMouseLoseFocus += HideTip;
    }

    private void OnDisable()
    {
        OnMouseHover -= ShowTip;
        OnMouseLoseFocus -= HideTip;
    }

    private void Awake()
    {
        if (tipText == null || tipWindow == null)
            Debug.LogError("Tooltip needs to be assigned in inspector");
    }

    void Start()
    {
        HideTip();
    }

    private void ShowTip(string tip, Vector2 mousePos)
    {
        tipText.text = tip;
        tipWindow.sizeDelta = new Vector2(tipText.preferredWidth > 615 ? 615 : tipText.preferredWidth, tipText.preferredHeight);

        tipWindow.gameObject.SetActive(true);
        float windowX;
        float windowY;
        if (mousePos.x + tipWindow.sizeDelta.x >= Screen.width)
            windowX = mousePos.x - (tipWindow.sizeDelta.x / 2);
        else
            windowX = mousePos.x + (tipWindow.sizeDelta.x / 2);
        if (mousePos.y - tipWindow.sizeDelta.y < 0)
            windowY = mousePos.y + (tipWindow.sizeDelta.y / 2);
        else
            windowY = mousePos.y - (tipWindow.sizeDelta.y / 2);
        tipWindow.transform.position = new Vector2(windowX, windowY);
    }

    private void HideTip()
    {
        tipText.text = default;
        tipWindow.gameObject.SetActive(false);
    }
}
