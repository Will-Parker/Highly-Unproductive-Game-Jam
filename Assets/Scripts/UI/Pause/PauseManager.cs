using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private RectTransform partyGroup;
    private PartyManager pm;
    private void Awake()
    {
        pm = FindObjectOfType<PartyManager>();
    }

    private void OnEnable()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject obj = null;
            switch (pm.allies[i].type)
            {
                case AllyType.Apple:
                    obj = Instantiate(Resources.Load("Prefabs/UI/Apple Group", typeof(GameObject)), partyGroup) as GameObject;
                    break;
                case AllyType.Strawberry:
                    obj = Instantiate(Resources.Load("Prefabs/UI/Strawberry Group", typeof(GameObject)), partyGroup) as GameObject;
                    break;
                case AllyType.Lemon:
                    obj = Instantiate(Resources.Load("Prefabs/UI/Lemon Group", typeof(GameObject)), partyGroup) as GameObject;
                    break;
                case AllyType.Blueberry:
                    obj = Instantiate(Resources.Load("Prefabs/UI/Blueberry Group", typeof(GameObject)), partyGroup) as GameObject;
                    break;
            }
            if (obj != null)
            {
                AllyPauseGroup apg = obj.GetComponent<AllyPauseGroup>();
                apg.id = i;
                apg.SetText();
            }
        }

    }

    private void OnDisable()
    {
        foreach (Transform child in partyGroup)
        {
            Destroy(child.gameObject);
        }
    }
}
