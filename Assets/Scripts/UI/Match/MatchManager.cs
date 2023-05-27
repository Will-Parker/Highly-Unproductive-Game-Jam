using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    [SerializeField] private DropSlot slot1;
    [SerializeField] private DropSlot slot2;
    public void Match()
    {
        if (slot1.GetAllyInSlot() != null && slot2.GetAllyInSlot() != null)
        {
            // go to dialogue window with slot1.GetAllyInSlot() and slot2.GetAllyInSlot() plugged in
            Debug.Log("TODO");
        }
    }
}
