using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : Entity
{
    public AllyType type;
    public Animator anim;

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
    public void UpdateAnim(bool isMoving, Vector2 moveDir)
    {
        Debug.Log("Update Anim");
        if (isMoving)
        {
            anim.SetFloat("Horizontal", moveDir.x);
            anim.SetFloat("Vertical", moveDir.y);
        }
        anim.SetBool("isMoving", isMoving);
    }

    public void UpdateAnim(bool isMoving)
    {
        Debug.Log("Update Anim");
        anim.SetBool("isMoving", isMoving);
    }
}

public enum AllyType
{
    Boomerang,
    Tank,
    Healer,
    Stun
}
