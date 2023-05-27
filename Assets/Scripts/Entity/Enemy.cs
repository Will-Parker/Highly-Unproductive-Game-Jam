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
    [HideInInspector] public AIState aiState;
    private readonly float moveSpeed = 5; // how quickly it moves to next tile
    private readonly int maxPursuitTime = 25; // max number of turns the AI is willing to keep pursuing the player before giving up
    private int pursuitTime = 0;
    [SerializeField] private LayerMask impassableLayer;
    private readonly float detectRadius = 5;
    private readonly float viewAngle = 120;
    [HideInInspector] public bool hasFinishedTurn = false;
    private PartyManager pm;
    private Vector3 movePoint;
    public bool IsMoving { get; private set; }
    private Ally targetAlly;
    public int turnsStunned;

    //public Transform debug;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        pm = FindObjectOfType<PartyManager>();
    }
    private void Start()
    {
        IsMoving = false;
        facingDirection = Vector2.up;
        Health = MaxHealth;
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
        //foreach (Transform child in debug)
        //    Destroy(child.gameObject);
        hasFinishedTurn = false;
        switch (aiState)
        {
            case AIState.Wander:
                pursuitTime = 0;
                // if the player is one space away of us turn to face & attack
                if (AdjacentAttackablesCheck())
                {
                    aiState = AIState.Attack;
                    //Debug.Log("" + gameObject + ": Wander -> Attack");
                }
                // elif the player is nearby or in line of sight pursue
                else if (Vector3.Distance(transform.position, pm.allies[0].transform.position) <= detectRadius
                        || Vector3.Distance(transform.position, pm.allies[1].transform.position) <= detectRadius
                        || Vector3.Distance(transform.position, pm.allies[2].transform.position) <= detectRadius
                        || Vector3.Distance(transform.position, pm.allies[3].transform.position) <= detectRadius
                        || FieldOfViewCheck())
                {
                    aiState = AIState.Pursue;
                    //Debug.Log("" + gameObject + ": Wander -> Pursue");
                }
                break;
            case AIState.Pursue:
                // if the player is in front of us attack
                if (AdjacentAttackablesCheck())
                {
                    aiState = AIState.Attack;
                    //Debug.Log("" + gameObject + ": Pursue -> Attack");
                }
                // elif pursuit time > max pursuit time return to wander
                else if (pursuitTime > maxPursuitTime)
                {
                    aiState = AIState.Wander;
                    //Debug.Log("" + gameObject + ": Pursue -> Wander");
                    pursuitTime = 0;
                }
                break;
            case AIState.Attack:
                pursuitTime = 0;
                // if the player is in front of us attack
                if (AdjacentAttackablesCheck())
                {
                    aiState = AIState.Attack;
                }
                // else pursue
                else
                {
                    aiState = AIState.Pursue;
                    //Debug.Log("" + gameObject + ": Attack -> Pursue");
                }
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
                // Target ally on naive shortest distance
                int naiveShortestDist = int.MaxValue;
                Ally naiveClosestAlly = null;
                foreach (Ally ally in pm.allies)
                {
                    if (ally.Health > 0)
                    {
                        int naiveDist = ManhattanDistance(Vec3ToVec2Int(transform.position), Vec3ToVec2Int(ally.transform.position));
                        if (naiveDist < naiveShortestDist)
                        {
                            naiveShortestDist = naiveDist;
                            naiveClosestAlly = ally;
                        }
                    }
                }
                if (naiveClosestAlly != null)
                {
                    var path = AStar(Vec3ToVec2Int(transform.position), Vec3ToVec2Int(naiveClosestAlly.transform.position));
                    if (path != null)
                    {
                        if (path.Count > 1)
                        {
                            // string log = "Path is "; foreach (Vector2Int tile in path) { log += tile + ", "; } Debug.Log(log);
                            movePoint = Vec2IntToVec3(path[1]);
                            IsMoving = true;
                        }
                    }
                }
                if (IsMoving)
                {
                    UpdateAnim(true, Vec3ToVec2(movePoint - transform.position));
                    pursuitTime++;
                } 
                else
                {
                    hasFinishedTurn = true;
                }
                break;
            case AIState.Attack:
                // damage target ally (may want to wait until animation after that is implemented)
                targetAlly.TakeDamage(Attack);
                // reset pursuit timer
                pursuitTime = 0;
                hasFinishedTurn = true;
                break;
        }
    }

    public new void TakeDamage(float damage)
    {
        Health = Mathf.Max(Health - damage, 0f);
        if (Health == 0)
        {
            pm.GainExperience(1);
            Destroy(gameObject);
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
            if (allyWithMinHealth == null)
                return false;
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
                if (!Physics2D.Raycast(Vec3ToVec2(transform.position), directionToTarget, distanceToTarget, impassableLayer))
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
        allies.RemoveAll(ally => ally.Health <= 0);

        if (allies.Count == 0)
            return null;

        return allies.MinObject(ally => ally.Health);
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
                if (pm.allies.All(ally => ally.transform.position != targetNeighbor)) 
                {
                    List<Vector3> enemyClaimedPosition = new ();
                    foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                    {
                        if (enemy != this)
                        {
                            if (enemy.IsMoving)
                            {
                                enemyClaimedPosition.Add(enemy.movePoint);
                            }
                            else
                            {
                                enemyClaimedPosition.Add(enemy.transform.position);
                            }
                        }
                    }

                    if (enemyClaimedPosition.All(mp => mp != targetNeighbor)) 
                    {
                        valid.Add(dir + point);
                    }
                }
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
            //GameObject x = Instantiate(Resources.Load("Prefabs/Debug", typeof(GameObject)), Vec2IntToVec3(current), Quaternion.identity, debug) as GameObject;
            //x.GetComponent<SpriteRenderer>().color = Color.red;
            //x.GetComponent<SpriteRenderer>().sortingOrder = 10;
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
        var openSet = new Dictionary<Vector2Int, int>() { [start] = 0 };
        //var openSetArr = new List<System.ValueTuple<Vector2Int, int>>() { (start, 0) };

        // For node n, cameFrom[n] is the node immediately preceding it on the cheapest path from the start
        // to n currently known.
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        // For node n, gScore[n] is the cost of the cheapest path from start to n currently known.
        var gScore = new Dictionary<Vector2Int, int> { [start] = 0 };

        // For node n, fScore[n] := gScore[n] + h(n). fScore[n] represents our current best guess as to
        // how cheap a path could be from start to finish if it goes through n.
        var fScore = new Dictionary<Vector2Int, int> { [start] = ManhattanDistance(start, goal) };

        //int iteration = 0;
        while (openSet.Count > 0)
        {
            //iteration++;
            //string log = "Iteration: " + iteration;
            //log += "\nOpen Set:";
            //foreach (var kvp in openSet)
            //{
            //    log += "\n  Node: " + kvp.Key + ", Distance: " + kvp.Value;
            //}
            //Debug.Log(log);

            // This operation can occur in O(Log(N)) time if openSet is a min-heap or a priority queue
            var current = openSet.Aggregate((l, r) => l.Value < r.Value ? l : r).Key;
            if (openSet[current] > 5)
            {
                //Debug.Log("Path was greater than 5 which so we take min fScore path\n"
                //    + "Start was " + start + "\n"
                //    + "Goal was " + goal);
                return ReconstructPath(cameFrom, fScore.Aggregate((l, r) => l.Value < r.Value ? l : r).Key);
            }
            openSet.Remove(current);
            //GameObject x = Instantiate(Resources.Load("Prefabs/Debug", typeof(GameObject)), Vec2IntToVec3(current), Quaternion.identity, debug) as GameObject;
            //x.GetComponent<SpriteRenderer>().color = Color.blue;

            //if (current.Equals(goal))
            if (Vector2Int.Distance(current, goal) == 1)
                return ReconstructPath(cameFrom, current);

            // get neighboors of current
            List<Vector2> neighbors = GetValidNeighbors(Vec2IntToVec2(current));
            if (neighbors != null)
            {
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
                            //fScore[neighbor] = tentativeGScore + ManhattanDistance(neighbor, goal);
                            fScore[neighbor] = ManhattanDistance(neighbor, goal);
                            openSet[neighbor] = tentativeGScore;
                        }
                    }
                    else
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        //fScore[neighbor] = tentativeGScore + ManhattanDistance(neighbor, goal);
                        fScore[neighbor] = ManhattanDistance(neighbor, goal);
                        openSet[neighbor] = tentativeGScore;
                    }
                }
            }
        }
        Debug.LogWarning("A* could not find goal in accesible area\n"
            + "Start was " + start + "\n"
            + "Goal was " + goal);
        return null;
    }


}
