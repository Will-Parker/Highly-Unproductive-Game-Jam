using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Helpers;

public class HoverCommand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea] public string tipToShow;
    private float timeToWait = 0.5f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(StartTimer());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        HoverTipManager.OnMouseLoseFocus();
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            enemy.healthbar.DisableTemporaryDamage();
            enemy.healthbar.gameObject.SetActive(false);
        }
        FindObjectOfType<CursorTileDisplay>().ClearOverlay();
    }

    private void ShowMessage()
    {
        HoverTipManager.OnMouseHover(tipToShow, Input.mousePosition);
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(timeToWait);
        ShowMessage();
        PartyManager pm = FindObjectOfType<PartyManager>();
        float damage = GameData.GetStatSum(pm.allies[0].type, pm.allies[3].type, pm.allies[1].type, StatType.Unique);
        var dirs = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        foreach (var dir in dirs)
        {
            foreach (Enemy enemy in FindObjectsOfType<Enemy>())
            {
                if (enemy.transform.position == pm.allies[1].transform.position + Vec2ToVec3(dir)
                    && pm.allies[1].Health > 0)
                {
                    enemy.healthbar.temporaryCumulativeDamage += damage;
                }
                if (enemy.transform.position == pm.allies[2].transform.position + Vec2ToVec3(dir)
                    && pm.allies[2].Health > 0)
                {
                    enemy.healthbar.temporaryCumulativeDamage += damage;
                }
                if (enemy.transform.position == pm.allies[3].transform.position + Vec2ToVec3(dir)
                    && pm.allies[3].Health > 0)
                {
                    enemy.healthbar.temporaryCumulativeDamage += damage;
                }
            }
        }
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            enemy.healthbar.EnableTemporaryCumulativeDamage();
        }
        FindObjectOfType<CursorTileDisplay>().SetCommandOverlay();
    }
}