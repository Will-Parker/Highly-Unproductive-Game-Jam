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
    private readonly float timeToWait = 0.2f;
    private bool isHover = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        pm = FindObjectOfType<PartyManager>();
    }

    void Start()
    {
        InitializeStats();
        UpdateAnim(facingDirection);
    }

    private void InitializeStats()
    {
        Ally[] neighbors = pm.GetNeighborAllies(this);
        Health = GameData.GetStatSum(type, neighbors[0].type, neighbors[1].type, StatType.MaxHealth);
        healthbar.SetMaxHealth(GameData.GetStatSum(type, neighbors[0].type, neighbors[1].type, StatType.MaxHealth));
        healthbar.SetHealth(Health);
    }

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
                CharacterControl.instance.UnsubFromEverything();
                SceneManager.LoadSceneAsync(1);
            }
        }
    }

    public List<Vector3> GetEmptyNeighbors()
    {
        var dirs = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        var valid = new List<Vector3>();
        foreach (var dir in dirs)
        {
            RaycastHit2D ray = Physics2D.Raycast(Vec3ToVec2(transform.position), dir, 1f, impassableLayer);
            if (!ray)
            {
                Vector3 targetNeighbor = transform.position + Vec2ToVec3(dir);
                valid.Add(targetNeighbor);
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    if (enemy.transform.position == targetNeighbor)
                        valid.Remove(targetNeighbor);
                }
                valid.Remove(pm.allies[1].transform.position);
            }
        }
        if (valid.Count > 0)
            return valid;
        else
            return null;
    }

    public List<Vector3> GetEnemyNeighbors()
    {
        var dirs = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        var valid = new List<Vector3>();
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

    public List<Vector3> GetNonAllyNeighbors()
    {
        var dirs = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        var valid = new List<Vector3>();
        foreach (var dir in dirs)
        {
            Vector3 targetNeighbor = transform.position + Vec2ToVec3(dir);
            valid.Add(targetNeighbor);
            foreach (Ally ally in pm.allies)
            {
                if (ally.transform.position == targetNeighbor)
                    valid.Remove(targetNeighbor);
            }
        }
        if (valid.Count > 0)
            return valid;
        else
            return null;
    }

    public void AttackEnemy(Enemy enemy)
    {
        Ally[] neighbors = pm.GetNeighborAllies(this);
        if (enemy != null)
            enemy.TakeDamage(GameData.GetStatSum(type, neighbors[0].type, neighbors[1].type, StatType.Attack));
        // do attack anim
    }

    internal void AttackAllAdjacentEnemies(float damage)
    {
        var dirs = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        foreach (var dir in dirs)
        {
            foreach (Enemy enemy in FindObjectsOfType<Enemy>())
            {
                if (enemy.transform.position == transform.position + Vec2ToVec3(dir))
                {
                    enemy.TakeDamage(damage);
                }
            }
        }
    }

    //internal void HeavyAttackEnemy(Enemy enemy)
    //{
    //    Ally[] neighbors = pm.GetNeighborAllies(this);
    //    if (enemy != null)
    //        enemy.TakeDamage((Attack + partnerBuffs[neighbors[0].type][StatType.Attack] + partnerBuffs[neighbors[1].type][StatType.Attack]) 
    //            + (HeavyAttackStat + partnerBuffs[neighbors[0].type][StatType.HeavyAttack] + partnerBuffs[neighbors[1].type][StatType.HeavyAttack]));
    //}

    public void HealAlly(Ally ally)
    {
        Ally[] neighbors = pm.GetNeighborAllies(this);
        AudioManager.instance.Play("Heal");
        ally.Heal(GameData.GetStatSum(type, neighbors[0].type, neighbors[1].type, StatType.Unique));
        // do heal anim;
    }

    public void Heal(float health)
    {
        Ally[] neighbors = pm.GetNeighborAllies(this);
        Health = Mathf.Min(Health + health, GameData.GetStatSum(type, neighbors[0].type, neighbors[1].type, StatType.MaxHealth));
        healthbar.SetHealth(Health);
        anim.SetTrigger("heal");
        anim.SetBool("isDead", false);
    }

    internal void StunEnemy(Enemy enemy)
    {
        AudioManager.instance.Play("Stun");
        Ally[] neighbors = pm.GetNeighborAllies(this);
        enemy.turnsStunned = Mathf.FloorToInt(Mathf.Max(enemy.turnsStunned, GameData.GetStatSum(type, neighbors[0].type, neighbors[1].type, StatType.Unique)));
        enemy.anim.SetBool("isStunned", true);
        enemy.stunText.enabled = true;
        enemy.stunText.text = enemy.turnsStunned.ToString();
    }

    public void PlaceBomb(Vector3 bombLocation)
    {
        var bombGO = Instantiate(Resources.Load("Prefabs/Bomb", typeof(GameObject)), bombLocation, Quaternion.identity) as GameObject;
        var bomb = bombGO.GetComponent<Bomb>();
        Ally[] neighbors = pm.GetNeighborAllies(this);
        bomb.bombRadius = Mathf.FloorToInt(GameData.GetStatSum(pm.allies[0].type, pm.allies[3].type, pm.allies[1].type, StatType.Unique));
        bomb.bombDmg = GameData.GetStatSum(type, neighbors[0].type, neighbors[1].type, StatType.Attack);
    }

    public void SetMaxHealth()
    {
        Ally[] neighbors = pm.GetNeighborAllies(this);
        float newMaxHealth = GameData.GetStatSum(type, neighbors[0].type, neighbors[1].type, StatType.MaxHealth);
        healthbar.SetMaxHealth(newMaxHealth);
        if (newMaxHealth < Health)
        {
            Health = newMaxHealth;
        }
    }

    private void OnMouseOver()
    {
        if (!isHover)
        {
            if (FindObjectOfType<ActionUIManager>().mode == UIActionMode.Heal)
            {
                isHover = true;
                StopAllCoroutines();
                StartCoroutine(StartTemporaryHealbarTimer());
            }
        }
    }

    private void OnMouseExit()
    {
        isHover = false;
        StopAllCoroutines();
        healthbar.DisableTemporaryHeal();
    }

    private IEnumerator StartTemporaryHealbarTimer()
    {
        yield return new WaitForSeconds(timeToWait);
        Ally strawb = pm.GetAlly(AllyType.Strawberry);
        Ally[] neighbors = pm.GetNeighborAllies(strawb);
        healthbar.SetTemporaryHeal(GameData.GetStatSum(AllyType.Strawberry, neighbors[0].type, neighbors[1].type, StatType.Unique));
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
    Unique
}
