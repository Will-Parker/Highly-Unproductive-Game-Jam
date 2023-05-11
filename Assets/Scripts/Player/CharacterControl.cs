using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    }

    private void Start()
    {
        for (int i = 0; i < 4; i++)
            movePoints[i] = pm.allies[i].transform.position;
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
                    Color allyColor = pm.allies[0].GetComponent<SpriteRenderer>().color;
                    pm.allies[0].GetComponent<SpriteRenderer>().color = new Color(allyColor.r, allyColor.g, allyColor.b, 0);
                    for (int i = 0; i < 4; i++)
                        pm.allies[i].transform.position = movePoints[i];
                    RotateParty();
                }
            }
            else if (moveState == MoveState.Rotating)
            {
                if (Vector3.Distance(pm.allies[0].transform.position, movePoints[0]) <= 0.05f)
                {
                    Color allyColor = pm.allies[3].GetComponent<SpriteRenderer>().color;
                    pm.allies[3].GetComponent<SpriteRenderer>().color = new Color(allyColor.r, allyColor.g, allyColor.b, 1);
                    for (int i = 0; i < 4; i++)
                        pm.allies[i].transform.position = movePoints[i];
                    moveState = MoveState.NotMoving;
                }
            }
        }
    }

    private void Move_performed(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();
        if (moveState == MoveState.NotMoving)
        {
            if (Mathf.Abs(inputVector.x) == 1f)
            {
                Vector2 moveDir = new (inputVector.x, 0f);
                RaycastHit2D ray = Physics2D.Raycast(Vec3ToVec2(pm.allies[0].transform.position), moveDir, 1f, impassableLayer);
                if (!ray)
                {
                    MoveParty(moveDir);
                }
            }
            else if (Mathf.Abs(inputVector.y) == 1f)
            {
                Vector2 moveDir = new (0f, inputVector.y);
                RaycastHit2D ray = Physics2D.Raycast(Vec3ToVec2(pm.allies[0].transform.position), moveDir, 1f, impassableLayer);
                if (!ray)
                {
                    MoveParty(moveDir);
                }
            }
        }
        // TODO: animate movement
        // TODO: move camera (actually probably not handled here)
    }

    private void MoveParty(Vector2 moveDir)
    {
        movePoints[0] += Vec2ToVec3(moveDir);
        prevTail = pm.allies[3].transform.position;
        for (int i = 1; i < 4; i++)
            movePoints[i] = pm.allies[i - 1].transform.position;
        moveState = MoveState.Moving;
        StartCoroutine(SpriteFade(pm.allies[0].GetComponent<SpriteRenderer>(), 0f, 1f / moveSpeed));
    }

    private void RotateParty()
    {
        moveState = MoveState.Rotating;

        pm.allies[0].transform.position = prevTail;

        pm.RotatePartyPositions();

        prevTail = pm.allies[3].transform.position;
        for (int i = 1; i < 4; i++)
            movePoints[i] = pm.allies[i - 1].transform.position;

        StartCoroutine(SpriteFade(pm.allies[3].GetComponent<SpriteRenderer>(), 1f, 1f / moveSpeed));
    }

    private Vector2 Vec3ToVec2(Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    private Vector3 Vec2ToVec3(Vector2 v)
    {
        return new Vector3(v.x, v.y, 0f);
    }


    private enum MoveState
    {
        NotMoving,
        Moving,
        Rotating
    }

    // Credit: https://answers.unity.com/questions/1687634/how-do-i-mathflerp-the-spriterendereralpha.html
    private IEnumerator SpriteFade(SpriteRenderer sr, float endValue, float duration)
    {
        float elapsedTime = 0;
        float startValue = sr.color.a;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, newAlpha);
            yield return null;
        }
    }
}
