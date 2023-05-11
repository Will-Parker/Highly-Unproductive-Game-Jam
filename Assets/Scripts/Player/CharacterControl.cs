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
        for (int i = 0; i < 4; i++)
            pm.allies[i].transform.position = Vector3.MoveTowards(pm.allies[i].transform.position, movePoints[i], moveSpeed * Time.deltaTime);
    }

    private void Move_performed(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();
        if (Vector3.Distance(pm.allies[0].transform.position, movePoints[0]) <= 0.05f)
        {
            for (int i = 0; i < 4; i++)
                pm.allies[i].transform.position = movePoints[i];

            if (Mathf.Abs(inputVector.x) == 1f)
            {
                Vector2 moveDir = new (inputVector.x, 0f);
                RaycastHit2D ray = Physics2D.Raycast(Vec3ToVec2(pm.allies[0].transform.position), moveDir, 1f, impassableLayer);
                if (!ray)
                {
                    movePoints[0] += Vec2ToVec3(moveDir);
                    for (int i = 1; i < 4; i++)
                        movePoints[i] = pm.allies[i - 1].transform.position;
                }
            }
            else if (Mathf.Abs(inputVector.y) == 1f)
            {
                Vector2 moveDir = new (0f, inputVector.y);
                RaycastHit2D ray = Physics2D.Raycast(Vec3ToVec2(pm.allies[0].transform.position), moveDir, 1f, impassableLayer);
                if (!ray)
                {
                    movePoints[0] += Vec2ToVec3(moveDir);
                    for (int i = 1; i < 4; i++)
                        movePoints[i] = pm.allies[i - 1].transform.position;
                }
            }
        }
        // TODO: animate movement
        // TODO: move all allies
        // TODO: move camera (actually probably not handled here)
    }


    private Vector2 Vec3ToVec2(Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    private Vector3 Vec2ToVec3(Vector2 v)
    {
        return new Vector3(v.x, v.y, 0f);
    }
}
