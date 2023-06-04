using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Helpers;

public class Bomb : MonoBehaviour
{
    private readonly float timeToWait = 0.2f;
    public int bombRadius;
    public float bombDmg;
    public GameObject displayRadius;
    private bool isHover = false;

    public void Start()
    {
        displayRadius.transform.localScale = new Vector3((bombRadius * 2) + 1, (bombRadius * 2) + 1, (bombRadius * 2) + 1);
    }

    public void Explode()
    {
        AudioManager.instance.Play("Explode");
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            if (Vector3.Distance(enemy.transform.position, transform.position) < bombRadius + 0.5f)
            {
                enemy.TakeDamage(bombDmg);
            }
        }
        Destroy(gameObject);
    }

    private void OnMouseOver()
    {
        if (!isHover)
        {
            isHover = true;
            StopAllCoroutines();
            StartCoroutine(StartTimer());
        }
    }

    private void OnMouseExit()
    {
        isHover = false;
        StopAllCoroutines();
        displayRadius.SetActive(false);
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            enemy.healthbar.DisableTemporaryDamage();
            enemy.healthbar.gameObject.SetActive(false);
        }
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(timeToWait);
        displayRadius.SetActive(true);
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            enemy.healthbar.DisableTemporaryDamage();
            if (Vector3.Distance(enemy.transform.position, transform.position) < bombRadius + 0.5f)
            {
                enemy.healthbar.gameObject.SetActive(true);
                enemy.healthbar.SetTemporaryDamage(bombDmg);
            }
            else
            {
                enemy.healthbar.DisableTemporaryDamage();
                enemy.healthbar.gameObject.SetActive(false);
            }
        }
    }

    public void DetonateHover()
    {
        displayRadius.SetActive(true);
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            enemy.healthbar.temporaryCumulativeDamage += bombDmg;
        }
    }
}
