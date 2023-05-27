// Credit: https://www.youtube.com/watch?v=BGr-7GZJNXg
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    private RectTransform thisTransform;
    [SerializeField] private RectTransform partyTransform;
    [SerializeField] private RectTransform windowTransform;
    private CanvasGroup canvasGroup;
    public AllyType allyType;
    private void Awake()
    {
        thisTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (thisTransform.parent != partyTransform)
            thisTransform.parent.GetComponent<DropSlot>().isFilled = false;
        thisTransform.SetParent(windowTransform, true);
        canvasGroup.alpha = 0.5f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        thisTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (thisTransform.parent == windowTransform)
            thisTransform.SetParent(partyTransform, true);
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }
}
