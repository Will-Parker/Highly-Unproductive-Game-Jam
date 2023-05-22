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
    [HideInInspector] public static Controls controls;

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

        controls.Gameplay.SpecialAction.canceled += SpecialAction_canceled;
    }

    

    private void Move_performed(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();
        bool isXInputMax = Mathf.Abs(inputVector.x) == 1f;
        bool isYInputMax = Mathf.Abs(inputVector.y) == 1f;
        Vector2 moveDir = Vector2.zero;
        if (isXInputMax)
            moveDir = new(inputVector.x, 0f);
        else if (isYInputMax)
            moveDir = new(0f, inputVector.y);
        if (isXInputMax || isYInputMax)
        {
            bool successfulAttempt = false;
            if (!successfulAttempt)
            {
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    if (Vector2.Distance(Vec3ToVec2(enemy.transform.position), Vec3ToVec2(pm.allies[0].transform.position) + moveDir) <= 0.05)
                    {
                        Debug.Log("Attempt attack");
                        pm.AttemptAttack(enemy, moveDir);
                        successfulAttempt = true;
                    }
                        
                }
            }
            if (!successfulAttempt)
            {
                if (Vector2.Distance(Vec3ToVec2(pm.allies[1].transform.position), Vec3ToVec2(pm.allies[0].transform.position) + moveDir) <= 0.05)
                {
                    Debug.Log("Attempt swap");
                    pm.AttemptSwap(moveDir);
                    successfulAttempt = true;
                }
            }
            if (!successfulAttempt)
            {
                Debug.Log("Attempt move");
                pm.AttemptMove(moveDir);
            }
        }
    }

    private void SpecialAction_performed(InputAction.CallbackContext context)
    {
        switch (pm.allies[0].type)
        {
            case AllyType.Apple:
                break;
            case AllyType.Strawberry:
                break;
            case AllyType.Lemon:
                break;
            case AllyType.Blueberry:
                break;
        }
        //pm.AttemptRotate();
    }

    private void SpecialAction_canceled(InputAction.CallbackContext context)
    {
        
    }
}
