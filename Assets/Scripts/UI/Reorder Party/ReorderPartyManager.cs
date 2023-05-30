using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReorderPartyManager : MonoBehaviour
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
                    obj = Instantiate(Resources.Load("Prefabs/UI/Reorder Party/Apple Level Up Group", typeof(GameObject)), partyGroup) as GameObject;
                    break;
                case AllyType.Strawberry:
                    obj = Instantiate(Resources.Load("Prefabs/UI/Reorder Party/Strawberry Level Up Group", typeof(GameObject)), partyGroup) as GameObject;
                    break;
                case AllyType.Lemon:
                    obj = Instantiate(Resources.Load("Prefabs/UI/Reorder Party/Lemon Level Up Group", typeof(GameObject)), partyGroup) as GameObject;
                    break;
                case AllyType.Blueberry:
                    obj = Instantiate(Resources.Load("Prefabs/UI/Reorder Party/Blueberry Level Up Group", typeof(GameObject)), partyGroup) as GameObject;
                    break;
            }
            if (obj != null)
            {
                AllyLevelUpGroup alug = obj.GetComponent<AllyLevelUpGroup>();
                alug.isLevelUp = pm.allies[i].isLevelUp;
                alug.oldId = i;
                alug.id = i;
                alug.allyType = pm.allies[i].type;
                alug.InitializeText();
                obj.AddComponent<DragOrderObject>();

                pm.allies[i].isLevelUp = false;
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

    public void SetOut()
    {
        AllyType allyType1 = partyGroup.GetChild(0).GetComponent<AllyLevelUpGroup>().allyType;
        AllyType allyType2 = partyGroup.GetChild(1).GetComponent<AllyLevelUpGroup>().allyType;
        AllyType allyType3 = partyGroup.GetChild(2).GetComponent<AllyLevelUpGroup>().allyType;
        AllyType allyType4 = partyGroup.GetChild(3).GetComponent<AllyLevelUpGroup>().allyType;
        FindObjectOfType<PartyManager>().SetPartyOrder(allyType1, allyType2, allyType3, allyType4);
        FindObjectOfType<CharacterControl>().SubToAllGameplayActions();
        gameObject.SetActive(false);
    }

    public void UpdateAllAllyText()
    {
        foreach (Transform child in partyGroup)
        {
            child.GetComponent<AllyLevelUpGroup>().SetText();
        }
    }
}
