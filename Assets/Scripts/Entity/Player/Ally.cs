using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : Entity
{
    public AllyType type;

    private void Awake()
    {
        anim = GetComponent<Animator>();
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

    public new void TakeDamage(float damage)
    {
        health = Mathf.Max(health - damage, 0);
        if (health == 0)
        {
            GetComponent<SpriteRenderer>().color = Color.gray;
        }
    }
}

public enum AllyType
{
    Boomerang,
    Tank,
    Healer,
    Stun
}
