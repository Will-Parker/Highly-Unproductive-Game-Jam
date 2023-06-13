using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Helpers;
using TMPro;
using UnityEngine.Tilemaps;

public enum AIState
{
    Wander,
    Pursue,
    Attack
}
public enum EnemyType
{
    Basic,
    Knight,
    Copy
}

public class Enemy : Entity
{
    public EnemyType enemyType;
    [HideInInspector] public AIState aiState;
    private readonly float moveSpeed = 5; // how quickly it moves to next tile
    private readonly int maxPursuitTime = 25; // max number of turns the AI is willing to keep pursuing the player before giving up
    private int pursuitTime = 0;
    [SerializeField] private LayerMask impassableLayer;
    private readonly float detectRadius = 4;
    private readonly float viewAngle = 90;
    [HideInInspector] public bool hasFinishedTurn = false;
    private PartyManager pm;
    private Vector3 movePoint;
    public bool IsMoving { get; private set; }
    private Ally targetAlly;
    private Ally[] targetAllies;
    public int turnsStunned;

    private readonly float timeToWait = 0.2f;
    private bool isDefaultHover = false;
    private bool isAttackHover = false;
    private bool isStunHover = false;

    public TextMeshProUGUI stunText;

    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap impassableMap = null;

    //public Transform debug;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        pm = FindObjectOfType<PartyManager>();
    }
    private void Start()
    {
        IsMoving = false;
        if (facingDirection == Vector2.zero)
            facingDirection = Vector2.down;
        Health = MaxHealth;
        healthbar.SetMaxHealth(MaxHealth);
        healthbar.SetHealth(Health);
        healthbar.gameObject.SetActive(false);
        UpdateAnim(facingDirection);
        switch (enemyType)
        {
            case EnemyType.Basic:
                aiState = AIState.Wander;
                break;
            case EnemyType.Knight:
                aiState = AIState.Pursue;
                break;
        }
    }
    private void Update()
    {
        GameStateManager gsm = GameStateManager.instance;
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
                    SpriteRenderer sr = GetComponent<SpriteRenderer>();
                    sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
                    Debug.Log(gameObject + " has Finished Turn");
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
        if (enemyType != EnemyType.Copy)
        {
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
                    switch (enemyType)
                    {
                        case EnemyType.Basic:
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
                        case EnemyType.Knight:
                            // if the player is in L attack range
                            if (AdjacentAttackablesCheck())
                            {
                                aiState = AIState.Attack;
                                //Debug.Log("" + gameObject + ": Pursue -> Attack");
                            }
                            break;
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
        else
        {
            if (pm.lastActionWasMove)
            {
                aiState = AIState.Wander;
            }
            else
            {
                aiState = AIState.Attack;
            }
        }
    }

    public void CalculateAction()
    {
        switch (aiState)
        {
            case AIState.Wander:
                if (enemyType != EnemyType.Copy)
                {
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
                    {
                        IsMoving = false;
                        hasFinishedTurn = true;
                        Debug.Log(gameObject + " has Finished Turn");
                    }
                }
                else
                {
                    IsMoving = false;
                    hasFinishedTurn = true;
                    Debug.Log(gameObject + " has Finished Turn");
                }
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
                    if (path.Count > 1)
                    {
                        // string log = "Path is "; foreach (Vector2Int tile in path) { log += tile + ", "; } Debug.Log(log);
                        movePoint = Vec2IntToVec3(path[1]);
                        IsMoving = true;
                    }
                    else
                    {
                        IsMoving = false;
                        hasFinishedTurn = true;
                        Debug.Log(gameObject + " has Finished Turn");
                    }
                }
                if (IsMoving)
                {
                    switch (enemyType)
                    {
                        case EnemyType.Basic:
                            UpdateAnim(true, Vec3ToVec2(movePoint - transform.position));
                            pursuitTime++;
                            break;
                        case EnemyType.Knight:
                            Vector3 moveDir = movePoint - transform.position;
                            Vector2 animDir = Vector2.zero;
                            if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
                            {
                                animDir.x = moveDir.x > 0 ? 1 : -1;
                                animDir.y = 0;
                            }
                            else
                            {
                                animDir.x = 0;
                                animDir.y = moveDir.y > 0 ? 1 : -1;
                            }
                            UpdateAnim(animDir);
                            StartCoroutine(SpriteFadeOutFadeIn(GetComponent<SpriteRenderer>(), 4f / moveSpeed));
                            break;
                    }
                } 
                else
                {
                    hasFinishedTurn = true;
                    Debug.Log(gameObject + " has Finished Turn");
                }
                break;
            case AIState.Attack:
                // damage target ally (may want to wait until animation after that is implemented)
                switch (enemyType)
                {
                    case EnemyType.Basic:
                        targetAlly.TakeDamage(Attack);
                        break;
                    case EnemyType.Knight:
                        foreach (Ally ally in targetAllies)
                        {
                            ally.TakeDamage(Attack);
                        }
                        break;
                    case EnemyType.Copy:
                        foreach (Ally ally in pm.allies)
                        {
                            Debug.Log("Ally " + ally.type + " takes " + Attack + " damage");
                            ally.TakeDamage(Attack);
                        }
                        break;
                }
                // reset pursuit timer
                pursuitTime = 0;
                hasFinishedTurn = true;
                Debug.Log(gameObject + " has Finished Turn");
                break;
        }
    }

    public new void TakeDamage(float damage)
    {
        Health = Mathf.Max(Health - damage, 0f);
        healthbar.SetHealth(Health);
        AudioManager.instance.Play("Mushroom Damaged");
        if (Health == 0)
        {
            Destroy(gameObject);
        }
        anim.SetTrigger("takeDamage");
    }

    private bool AdjacentAttackablesCheck()
    {
        var dirs = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        switch (enemyType)
        {
            case EnemyType.Basic:
                var dict = new Dictionary<Ally, Vector2>();
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
                break;
            case EnemyType.Knight:
                Vector3 up = Vec2ToVec3(Vector2.up); Vector3 rt = Vec2ToVec3(Vector2.right); Vector3 dn = Vec2ToVec3(Vector2.down); Vector3 lt = Vec2ToVec3(Vector2.left); Vector3 c = transform.position;
                Vector3[] locs = new Vector3[] 
                {
                    c + up,
                    c + up + up,
                    c + up + up + lt,
                    c + up + up + rt,
                    c + rt,
                    c + rt + rt,
                    c + rt + rt + up,
                    c + rt + rt + dn,
                    c + dn,
                    c + dn + dn,
                    c + dn + dn + lt,
                    c + dn + dn + rt,
                    c + lt,
                    c + lt + lt,
                    c + lt + lt + up,
                    c + lt + lt + dn 
                };
                var hits = new Dictionary<string, (Vector2 dir, List<Ally> allies)>() 
                { 
                    { "upLeftHits", (Vector2.up, new()) },
                    { "upRightHits", (Vector2.up, new()) },
                    { "rightUpHits", (Vector2.right, new()) },
                    { "rightDownHits", (Vector2.right, new()) },
                    { "downLeftHits", (Vector2.down, new()) },
                    { "downRightHits", (Vector2.down, new()) },
                    { "leftUpHits", (Vector2.left, new()) },
                    { "leftDownHits", (Vector2.left, new()) }
                };

                foreach (Ally ally in pm.allies)
                {
                    if (ally.Health > 0)
                    {
                        for (int i = 0; i < locs.Length; i++)
                        {
                            if (ally.transform.position.Equals(locs[i]))
                            {
                                switch (i)
                                {
                                    case 0:
                                    case 1:
                                        hits["upLeftHits"].allies.Add(ally);
                                        hits["upRightHits"].allies.Add(ally);
                                        break;
                                    case 2:
                                        hits["upLeftHits"].allies.Add(ally);
                                        break;
                                    case 3:
                                        hits["upRightHits"].allies.Add(ally);
                                        break;
                                    case 4:
                                    case 5:
                                        hits["rightUpHits"].allies.Add(ally);
                                        hits["rightDownHits"].allies.Add(ally);
                                        break;
                                    case 6:
                                        hits["rightUpHits"].allies.Add(ally);
                                        break;
                                    case 7:
                                        hits["rightDownHits"].allies.Add(ally);
                                        break;
                                    case 8:
                                    case 9:
                                        hits["downLeftHits"].allies.Add(ally);
                                        hits["downRightHits"].allies.Add(ally);
                                        break;
                                    case 10:
                                        hits["downLeftHits"].allies.Add(ally);
                                        break;
                                    case 11:
                                        hits["downRightHits"].allies.Add(ally);
                                        break;
                                    case 12:
                                    case 13:
                                        hits["leftUpHits"].allies.Add(ally);
                                        hits["leftDownHits"].allies.Add(ally);
                                        break;
                                    case 14:
                                        hits["leftUpHits"].allies.Add(ally);
                                        break;
                                    case 15:
                                        break;
                                }
                            }
                        }
                    }
                }
                var listsOfHitDirsAndAlliesPairs = hits.Values.ToList();
                listsOfHitDirsAndAlliesPairs.RemoveAll(hitDirsAndAlliesPairs => hitDirsAndAlliesPairs.allies.Count == 0);
                if (listsOfHitDirsAndAlliesPairs.Count > 0)
                {
                    (int maxHits, int index) = (0, int.MinValue);
                    for (int i = 0; i < listsOfHitDirsAndAlliesPairs.Count; i++)
                    {
                        if (listsOfHitDirsAndAlliesPairs[i].allies.Count > maxHits)
                        {
                            maxHits = listsOfHitDirsAndAlliesPairs[i].allies.Count;
                            index = i;
                        }
                    }
                    if (index >= 0 && index < listsOfHitDirsAndAlliesPairs.Count)
                    {
                        targetAllies = listsOfHitDirsAndAlliesPairs[index].allies.ToArray();
                        UpdateAnim(listsOfHitDirsAndAlliesPairs[index].dir);
                        return true;
                    }
                }
                break;
        }
        return false;
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
                if (distanceToTarget < 10)
                {
                    if (!Physics2D.Raycast(Vec3ToVec2(transform.position), directionToTarget, distanceToTarget, impassableLayer))
                        viewableAllies.Add(ally);
                }
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
        switch (enemyType)
        {
            case EnemyType.Basic:
                break;
            case EnemyType.Knight:
                break;
        }
        var dirs = enemyType == EnemyType.Knight 
            ? new Vector2[] 
            { 
                Vector2.up + Vector2.up + Vector2.left,
                Vector2.up + Vector2.up + Vector2.right,
                Vector2.right + Vector2.right + Vector2.up,
                Vector2.right + Vector2.right + Vector2.down,
                Vector2.down + Vector2.down + Vector2.left,
                Vector2.down + Vector2.down + Vector2.right,
                Vector2.left + Vector2.left + Vector2.up,
                Vector2.left + Vector2.left + Vector2.down,
            } 
            : new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        var valid = new List<Vector2>();
        foreach (var dir in dirs)
        {
            bool isImpassable = enemyType == EnemyType.Knight ? impassableMap.GetTile(grid.WorldToCell(Vec2ToVec3(point + dir))) != null 
                : Physics2D.Raycast(Vec3ToVec2(point), dir, 1f, impassableLayer);
            if (!isImpassable)
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

    private void OnMouseOver()
    {
        switch (FindObjectOfType<ActionUIManager>().mode)
        {
            case UIActionMode.Attack:
            case UIActionMode.None:
                if (!isAttackHover)
                {
                    isAttackHover = true;
                    isDefaultHover = false;
                    isStunHover = false;
                    StopAllCoroutines();
                    StartCoroutine(StartTemporaryDamagebarTimer());
                }
                break;
            case UIActionMode.Stun:
                if (!isStunHover)
                {
                    isStunHover = true;
                    isAttackHover = false;
                    isDefaultHover = false;
                    StopAllCoroutines();
                    StartCoroutine(StartStunTextTimer());
                }
                break;
            case UIActionMode.Bomb:
                break;
            default:
                if (!isDefaultHover)
                {
                    isDefaultHover = true;
                    isAttackHover = false;
                    isStunHover = false;
                    StopAllCoroutines();
                    StartCoroutine(StartHealthbarTimer());
                }
                break;
        }
    }

    private void OnMouseExit()
    {
        isAttackHover = false;
        isDefaultHover = false;
        isStunHover = false;
        StopAllCoroutines();
        healthbar.DisableTemporaryDamage();
        healthbar.gameObject.SetActive(false);
        stunText.text = turnsStunned.ToString();
        if (turnsStunned == 0)
        {
            stunText.enabled = false;
        }
    }

    private IEnumerator StartHealthbarTimer()
    {
        yield return new WaitForSeconds(timeToWait);
        healthbar.gameObject.SetActive(true);
        healthbar.DisableTemporaryDamage();
        stunText.text = turnsStunned.ToString();
        if (turnsStunned == 0)
        {
            stunText.enabled = false;
        }
    }

    private IEnumerator StartTemporaryDamagebarTimer()
    {
        yield return new WaitForSeconds(timeToWait);
        healthbar.gameObject.SetActive(true);
        healthbar.SetTemporaryDamage(GameData.GetStatSum(pm.allies[0].type, pm.allies[3].type, pm.allies[1].type, StatType.Attack));
        stunText.text = turnsStunned.ToString();
        if (turnsStunned == 0)
        {
            stunText.enabled = false;
        }
    }

    private IEnumerator StartStunTextTimer()
    {
        yield return new WaitForSeconds(timeToWait);
        healthbar.gameObject.SetActive(true);
        healthbar.DisableTemporaryDamage();
        stunText.enabled = true;
        stunText.text = turnsStunned.ToString() + ">" + 
            Mathf.FloorToInt(Mathf.Max(turnsStunned, GameData.GetStatSum(pm.allies[0].type, pm.allies[3].type, pm.allies[1].type, StatType.Unique)));
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
        
        while (openSet.Count > 0)
        {
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
        return ReconstructPath(cameFrom, fScore.Aggregate((l, r) => l.Value < r.Value ? l : r).Key);
    }

    // Credit: https://answers.unity.com/questions/1687634/how-do-i-mathflerp-the-spriterendereralpha.html
    private IEnumerator SpriteFadeOutFadeIn(SpriteRenderer sr, float duration)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration / 2)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(1f, 0f, elapsedTime / (duration / 2));
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, newAlpha);
            yield return null;
        }
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(0f, 1f, (elapsedTime - (duration / 2)) / (duration / 2));
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, newAlpha);
            yield return null;
        }
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
    }
}
