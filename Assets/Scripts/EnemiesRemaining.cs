using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemiesRemaining : MonoBehaviour
{
    [SerializeField] public List<GameObject> enemies;
    private TextMeshProUGUI a;



    // Start is called before the first frame update
    void Start()
    {
        a = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        enemies.RemoveAll(s => s == null);
        a.text = "Enemies Remaining: " + enemies.Count;
    }
}
