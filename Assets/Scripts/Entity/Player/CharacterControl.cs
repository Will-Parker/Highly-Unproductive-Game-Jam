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

    private void Move_performed(InputAction.CallbackContext context)
    {
        GameStateManager gsm = FindObjectOfType<GameStateManager>();
        if (gsm.GetGameState() == GameState.Player)
        {
            Vector2 inputVector = context.ReadValue<Vector2>();
            if (Mathf.Abs(inputVector.x) == 1f)
            {
                Vector2 moveDir = new(inputVector.x, 0f);
                pm.AttemptMove(moveDir);
            }
            else if (Mathf.Abs(inputVector.y) == 1f)
            {
                Vector2 moveDir = new(0f, inputVector.y);
                pm.AttemptMove(moveDir);
            }
        }
    }

    private void SpecialAction_performed(InputAction.CallbackContext context)
    {
        GameStateManager gsm = FindObjectOfType<GameStateManager>();
        if (gsm.GetGameState() == GameState.Player)
        {
            pm.AttemptRotate();
        }
    }
}
