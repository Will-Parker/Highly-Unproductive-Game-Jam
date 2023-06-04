using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverDetonate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
        foreach (Bomb bomb in FindObjectsOfType<Bomb>())
        {
            bomb.displayRadius.SetActive(false);
        }
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            enemy.healthbar.DisableTemporaryDamage();
            enemy.healthbar.gameObject.SetActive(false);
        }
    }

    private void ShowMessage()
    {
        HoverTipManager.OnMouseHover(tipToShow, Input.mousePosition);
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(timeToWait);
        ShowMessage();
        foreach (Bomb bomb in FindObjectsOfType<Bomb>())
        {
            bomb.DetonateHover();
        }
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            enemy.healthbar.EnableTemporaryCumulativeDamage();
        }
    }
}
