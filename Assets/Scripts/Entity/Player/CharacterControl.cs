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
    private ActionUIManager auim;
    private CursorTileDisplay ctd;

    private void Awake()
    {
        pm = GetComponent<PartyManager>();
        if (pm == null)
            Debug.LogWarning("PartyManager component not added");
        auim = FindObjectOfType<ActionUIManager>();
        if (auim == null)
            Debug.LogWarning("Action UI Manager is not in the scene");
        ctd = FindObjectOfType<CursorTileDisplay>();
        if (ctd == null)
            Debug.LogWarning("CursorTileDisplay is not in the scene");

        // Enable Gameplay controls
        controls = new Controls();
        controls.Gameplay.Enable();
        controls.Gameplay.Move.performed += Move_performed;
        controls.Gameplay.SpecialAction.performed += SpecialAction_performed;
        controls.Gameplay.Click.performed += Click_performed;

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
                        if (auim.mode == UIActionMode.HeavyAttack)
                            pm.AttemptHeavyAttack(enemy, moveDir);
                        else
                        {
                            pm.AttemptAttack(enemy, moveDir);
                            auim.UpdateActionUI(UIActionMode.Attack);
                        }
                        successfulAttempt = true;
                    }
                }
            }
            if (!successfulAttempt)
            {
                if (Vector2.Distance(Vec3ToVec2(pm.allies[1].transform.position), Vec3ToVec2(pm.allies[0].transform.position) + moveDir) <= 0.05)
                {
                    auim.UpdateActionUI(UIActionMode.Swap);
                    pm.AttemptSwap(moveDir);
                    successfulAttempt = true;
                }
            }
            if (!successfulAttempt)
            {
                auim.UpdateActionUI(UIActionMode.Move);
                pm.AttemptMove(moveDir);
            }
        }
    }

    private void SpecialAction_performed(InputAction.CallbackContext context)
    {
        switch (pm.allies[0].type)
        {
            case AllyType.Apple:
                auim.UpdateActionUI(UIActionMode.HeavyAttack);
                break;
            case AllyType.Strawberry:
                auim.UpdateActionUI(UIActionMode.Heal);
                break;
            case AllyType.Lemon:
                auim.UpdateActionUI(UIActionMode.Stun);
                break;
            case AllyType.Blueberry:
                auim.UpdateActionUI(UIActionMode.Bomb);
                break;
        }
    }

    private void SpecialAction_canceled(InputAction.CallbackContext context)
    {
        auim.UpdateActionUI(UIActionMode.None);
    }

    private void Click_performed(InputAction.CallbackContext context)
    {
        if (pm.moveState == MoveState.NotMoving && (auim.mode == UIActionMode.HeavyAttack || auim.mode == UIActionMode.Heal || auim.mode == UIActionMode.Stun || auim.mode == UIActionMode.Bomb || auim.mode == UIActionMode.Move || auim.mode == UIActionMode.Attack))
        {
            Vector3 mousePos = Vec2IntToVec3(Vec3IntToVec2Int(ctd.GetMousePosition()));
            Vector2 moveDir = Vec3ToVec2(mousePos - pm.allies[0].transform.position);
            switch (auim.mode)
            {
                case UIActionMode.Attack:
                    if (moveDir.magnitude <= 1.05)
                    {
                        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                        {
                            if (Vector3.Distance(enemy.transform.position, mousePos) <= 0.05)
                            {
                                pm.AttemptAttack(enemy, moveDir);
                            }
                        }
                    }
                    break;
                case UIActionMode.HeavyAttack:
                    if (moveDir.magnitude <= 1.05)
                    {
                        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                        {
                            if (Vector3.Distance(enemy.transform.position, mousePos) <= 0.05)
                            {
                                pm.AttemptHeavyAttack(enemy, moveDir);
                            }
                        }
                    }
                    break;
                case UIActionMode.Move:
                    if (moveDir.sqrMagnitude <= 1.05 && moveDir.sqrMagnitude > 0.05)
                    {
                        pm.AttemptMove(moveDir);
                    }
                    break;
            }
        }
    }
}
