using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using static Helpers;
using System;

public class PartyManager : MonoBehaviour
{
    public Ally[] allies;
    // points each ally moves toward
    private Vector3[] movePoints = new Vector3[4]; // hardcoded to 4
    [SerializeField] private CinemachineVirtualCamera virCam;
    // how quickly it moves to next tile
    [SerializeField] [Range(0, 20)] private float moveSpeed;
    [HideInInspector] public MoveState moveState = MoveState.NotMoving;
    // impassable layer
    [SerializeField] private LayerMask impassableLayer;
    private Vector3 prevTail;

    void Start()
    {
        if (allies == null)
            Debug.Log("Allies unassigned in inspector");
        else
        {
            if (allies.Length != 4) // must have a party size of 4
            {
                if (allies[0] == null)
                    Debug.Log("ally 1 unassigned in inspector");
                if (allies[1] == null)
                    Debug.Log("ally 1 unassigned in inspector");
                if (allies[2] == null)
                    Debug.Log("ally 1 unassigned in inspector");
                if (allies[3] == null)
                    Debug.Log("ally 1 unassigned in inspector");
            }
        }
        if (virCam == null)
            Debug.Log("virtual camera unassigned in inspector");
        for (int i = 0; i < 4; i++)
        {
            movePoints[i] = allies[i].transform.position;
            //allies[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }

        prevTail = new Vector3(allies[3].transform.position.x - 1, allies[3].transform.position.y, allies[3].transform.position.z);
    }

    void Update()
    {
        if (moveState != MoveState.NotMoving)
        {
            if (moveState == MoveState.Swap)
            {
                allies[0].transform.position = Vector3.MoveTowards(allies[0].transform.position, movePoints[0], moveSpeed * Time.deltaTime);
                allies[1].transform.position = Vector3.MoveTowards(allies[1].transform.position, movePoints[1], moveSpeed * Time.deltaTime);
                if (Vector3.Distance(allies[0].transform.position, movePoints[0]) <= 0.05f)
                {
                    allies[0].transform.position = movePoints[0];
                    allies[1].transform.position = movePoints[1];
                    allies[0].UpdateAnim(false, allies[0].facingDirection);
                    allies[1].facingDirection = Vec3ToVec2(allies[1].transform.position - allies[2].transform.position);
                    allies[1].UpdateAnim(false, allies[1].facingDirection);
                    moveState = MoveState.NotMoving;
                    GameStateManager gsm = GameStateManager.instance;
                    if (gsm == null)
                        Debug.LogWarning("No GSM in scene");
                    else
                        gsm.EndTurn();
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                    allies[i].transform.position = Vector3.MoveTowards(allies[i].transform.position, movePoints[i], moveSpeed * Time.deltaTime);

                if (moveState == MoveState.Moving)
                {
                    if (Vector3.Distance(allies[0].transform.position, movePoints[0]) <= 0.05f)
                    {
                        for (int i = 0; i < 4; i++)
                            allies[i].transform.position = movePoints[i];
                        RotateParty();
                    }
                }
                else if (moveState == MoveState.Rotating)
                {
                    if (Vector3.Distance(allies[0].transform.position, movePoints[0]) <= 0.05f)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            allies[i].transform.position = movePoints[i];
                            allies[i].UpdateAnim(false);
                        }
                        moveState = MoveState.NotMoving;
                        GameStateManager gsm = GameStateManager.instance;
                        if (gsm == null)
                            Debug.LogWarning("No GSM in scene");
                        else
                            gsm.EndTurn();
                    }
                }
            }
        }
    }

    public void AttemptMove(Vector2 moveDir)
    {
        if (moveState == MoveState.NotMoving)
        {
            RaycastHit2D ray = Physics2D.Raycast(Vec3ToVec2(allies[0].transform.position), moveDir, 1f, impassableLayer);
            if (!ray
                && Vector2.Distance(Vec3ToVec2(allies[0].transform.position) + moveDir, Vec3ToVec2(allies[1].transform.position)) > 0.05f
                && Vector2.Distance(Vec3ToVec2(allies[0].transform.position) + moveDir, Vec3ToVec2(allies[2].transform.position)) > 0.05f
                && !WillLandOnEnemy(moveDir))
            {
                FindObjectOfType<CursorTileDisplay>().ClearOverlay();
                MoveParty(moveDir);
                StartCoroutine(SpriteFadeOutFadeIn(allies[0].GetComponent<SpriteRenderer>(), 2f / moveSpeed));
            }
        }
    }

    public void AttemptAttack(Enemy enemy, Vector2 moveDir)
    {
        if (moveState == MoveState.NotMoving)
        {
            moveState = MoveState.Attack;
            allies[0].facingDirection = moveDir;
            allies[0].AttackEnemy(enemy);
            FindObjectOfType<CursorTileDisplay>().ClearOverlay();
            StartCoroutine(SpriteFadeOutFadeIn(allies[0].GetComponent<SpriteRenderer>(), 2f / moveSpeed));
            StartCoroutine(WaitToRotate(1f / moveSpeed));
        }
    }

