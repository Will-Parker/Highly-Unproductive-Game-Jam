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
    private float exp;
    private float maxExp;
    [SerializeField] private Experiencebar expBar;

    private GameStateManager gsm;
    private void Awake()
    {
        gsm = FindObjectOfType<GameStateManager>();
        if (gsm == null)
            Debug.LogWarning("No GSM in scene");
    }
    // Start is called before the first frame update
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
        maxExp = 1;
        expBar.SetMaxExperience(maxExp);
        expBar.SetExperience(0);
    }

    // Update is called once per frame
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
                MoveParty(moveDir);
                StartCoroutine(SpriteFadeOutFadeIn(allies[0].GetComponent<SpriteRenderer>(), 2f / moveSpeed));
            }
        }
    }

    public void AttemptRotate()
    {
        if (moveState == MoveState.NotMoving)
        {
            moveState = MoveState.SpecialAction;
            allies[0].PeformSpecialAction();
            StartCoroutine(SpriteFadeOutFadeIn(allies[0].GetComponent<SpriteRenderer>(), 2f / moveSpeed));
            StartCoroutine(WaitToRotate(1f / moveSpeed));
        }
    }

    public void AttemptAttack(Enemy enemy, Vector2 moveDir)
    {
        if (moveState == MoveState.NotMoving)
        {
            moveState = MoveState.Attack;
            allies[0].facingDirection = moveDir;
            allies[0].AttackEnemy(enemy);
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

            moveState = MoveState.Swap;
        }
    }

    internal void AttemptStun(Enemy enemy)
    {
        if (moveState == MoveState.NotMoving)
        {
            moveState = MoveState.Stun;
            allies[0].StunEnemy(enemy);
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
            StartCoroutine(SpriteFadeOutFadeIn(allies[0].GetComponent<SpriteRenderer>(), 2f / moveSpeed));
            StartCoroutine(WaitToRotate(1f / moveSpeed));
        }
    }

    public void AttemptHeavyAttack(Enemy enemy, Vector2 moveDir)
    {
        if (moveState == MoveState.NotMoving)
        {
            moveState = MoveState.HeavyAttack;
            allies[0].facingDirection = moveDir;
            allies[0].HeavyAttackEnemy(enemy);
            gsm.EndTurn();
            //StartCoroutine(SpriteFadeOutFadeIn(allies[0].GetComponent<SpriteRenderer>(), 2f / moveSpeed));
            //StartCoroutine(WaitToRotate(1f / moveSpeed));
        }
    }

    public void AttemptDetonate()
    {
        if (moveState == MoveState.NotMoving)
        {
            moveState = MoveState.Detonate;
            foreach (Bomb bomb in FindObjectsOfType<Bomb>())
            {
                bomb.Explode();
            }
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

    public void GainExperience(float experienceToAdd)
    {
        exp = Mathf.Min(exp + experienceToAdd, maxExp);
        expBar.SetExperience(exp);
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
    HeavyAttack,
    Heal,
    Stun,
    Detonate
}
