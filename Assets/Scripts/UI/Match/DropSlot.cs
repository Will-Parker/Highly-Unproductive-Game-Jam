// Credit: https://www.youtube.com/watch?v=BGr-7GZJNXg
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    [HideInInspector] public bool isFilled = false;
    private AllyType allyInSlot;
    [SerializeField] private RectTransform partyTransform;
    private RectTransform thisTransform;
    private void Awake()
    {
        thisTransform = GetComponent<RectTransform>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            GameObject dragObject = eventData.pointerDrag;
            RectTransform dragObjectTransform = eventData.pointerDrag.GetComponent<RectTransform>();
            if (isFilled)
            {
                thisTransform.GetChild(0).GetComponent<RectTransform>().SetParent(partyTransform, true);
            }
            dragObjectTransform.localPosition = thisTransform.localPosition;
            dragObjectTransform.SetParent(thisTransform, true);
            isFilled = true;
            allyInSlot = dragObject.GetComponent<DragDrop>().allyType;
        } 
        else
        {
            isFilled = false;
        }
    }

    public AllyType? GetAllyInSlot()
    {
        if (isFilled)
            return allyInSlot;
        else
            return null;
    }
}