    internal void AttemptSwap(Vector2 moveDir)
    {
        if (moveState == MoveState.NotMoving)
        {
            
            allies[1].facingDirection = -moveDir;
            allies[1].UpdateAnim(true, allies[1].facingDirection);
            allies[0].facingDirection = moveDir;
            allies[0].UpdateAnim(true, allies[0].facingDirection);

            Ally temp = allies[0];
            allies[0] = allies[1];
            allies[1] = temp;
            virCam.Follow = allies[0].transform;

            foreach (Ally ally in allies)
            {
                ally.SetMaxHealth();
            }
            FindObjectOfType<CursorTileDisplay>().ClearOverlay();
            moveState = MoveState.Swap;
        }
    }

    internal void AttemptStun(Enemy enemy)
    {
        if (moveState == MoveState.NotMoving)
        {
            moveState = MoveState.Stun;
            allies[0].StunEnemy(enemy);
            FindObjectOfType<CursorTileDisplay>().ClearOverlay();
            StartCoroutine(SpriteFadeOutFadeIn(allies[0].GetComponent<SpriteRenderer>(), 2f / moveSpeed));
            StartCoroutine(WaitToRotate(1f / moveSpeed));
        }
    }

    internal void AttemptPlaceBomb(Vector3 mousePos)
    {
        if (moveState == MoveState.NotMoving)
        {
            moveState = MoveState.Stun;
            allies[0].PlaceBomb(mousePos);
            FindObjectOfType<CursorTileDisplay>().ClearOverlay();
            StartCoroutine(SpriteFadeOutFadeIn(allies[0].GetComponent<SpriteRenderer>(), 2f / moveSpeed));
            StartCoroutine(WaitToRotate(1f / moveSpeed));
        }
    }

    internal void AttemptHealAlly(Ally ally)
    {
        if (moveState == MoveState.NotMoving)
        {
            moveState = MoveState.Heal;
            allies[0].HealAlly(ally);
            FindObjectOfType<CursorTileDisplay>().ClearOverlay();
            StartCoroutine(SpriteFadeOutFadeIn(allies[0].GetComponent<SpriteRenderer>(), 2f / moveSpeed));
            StartCoroutine(WaitToRotate(1f / moveSpeed));
        }
    }

    internal void AttemptCommand()
    {
        if (moveState == MoveState.NotMoving)
        {
            moveState = MoveState.Command;
            float damage = GameData.GetStatSum(allies[0].type, allies[3].type, allies[1].type, StatType.Unique);
            if (allies[1].Health > 0)
                allies[1].AttackAllAdjacentEnemies(damage);
            if (allies[2].Health > 0)
                allies[2].AttackAllAdjacentEnemies(damage);
            if (allies[3].Health > 0)
                allies[3].AttackAllAdjacentEnemies(damage);
            FindObjectOfType<CursorTileDisplay>().ClearOverlay();
            StartCoroutine(SpriteFadeOutFadeIn(allies[0].GetComponent<SpriteRenderer>(), 2f / moveSpeed));
            StartCoroutine(WaitToRotate(1f / moveSpeed));
        }
    }

    //public void AttemptHeavyAttack(Enemy enemy, Vector2 moveDir)
    //{
    //    if (moveState == MoveState.NotMoving)
    //    {
    //        moveState = MoveState.HeavyAttack;
    //        allies[0].facingDirection = moveDir;
    //        allies[0].HeavyAttackEnemy(enemy);
    //        gsm.EndTurn();
    //        //StartCoroutine(SpriteFadeOutFadeIn(allies[0].GetComponent<SpriteRenderer>(), 2f / moveSpeed));
    //        //StartCoroutine(WaitToRotate(1f / moveSpeed));
    //    }
    //}

    public void AttemptDetonate()
    {
        if (moveState == MoveState.NotMoving)
        {
            moveState = MoveState.Detonate;
            foreach (Bomb bomb in FindObjectsOfType<Bomb>())
            {
                bomb.Explode();
            }
            FindObjectOfType<CursorTileDisplay>().ClearOverlay();
            StartCoroutine(SpriteFadeOutFadeIn(allies[0].GetComponent<SpriteRenderer>(), 2f / moveSpeed));
            StartCoroutine(WaitToRotate(1f / moveSpeed));
        }
    }

