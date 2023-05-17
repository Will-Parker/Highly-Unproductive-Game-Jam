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
    [SerializeField] [Range(0, 20)] private float moveSpeed;
    [SerializeField] [Range(0, 50)] private int maxPursuitTime; // max number of turns the AI is willing to keep pursuing the player before giving up
    private int pursuitTime = 0;
    public bool hasFinishedTurn = false;
    private PartyManager pm;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        pm = FindObjectOfType<PartyManager>();
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        GameStateManager gsm = FindObjectOfType<GameStateManager>();
        if (gsm.GetGameState() == GameState.Enemy)
        {
            // animation
        }
    }

    public void UpdateAIState()
    {
        hasFinishedTurn = false;
        switch (aiState)
        {
            case AIState.Wander:
                // if the player is one space away of us turn to face & attack
                CheckForAttackables();
                if (aiState != AIState.Attack)
                {
                    // elif the player is nearby or in line of sight pursue

                    // else continue wander
                }
                break;
            case AIState.Pursue:
                // if the player is in front of us attack
                CheckForAttackables();
                // elif pursuit time > max pursuit time return to wander
                break;
            case AIState.Attack:
                // if the player is in front of us attack
                CheckForAttackables();
                // elif 
                break;
        }
    }

    public void CalculateAction()
    {
        switch (aiState)
        {
            case AIState.Wander:
                // determine bad directions
                // random pick good direction
                // if all directions bad do nothing
                break;
            case AIState.Pursue:
                // pathfind 
                break;
            case AIState.Attack:
                // reset pursuit timer
                break;
        }

        // TEMP
        hasFinishedTurn = true;
    }

    private void CheckForAttackables()
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
            aiState = AIState.Attack;

            List<Ally> livingAllies = dict.Keys.ToList();
            livingAllies.RemoveAll(ally => ally.GetHealth() <= 0);

            Ally allyWithMinHealth = livingAllies.MinObject(ally => ally.GetHealth());

            UpdateAnim(dict[allyWithMinHealth]);

            Debug.Log("" + allyWithMinHealth.type + ", " + allyWithMinHealth.GetHealth());
        }

        /*
        bool isTargetUp = false;
        bool isTargetDown = false;
        bool isTargetLeft = false;
        bool isTargetRight = false;
        for (int i = 0; i < 4 || aiState != AIState.Attack; i++)
        {
            Ally ally = pm.allies[i];
            isTargetUp = isTargetUp || ally.transform.position.Equals(Vec2ToVec3(Vector2.up) + transform.position);
            isTargetDown = isTargetDown || ally.transform.position.Equals(Vec2ToVec3(Vector2.down) + transform.position);
            isTargetLeft = isTargetLeft ||ally.transform.position.Equals(Vec2ToVec3(Vector2.left) + transform.position);
            isTargetRight = isTargetRight || ally.transform.position.Equals(Vec2ToVec3(Vector2.right) + transform.position);
        }

        bool[] isTargetDirs = new bool[] { isTargetUp, isTargetDown, isTargetLeft, isTargetRight };

        if (isTargetDirs.Any(isTargetDir => isTargetDir))
        {
            aiState = AIState.Attack;
            List<int> indices = new List<int>();
            for (int j = 0; j < isTargetDirs.Length; ++j) { if (isTargetDirs[j]) { indices.Add(j); indices.Add(j); } }
            switch (indices[Mathf.FloorToInt(Random.Range(0, indices.Count))])
            {
                case 0:
                    UpdateAnim(Vector2.up);
                    break;
                case 1:
                    UpdateAnim(Vector2.down);
                    break;
                case 2:
                    UpdateAnim(Vector2.left);
                    break;
                case 3:
                    UpdateAnim(Vector2.right);
                    break;
                default:
                    throw new System.Exception("Something went wrong with checking for attackable players");
            }
        }
        */
    }
}
