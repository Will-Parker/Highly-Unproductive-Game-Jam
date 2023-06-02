using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Helpers;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class Ally : Entity
{
    public AllyType type;
    private PartyManager pm;
    [SerializeField] private LayerMask impassableLayer;
    public float HealStat { get; private set; } // how much HP to heal ally when healing
    public float HeavyAttackStat { get; private set; } // how much to multiply attack when heavy attacking
    public float StunStat { get; private set; } // how many turns to stun an enemy when stunning
    public float BombStat { get; private set; } // how big the radius of the bomb is
    public Dictionary<AllyType, Dictionary<StatType, float>> partnerBuffs = new Dictionary<AllyType, Dictionary<StatType, float>>() 
    {
        { 
            AllyType.Apple, new Dictionary<StatType, float>()
            {
                { StatType.MaxHealth, 0f },
                { StatType.Attack, 0f },
                { StatType.HeavyAttack, 0f },
                { StatType.Heal, 0f },
                { StatType.Stun, 0f },
                { StatType.Bomb, 0f }
            }
        },
        {
            AllyType.Strawberry, new Dictionary<StatType, float>()
            {
                { StatType.MaxHealth, 0f },
                { StatType.Attack, 0f },
                { StatType.HeavyAttack, 0f },
                { StatType.Heal, 0f },
                { StatType.Stun, 0f },
                { StatType.Bomb, 0f }
            }
        },
        {
            AllyType.Lemon, new Dictionary<StatType, float>()
            {
                { StatType.MaxHealth, 0f },
                { StatType.Attack, 0f },
                { StatType.HeavyAttack, 0f },
                { StatType.Heal, 0f },
                { StatType.Stun, 0f },
                { StatType.Bomb, 0f }
            }
        },
        {
            AllyType.Blueberry, new Dictionary<StatType, float>()
            {
                { StatType.MaxHealth, 0f },
                { StatType.Attack, 0f },
                { StatType.HeavyAttack, 0f },
                { StatType.Heal, 0f },
                { StatType.Stun, 0f },
                { StatType.Bomb, 0f }
            }
        }
    };
    public Dictionary<AllyType, Dictionary<StatType, float>> oldPartnerBuffs = new Dictionary<AllyType, Dictionary<StatType, float>>()
    {
        {
            AllyType.Apple, new Dictionary<StatType, float>()
            {
                { StatType.MaxHealth, 0f },
                { StatType.Attack, 0f },
                { StatType.HeavyAttack, 0f },
                { StatType.Heal, 0f },
                { StatType.Stun, 0f },
                { StatType.Bomb, 0f }
            }
        },
        {
            AllyType.Strawberry, new Dictionary<StatType, float>()
            {
                { StatType.MaxHealth, 0f },
                { StatType.Attack, 0f },
                { StatType.HeavyAttack, 0f },
                { StatType.Heal, 0f },
                { StatType.Stun, 0f },
                { StatType.Bomb, 0f }
            }
        },
        {
            AllyType.Lemon, new Dictionary<StatType, float>()
            {
                { StatType.MaxHealth, 0f },
                { StatType.Attack, 0f },
                { StatType.HeavyAttack, 0f },
                { StatType.Heal, 0f },
                { StatType.Stun, 0f },
                { StatType.Bomb, 0f }
            }
        },
        {
            AllyType.Blueberry, new Dictionary<StatType, float>()
            {
                { StatType.MaxHealth, 0f },
                { StatType.Attack, 0f },
                { StatType.HeavyAttack, 0f },
                { StatType.Heal, 0f },
                { StatType.Stun, 0f },
                { StatType.Bomb, 0f }
            }
        }
    };

    public bool isLevelUp = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        pm = FindObjectOfType<PartyManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeStats();
        UpdateAnim(facingDirection);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public new void TakeDamage(float damage)
    {
        Health = Mathf.Max(Health - damage, 0f);
        anim.SetTrigger("takeDamage");
        switch (type)
        {
            case AllyType.Apple:
                AudioManager.instance.Play("Apple Damaged");
                break;
            case AllyType.Strawberry:
                AudioManager.instance.Play("Strawberry Damaged");
                break;
            case AllyType.Lemon:
                AudioManager.instance.Play("Lemon Damaged");
                break;
            case AllyType.Blueberry:
                AudioManager.instance.Play("Blueberry Damaged");
                break;
        }
        healthbar.SetHealth(Health);
        if (Health == 0)
        {
            anim.SetBool("isDead", true);
            if (pm.allies[0].Health == 0 && pm.allies[1].Health == 0 && pm.allies[2].Health == 0 && pm.allies[3].Health == 0)
            {
                AudioManager.instance.Stop("Gameplay Music");
                FindObjectOfType<CharacterControl>().UnsubFromEverything();
                SceneManager.LoadSceneAsync(2);
            }
        }
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
                MaxHealth = 15f;
                Attack = 2f;
                HeavyAttackStat = 2f;
                break;
            case AllyType.Strawberry:
                MaxHealth = 6f;
                Attack = 1f;
                HealStat = 5f;
                break;
            case AllyType.Lemon:
                MaxHealth = 8f;
                Attack = 1f;
                StunStat = 1f;
                break;
            case AllyType.Blueberry:
                MaxHealth = 10f;
                Attack = 3f;
                BombStat = 1f;
                break;
        }
        Health = MaxHealth;
        healthbar.SetMaxHealth(MaxHealth);
        healthbar.SetHealth(Health);
    }

    public void AttackEnemy(Enemy enemy)
    {
        Ally[] neighbors = pm.GetNeighborAllies(this);
        if (enemy != null)
            enemy.TakeDamage(Attack + partnerBuffs[neighbors[0].type][StatType.Attack] + partnerBuffs[neighbors[1].type][StatType.Attack]);
        // do attack anim
    }

    internal void HeavyAttackEnemy(Enemy enemy)
    {
        Ally[] neighbors = pm.GetNeighborAllies(this);
        if (enemy != null)
            enemy.TakeDamage((Attack + partnerBuffs[neighbors[0].type][StatType.Attack] + partnerBuffs[neighbors[1].type][StatType.Attack]) 
                + (HeavyAttackStat + partnerBuffs[neighbors[0].type][StatType.HeavyAttack] + partnerBuffs[neighbors[1].type][StatType.HeavyAttack]));
    }

    public void HealAlly(Ally ally)
    {
        Ally[] neighbors = pm.GetNeighborAllies(this);
        AudioManager.instance.Play("Heal");
        ally.Heal(HealStat + partnerBuffs[neighbors[0].type][StatType.Heal] + partnerBuffs[neighbors[1].type][StatType.Heal]);
        // do heal anim;
    }

    public void Heal(float health)
    {
        Ally[] neighbors = pm.GetNeighborAllies(this);
        Health = Mathf.Min(Health + health, MaxHealth + partnerBuffs[neighbors[0].type][StatType.MaxHealth] + partnerBuffs[neighbors[1].type][StatType.MaxHealth]);
        healthbar.SetHealth(Health);
        anim.SetTrigger("heal");
        anim.SetBool("isDead", false);
    }

    internal void StunEnemy(Enemy enemy)
    {
        AudioManager.instance.Play("Stun");
        Ally[] neighbors = pm.GetNeighborAllies(this);
        enemy.turnsStunned = Mathf.FloorToInt(Mathf.Max(enemy.turnsStunned, 
            StunStat + partnerBuffs[neighbors[0].type][StatType.Stun] + partnerBuffs[neighbors[1].type][StatType.Stun]));
        enemy.anim.SetBool("isStunned", true);
    }

    public void PlaceBomb(Vector3 bombLocation)
    {
        var bombGO = Instantiate(Resources.Load("Prefabs/Bomb", typeof(GameObject)), bombLocation, Quaternion.identity) as GameObject;
        var bomb = bombGO.GetComponent<Bomb>();
        Ally[] neighbors = pm.GetNeighborAllies(this);
        bomb.bombRadius = Mathf.FloorToInt(((BombStat + partnerBuffs[neighbors[0].type][StatType.Bomb] + partnerBuffs[neighbors[1].type][StatType.Bomb]) * 2) + 1);
        bomb.bombDmg = Attack + partnerBuffs[neighbors[0].type][StatType.Attack] + partnerBuffs[neighbors[1].type][StatType.Attack];
    }

    public void SetMaxHealth()
    {
        Ally[] neighbors = pm.GetNeighborAllies(this);
        float newMaxHealth = MaxHealth + partnerBuffs[neighbors[0].type][StatType.MaxHealth] + partnerBuffs[neighbors[1].type][StatType.MaxHealth];
        healthbar.SetMaxHealth(newMaxHealth);
        if (newMaxHealth < Health)
        {
            Health = newMaxHealth;
        }
    }
}

public enum AllyType
{
    Apple,
    Strawberry,
    Lemon,
    Blueberry
}

public enum StatType
{
    MaxHealth,
    Attack,
    HeavyAttack,
    Heal,
    Stun,
    Bomb,
}
