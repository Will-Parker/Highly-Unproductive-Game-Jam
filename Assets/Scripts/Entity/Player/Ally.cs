using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Helpers;

public class Ally : Entity
{
    public AllyType type;
    private PartyManager pm;
    [SerializeField] private LayerMask impassableLayer;

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

    public List<Vector2> GetEmptyNeighbors()
    {
        var dirs = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        var valid = new List<Vector2>(dirs);
        foreach (var dir in dirs)
        {
            RaycastHit2D ray = Physics2D.Raycast(Vec3ToVec2(transform.position), dir, 1f, impassableLayer);
            if (!ray)
            {
                Vector3 targetNeighbor = transform.position + Vec2ToVec3(dir);
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    if (enemy.transform.position == targetNeighbor)
                        valid.Remove(targetNeighbor);
                }
            }
        }
        if (valid.Count > 0)
            return valid;
        else
            return null;
    }

    public List<Vector2> GetEnemyNeighbors()
    {
        var dirs = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        var valid = new List<Vector2>();
        foreach (var dir in dirs)
        {
            RaycastHit2D ray = Physics2D.Raycast(Vec3ToVec2(transform.position), dir, 1f, impassableLayer);
            if (!ray)
            {
                Vector3 targetNeighbor = transform.position + Vec2ToVec3(dir);
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    if (enemy.transform.position == targetNeighbor)
                        valid.Add(targetNeighbor);
                }
            }
        }
        if (valid.Count > 0)
            return valid;
        else
            return null;
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

    public void AttackEnemy(Enemy enemy)
    {
        if (enemy != null)
            enemy.TakeDamage(Attack);
        // do attack anim
    }
}

public enum AllyType
{
    Apple,
    Strawberry,
    Lemon,
    Blueberry
}
