using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PartyManager : MonoBehaviour
{
    public Ally[] allies;
    [SerializeField] private CinemachineVirtualCamera virCam;

    // Start is called before the first frame update
    void Start()
    {
        if (allies == null)
            Debug.Log("Allies unassigned in inspector");
        else
        {
            if (allies.Length != 4) // must have a party size of 4
            {
                if (allies[0] == null)
                    Debug.Log("ally 1 unassigned in inspector");
                if (allies[1] == null)
                    Debug.Log("ally 1 unassigned in inspector");
                if (allies[2] == null)
                    Debug.Log("ally 1 unassigned in inspector");
                if (allies[3] == null)
                    Debug.Log("ally 1 unassigned in inspector");
            }
        }
        if (virCam == null)
            Debug.Log("virtual camera unassigned in inspector");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RotatePartyPositions()
    {
        Ally temp = allies[0];
        allies[0] = allies[1];
        allies[1] = allies[2];
        allies[2] = allies[3];
        allies[3] = temp;

        virCam.Follow = allies[0].transform;
    }
}
