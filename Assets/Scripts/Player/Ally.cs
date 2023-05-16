using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : MonoBehaviour
{
    public AllyType type;
    public Animator anim;

    [HideInInspector] private float maxHealth;
    [HideInInspector] public float health;

    private void Awake()
    {
        if (anim == null)
            Debug.Log("Assign Animator in Inspector");
    }

    // Start is called before the first frame update
    void Start()
    {
        switch (type)
        {
            case AllyType.Boomerang:
                maxHealth = 100f;
                break;
            case AllyType.Tank:
                maxHealth = 200f;
                break;
            case AllyType.Healer:
                maxHealth = 50f;
                break;
            case AllyType.Stun:
                maxHealth = 80f;
                break;
        }
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum AllyType
{
    Boomerang,
    Tank,
    Healer,
    Stun
}
