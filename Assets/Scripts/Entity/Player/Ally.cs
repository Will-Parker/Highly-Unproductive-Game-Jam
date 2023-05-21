using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Helpers;

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
        InitializeStats();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public new void TakeDamage(float damage)
    {
        Health = Mathf.Max(Health - damage, 0f);
        if (Health == 0)
        {
            GetComponent<SpriteRenderer>().color = Color.gray;
        }
    }

    public void PeformSpecialAction()
    {
        // for now just attack enemy if there is an enemy one space ahead.
        Enemy enemy = GetEnemyInfrontOf();
        if (enemy != null)
            enemy.TakeDamage(Attack);
    }

    private Enemy GetEnemyInfrontOf()
    {
        var enemies = new List<Enemy>(FindObjectsOfType<Enemy>());
        foreach (Enemy enemy in enemies)
        {
            if (Vector2.Distance(Vec3ToVec2(transform.position) + facingDirection, Vec3ToVec2(enemy.transform.position)) <= 0.05f)
                return enemy;
        }
        return null;
    }

    private void InitializeStats()
    {
        switch (type)
        {
            case AllyType.Apple:
                MaxHealth = 20f;
                Attack = 2f;
                break;
            case AllyType.Strawberry:
                MaxHealth = 5f;
                Attack = 0f;
                break;
            case AllyType.Lemon:
                MaxHealth = 8f;
                Attack = 0f;
                break;
            case AllyType.Blueberry:
                MaxHealth = 10f;
                Attack = 3f;
                break;
        }
        Health = MaxHealth;
    }
}

public enum AllyType
{
    Apple,
    Strawberry,
    Lemon,
    Blueberry
}
