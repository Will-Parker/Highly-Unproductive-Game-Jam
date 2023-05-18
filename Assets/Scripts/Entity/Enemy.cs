using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Helpers;
public enum AIState
{
    Wander,
    Pursue,
    Attack
}

public class Enemy : Entity
{
    public AIState aiState;
    // how quickly it moves to next tile
    [SerializeField] [Range(0, 20)] private float moveSpeed = 5;
    [SerializeField] [Range(0, 50)] private int maxPursuitTime = 25; // max number of turns the AI is willing to keep pursuing the player before giving up
    private int pursuitTime = 0;
    [SerializeField] private LayerMask impassableLayer;
    [SerializeField] [Range(0, 20)] private float detectRadius = 15;
    [SerializeField] [Range(0, 360)] private float viewAngle = 120;
    public bool hasFinishedTurn = false;
    private PartyManager pm;
    private Vector3 movePoint;
    public bool IsMoving { get; private set; }
    private Ally targetAlly;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        pm = FindObjectOfType<PartyManager>();
    }
    private void Start()
    {
        IsMoving = false;
    }
    private void Update()
    {
        GameStateManager gsm = FindObjectOfType<GameStateManager>();
        if (gsm.GetGameState() == GameState.Enemy)
        {
            if (IsMoving)
            {
                transform.position = Vector3.MoveTowards(transform.position, movePoint, moveSpeed * Time.deltaTime);

                if (Vector3.Distance(transform.position, movePoint) <= 0.05f)
                {
                    transform.position = movePoint;
                    UpdateAnim(false);
                    hasFinishedTurn = true;
                    IsMoving = false;
                }
            }
        }
    }

    public void UpdateAIState()
    {
        hasFinishedTurn = false;
        switch (aiState)
        {
            case AIState.Wander:
                // if the player is one space away of us turn to face & attack
                if (AdjacentAttackablesCheck())
                    aiState = AIState.Attack;
                // elif the player is nearby or in line of sight pursue
                else if (Vector3.Distance(transform.position, pm.allies[0].transform.position) <= detectRadius
                        || Vector3.Distance(transform.position, pm.allies[1].transform.position) <= detectRadius
                        || Vector3.Distance(transform.position, pm.allies[2].transform.position) <= detectRadius
                        || Vector3.Distance(transform.position, pm.allies[3].transform.position) <= detectRadius
                        || FieldOfViewCheck())
                    aiState = AIState.Pursue;
                break;
            case AIState.Pursue:
                // if the player is in front of us attack
                if (AdjacentAttackablesCheck())
                    aiState = AIState.Attack;
                // elif pursuit time > max pursuit time return to wander
                else if (pursuitTime > maxPursuitTime)
                    aiState = AIState.Wander;
                break;
            case AIState.Attack:
                // if the player is in front of us attack
                if (AdjacentAttackablesCheck())
                    aiState = AIState.Attack;
                // else pursue
                else
                    aiState = AIState.Pursue;
                break;
        }
    }

    public void CalculateAction()
    {
        switch (aiState)
        {
            case AIState.Wander:
                // determine good directions
                var dirs = GetGoodDirections();
                // random pick good direction
                if (dirs != null)
                {
                    movePoint = transform.position + Vec2ToVec3(dirs[Mathf.FloorToInt(Random.Range(0, dirs.Count))]);
                    IsMoving = true;
                    UpdateAnim(true, Vec3ToVec2(movePoint - transform.position));
                }
                else
                    IsMoving = false;
                break;
            case AIState.Pursue:
                // pathfind 
                break;
            case AIState.Attack:
                // damage target ally (may want to wait until animation after that is implemented)
                targetAlly.TakeDamage(30);
                // reset pursuit timer
                pursuitTime = 0;
                break;
        }
    }

    private bool AdjacentAttackablesCheck()
    {
        var dict = new Dictionary<Ally, Vector2>();
        var dirs = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        foreach (Ally ally in pm.allies)
        {
            foreach (Vector2 dir in dirs)
            {
                if (ally.transform.position.Equals(Vec2ToVec3(dir) + transform.position))
                    dict.Add(ally, dir);
            }
        }

        if (dict.Count > 0)
        {
            Ally allyWithMinHealth = GetAllyWithMinHealthFromListExcludingDead(dict.Keys.ToList());
            targetAlly = allyWithMinHealth;
            UpdateAnim(dict[allyWithMinHealth]);
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool FieldOfViewCheck()
    {
        List<Ally> viewableAllies = new List<Ally>();
        foreach (Ally ally in pm.allies)
        {
            Vector2 directionToTarget = Vec3ToVec2(ally.transform.position - transform.position).normalized;

            if (Vector2.Angle(facingDirection, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, ally.transform.position);
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, impassableLayer))
                    viewableAllies.Add(ally);
            }
        }

        if (viewableAllies.Count > 0)
        {
            targetAlly = GetAllyWithMinHealthFromListExcludingDead(viewableAllies);
            return true;
        }
        else
        {
            targetAlly = null;
            return false;
        }
    }


    private Ally GetAllyWithMinHealthFromListExcludingDead(List<Ally> allies)
    {
        allies.RemoveAll(ally => ally.GetHealth() <= 0);

        return allies.MinObject(ally => ally.GetHealth());
    }

    private List<Vector2> GetGoodDirections()
    {
        var dirs = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        var goodDirs = new List<Vector2>();
        foreach (var dir in dirs)
        {
            RaycastHit2D ray = Physics2D.Raycast(Vec3ToVec2(transform.position), dir, 1f, impassableLayer);
            if (!ray)
            {
                // TEMP need to check allies and enemies' move point (if valid)
                goodDirs.Add(dir);
            }
        }
        if (goodDirs.Count > 0)
            return goodDirs;
        else
            return null;
    }
}
