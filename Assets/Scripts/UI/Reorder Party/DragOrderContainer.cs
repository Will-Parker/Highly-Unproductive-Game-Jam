//credit: https://forum.unity.com/threads/list-order-items-by-dragging.304882/
using UnityEngine;
public class DragOrderContainer : MonoBehaviour
{

    public GameObject objectBeingDragged { get; set; }

    void Awake()
    {
        objectBeingDragged = null;
    }
}

