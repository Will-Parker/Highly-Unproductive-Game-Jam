using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Helpers;

public class CharacterControl : MonoBehaviour
{
    // Reference to PartyManager component
    private PartyManager pm;
    // Unity New Input System Controls
    private Controls controls;
    // points each ally moves toward
    private Vector3[] movePoints = new Vector3[4]; // hardcoded to 4
    // impassable layer
    [SerializeField] private LayerMask impassableLayer;
    // how quickly it moves to next tile
    [SerializeField] [Range(0, 20)] private float moveSpeed;

    private MoveState moveState = MoveState.NotMoving;

    private Vector3 prevTail;

    private void Awake()
    {
        pm = GetComponent<PartyManager>();
        if (pm == null)
            Debug.Log("PartyManager component not added");

        // Enable Gameplay controls
        controls = new Controls();
        controls.Gameplay.Enable();
        controls.Gameplay.Move.performed += Move_performed;
        controls.Gameplay.SpecialAction.performed += SpecialAction_performed;
    }

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            movePoints[i] = pm.allies[i].transform.position;
            //pm.allies[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }

        prevTail = new Vector3(pm.allies[3].transform.position.x - 1, pm.allies[3].transform.position.y, pm.allies[3].transform.position.z);

        //Time.timeScale = 0.1f;
    }

    private void Update()
    {
        if (moveState != MoveState.NotMoving)
        {
            for (int i = 0; i < 4; i++)
                pm.allies[i].transform.position = Vector3.MoveTowards(pm.allies[i].transform.position, movePoints[i], moveSpeed * Time.deltaTime);
            
            if (moveState == MoveState.Moving)
            {
                if (Vector3.Distance(pm.allies[0].transform.position, movePoints[0]) <= 0.05f)
                {
                    for (int i = 0; i < 4; i++)
                        pm.allies[i].transform.position = movePoints[i];
                    RotateParty();
                }
            }
            else if (moveState == MoveState.Rotating)
            {
                if (Vector3.Distance(pm.allies[0].transform.position, movePoints[0]) <= 0.05f)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        pm.allies[i].transform.position = movePoints[i];
                        //pm.allies[i].UpdateAnim(false);
                    }
                    moveState = MoveState.NotMoving;
                    FindObjectOfType<GameStateManager>().EndTurn();
                }
            }
        }
    }

    private void Move_performed(InputAction.CallbackContext context)
    {
        GameStateManager gsm = FindObjectOfType<GameStateManager>();
        if (gsm.GetGameState() == GameState.Player)
        {
            Vector2 inputVector = context.ReadValue<Vector2>();
            if (moveState == MoveState.NotMoving)
            {
                if (Mathf.Abs(inputVector.x) == 1f)
                {
                    Vector2 moveDir = new(inputVector.x, 0f);
                    RaycastHit2D ray = Physics2D.Raycast(Vec3ToVec2(pm.allies[0].transform.position), moveDir, 1f, impassableLayer);
                    if (!ray
                        && Vector2.Distance(Vec3ToVec2(pm.allies[0].transform.position) + moveDir, Vec3ToVec2(pm.allies[1].transform.position)) > 0.05f
                        && Vector2.Distance(Vec3ToVec2(pm.allies[0].transform.position) + moveDir, Vec3ToVec2(pm.allies[2].transform.position)) > 0.05f
                        && !WillLandOnEnemy(moveDir))
                    {
                        MoveParty(moveDir);
                        StartCoroutine(SpriteFadeOutFadeIn(pm.allies[0].GetComponent<SpriteRenderer>(), 2f / moveSpeed));
                    }
                }
                else if (Mathf.Abs(inputVector.y) == 1f)
                {
                    Vector2 moveDir = new(0f, inputVector.y);
                    RaycastHit2D ray = Physics2D.Raycast(Vec3ToVec2(pm.allies[0].transform.position), moveDir, 1f, impassableLayer);
                    if (!ray
                        && Vector2.Distance(Vec3ToVec2(pm.allies[0].transform.position) + moveDir, Vec3ToVec2(pm.allies[1].transform.position)) > 0.05f
                        && Vector2.Distance(Vec3ToVec2(pm.allies[0].transform.position) + moveDir, Vec3ToVec2(pm.allies[2].transform.position)) > 0.05f
                        && !WillLandOnEnemy(moveDir))
                    {
                        MoveParty(moveDir);
                        StartCoroutine(SpriteFadeOutFadeIn(pm.allies[0].GetComponent<SpriteRenderer>(), 2f / moveSpeed));
                    }
                }
            }
        }
    }

    private void SpecialAction_performed(InputAction.CallbackContext context)
    {
        GameStateManager gsm = FindObjectOfType<GameStateManager>();
        if (gsm.GetGameState() == GameState.Player)
        {
            StartCoroutine(SpriteFadeOutFadeIn(pm.allies[0].GetComponent<SpriteRenderer>(), 2f / moveSpeed));
            StartCoroutine(WaitToRotate(1f / moveSpeed));
        }
    }

    private void MoveParty(Vector2 moveDir)
    {
        movePoints[0] += Vec2ToVec3(moveDir);
        //pm.allies[0].UpdateAnim(true, moveDir);
        prevTail = pm.allies[3].transform.position;
        for (int i = 1; i < 4; i++)
        {
            movePoints[i] = pm.allies[i - 1].transform.position;
            //pm.allies[i].UpdateAnim(true, Vec3ToVec2(movePoints[i] - pm.allies[i].transform.position));
        }
        moveState = MoveState.Moving;
    }

    private void RotateParty()
    {
        moveState = MoveState.Rotating;

        pm.allies[0].transform.position = prevTail;

        pm.RotatePartyPositions();

        prevTail = pm.allies[3].transform.position;
        //pm.allies[0].UpdateAnim(true, Vec3ToVec2(movePoints[0] - pm.allies[0].transform.position));
        for (int i = 1; i < 4; i++)
        {
            movePoints[i] = pm.allies[i - 1].transform.position;
            //pm.allies[i].UpdateAnim(true, Vec3ToVec2(movePoints[i] - pm.allies[i].transform.position));
        }
    }

    private enum MoveState
    {
        NotMoving,
        Moving,
        Rotating
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
            if (Vector2.Distance(Vec3ToVec2(pm.allies[0].transform.position) + moveDir, Vec3ToVec2(enemy.transform.position)) <= 0.05f)
                return true;
        }
        return false;
    }
}