    public void RotatePartyOrder()
    {
        Ally temp = allies[0];
        allies[0] = allies[1];
        allies[1] = allies[2];
        allies[2] = allies[3];
        allies[3] = temp;

        virCam.Follow = allies[0].transform;
    }
    public void PassTurn()
    {
        moveState = MoveState.PassingTurn;
        FindObjectOfType<CursorTileDisplay>().ClearOverlay();
        StartCoroutine(SpriteFadeOutFadeIn(allies[0].GetComponent<SpriteRenderer>(), 2f / moveSpeed));
        StartCoroutine(WaitToRotate(1f / moveSpeed));
    }

    private void MoveParty(Vector2 moveDir)
    {
        moveState = MoveState.Moving;
        movePoints[0] += Vec2ToVec3(moveDir);
        allies[0].facingDirection = moveDir;
        allies[0].UpdateAnim(true, allies[0].facingDirection);
        prevTail = allies[3].transform.position;
        for (int i = 1; i < 4; i++)
        {
            movePoints[i] = allies[i - 1].transform.position;
            allies[i].facingDirection = Vec3ToVec2(movePoints[i] - allies[i].transform.position);
            allies[i].UpdateAnim(true, allies[i].facingDirection);
        }
    }

    private void RotateParty()
    {
        moveState = MoveState.Rotating;

        allies[0].transform.position = prevTail;

        RotatePartyOrder();
        AudioManager.instance.Play("Step");
        prevTail = allies[3].transform.position;
        allies[0].facingDirection = Vec3ToVec2(movePoints[0] - allies[0].transform.position);
        allies[0].UpdateAnim(true, allies[0].facingDirection);
        for (int i = 1; i < 4; i++)
        {
            movePoints[i] = allies[i - 1].transform.position;
            allies[i].facingDirection = Vec3ToVec2(movePoints[i] - allies[i].transform.position);
            allies[i].UpdateAnim(true, allies[i].facingDirection);
        }
    }

    public void SetPartyOrder(AllyType ally1, AllyType ally2, AllyType ally3, AllyType ally4)
    {
        Ally newA1 = GetAlly(ally1);
        Ally newA2 = GetAlly(ally2);
        Ally newA3 = GetAlly(ally3);
        Ally newA4 = GetAlly(ally4);

        Vector3 oldA1Pos = allies[0].transform.position;
        Vector3 oldA2Pos = allies[1].transform.position;
        Vector3 oldA3Pos = allies[2].transform.position;
        Vector3 oldA4Pos = allies[3].transform.position;

        Vector2 oldA1FaceDir = allies[0].facingDirection;
        Vector2 oldA2FaceDir = allies[1].facingDirection;
        Vector2 oldA3FaceDir = allies[2].facingDirection;
        Vector2 oldA4FaceDir = allies[3].facingDirection;

        allies[0] = newA1;
        allies[1] = newA2;
        allies[2] = newA3;
        allies[3] = newA4;

        allies[0].transform.position = oldA1Pos;
        allies[1].transform.position = oldA2Pos;
        allies[2].transform.position = oldA3Pos;
        allies[3].transform.position = oldA4Pos;

        allies[0].UpdateAnim(oldA1FaceDir);
        allies[1].UpdateAnim(oldA2FaceDir);
        allies[2].UpdateAnim(oldA3FaceDir);
        allies[3].UpdateAnim(oldA4FaceDir);

        virCam.Follow = allies[0].transform;

        foreach (Ally ally in allies)
        {
            ally.SetMaxHealth();
        }

        UIManager.instance.UpdateUI();
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

    private IEnumerator WaitToRotate(float duration)
    {
        yield return new WaitForSeconds(duration);
        RotateParty();
    }

    private bool WillLandOnEnemy(Vector2 moveDir)
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            if (Vector2.Distance(Vec3ToVec2(allies[0].transform.position) + moveDir, Vec3ToVec2(enemy.transform.position)) <= 0.05f)
                return true;
        }
        return false;
    }

    public Ally GetAlly(AllyType type)
    {
        List<Ally> temp = new List<Ally>(allies);
        int index = temp.FindIndex(ally => ally.type == type);
        return allies[index];
    }

    public Ally[] GetNeighborAllies(Ally ally)
    {
        List<Ally> temp = new List<Ally>(allies);
        int id = temp.FindIndex(a => a.type == ally.type);
        Ally upperNeighbor;
        Ally lowerNeighbor;

        if (id == 0)
        {
            upperNeighbor = allies[3];
            lowerNeighbor = allies[1];
        }
        else if (id == 3)
        {
            upperNeighbor = allies[2];
            lowerNeighbor = allies[0];
        }
        else
        {
            upperNeighbor = allies[id - 1];
            lowerNeighbor = allies[id + 1];
        }
        return new Ally[] { upperNeighbor, lowerNeighbor };
    }
}
public enum MoveState
{
    NotMoving,
    Moving,
    Rotating,
    SpecialAction,
    PassingTurn,
    Attack,
    Swap,
    Command,
    Heal,
    Stun,
    Detonate
}
