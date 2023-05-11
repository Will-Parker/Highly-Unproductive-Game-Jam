using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterControl : MonoBehaviour
{
    // Unity New Input System Controls
    private Controls controls;
    // point player moves toward
    [SerializeField] private Transform movePoint;
    // impassable layer
    [SerializeField] private LayerMask impassableLayer;

    [SerializeField] [Range(0, 20)] private float moveSpeed;

    private void Awake()
    {
        if (movePoint == null)
            Debug.Log("Move Point not assigned");

        // Enable Gameplay controls
        controls = new Controls();
        controls.Gameplay.Enable();
        controls.Gameplay.Move.performed += Move_performed;
    }

    private void Start()
    {
        movePoint.parent = null;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
    }

    private void Move_performed(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();
        if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
        {
            transform.position = movePoint.position;
            if (Mathf.Abs(inputVector.x) == 1f)
            {
                Vector2 moveDir = new (inputVector.x, 0f);
                RaycastHit2D ray = Physics2D.Raycast(Vec3ToVec2(transform.position), moveDir, 1f, impassableLayer);
                if (!ray)
                {
                    movePoint.position += Vec2ToVec3(moveDir);
                }
            }
            else if (Mathf.Abs(inputVector.y) == 1f)
            {
                Vector2 moveDir = new (0f, inputVector.y);
                RaycastHit2D ray = Physics2D.Raycast(Vec3ToVec2(transform.position), moveDir, 1f, impassableLayer);
                if (!ray)
                {
                    movePoint.position += Vec2ToVec3(moveDir);
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
