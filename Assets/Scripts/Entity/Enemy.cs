using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Helpers;
using Utils;
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
                // determine good next point
                var points = GetValidNeighbors(Vec3ToVec2(transform.position));
                // random pick good next point
                if (points != null)
                {
                    movePoint = Vec2ToVec3(points[Mathf.FloorToInt(Random.Range(0, points.Count))]);
                    IsMoving = true;
                    UpdateAnim(true, Vec3ToVec2(movePoint - transform.position));
                }
                else
                    IsMoving = false;
                break;
            case AIState.Pursue:
                // pathfind 
                int shortestPathDist = int.MaxValue;
                foreach (Ally ally in pm.allies)
                {
                    var path = AStar(Vec3ToVec2Int(transform.position), Vec3ToVec2Int(ally.transform.position));
                    if (path != null)
                    {
                        string log = "Path is ";
                        foreach (Vector2Int tile in path)
                        {
                            log += tile + ", ";
                        }
                        Debug.Log(log);
                        if (path.Count < shortestPathDist)
                        {
                            shortestPathDist = path.Count;
                            movePoint = Vec2IntToVec3(path[1]);
                            IsMoving = true;
                        }
                    }
                    else
                        Debug.Log("Path is null");
                }
                if (IsMoving)
                {
                    UpdateAnim(true, Vec3ToVec2(movePoint - transform.position));
                }
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
            //targetAlly = GetAllyWithMinHealthFromListExcludingDead(viewableAllies);
            return true;
        }
        else
        {
            //targetAlly = null;
            return false;
        }
    }


    private Ally GetAllyWithMinHealthFromListExcludingDead(List<Ally> allies)
    {
        allies.RemoveAll(ally => ally.GetHealth() <= 0);

        return allies.MinObject(ally => ally.GetHealth());
    }

    private List<Vector2> GetValidNeighbors(Vector2 point)
    {
        var dirs = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        var valid = new List<Vector2>();
        foreach (var dir in dirs)
        {
            RaycastHit2D ray = Physics2D.Raycast(Vec3ToVec2(point), dir, 1f, impassableLayer);
            if (!ray)
            {
                Vector3 targetNeighbor = Vec2ToVec3(point + dir);
                //if (pm.allies.All(ally => ally.transform.position != targetNeighbor)) 
                //{
                    List<Vector3> enemyMovePoints = new ();
                    foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                    {
                        if (enemy.IsMoving)
                        {
                            enemyMovePoints.Add(enemy.movePoint);
                        }
                    }

                    if (enemyMovePoints.All(mp => mp != targetNeighbor)) 
                    {
                        valid.Add(dir + point);
                    }
                //}
            }
        }
        if (valid.Count > 0)
            return valid;
        else
            return null;
    }



    private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        List<Vector2Int> totalPath = new() { current };
        while (cameFrom.ContainsKey(current)) {
            Debug.Log(current);
            current = cameFrom[current];
            totalPath = new List<Vector2Int>(totalPath.Prepend(current));
        }
        return totalPath;
    }

    // A* finds a path from start to goal.
    // h is the heuristic function. h(n) estimates the cost to reach goal from node n.
    private List<Vector2Int> AStar(Vector2Int start, Vector2Int goal)
    {
        // The set of discovered nodes that may need to be (re-)expanded.
        // Initially, only the start node is known.
        // This is usually implemented as a min-heap or priority queue rather than a hash-set.
        var openSetPQ = new PriorityQueue<Vector2Int, int>(new List<System.ValueTuple<Vector2Int, int>>() { (start, 0) });
        var openSetArr = new List<Vector2Int>() { start };

        // For node n, cameFrom[n] is the node immediately preceding it on the cheapest path from the start
        // to n currently known.
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        // For node n, gScore[n] is the cost of the cheapest path from start to n currently known.
        var gScore = new Dictionary<Vector2Int, int> { [start] = 0 };

        // For node n, fScore[n] := gScore[n] + h(n). fScore[n] represents our current best guess as to
        // how cheap a path could be from start to finish if it goes through n.
        var fScore = new Dictionary<Vector2Int, int> { [start] = ManhattanDistance(start, goal) };

        while (openSetPQ.Count > 0)
        {
            // This operation can occur in O(Log(N)) time if openSet is a min-heap or a priority queue
            var current = openSetPQ.Dequeue();
            openSetArr.Remove(current);
            if (current.Equals(goal))
                return ReconstructPath(cameFrom, current);

            // get neighboors of current
            List<Vector2> neighbors = GetValidNeighbors(Vec2IntToVec2(current));
            foreach (Vector2 n in neighbors)
            {
                Vector2Int neighbor = Vec2ToVec2Int(n);
                // d(current,neighbor) is the weight of the edge from current to neighbor
                // tentative_gScore is the distance from start to the neighbor through current
                var tentativeGScore = gScore[current] + 1;
                if (gScore.ContainsKey(neighbor))
                {
                    if (tentativeGScore < gScore[neighbor])
                    {
                        // This path to neighbor is better than any previous one. Record it!
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = tentativeGScore + ManhattanDistance(neighbor, goal);
                        if (!openSetArr.Contains(neighbor))
                        {
                            openSetPQ.Enqueue(neighbor, tentativeGScore);
                            openSetArr.Add(neighbor);
                        }
                    }
                }
                else
                {
                    if (tentativeGScore < int.MaxValue)
                    {
                        // This path to neighbor is better than any previous one. Record it!
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = tentativeGScore + ManhattanDistance(neighbor, goal);
                        if (!openSetArr.Contains(neighbor))
                        {
                            openSetPQ.Enqueue(neighbor, tentativeGScore);
                            openSetArr.Add(neighbor);
                        }
                    }
                }
            }
        }

        return null;
    }


}
