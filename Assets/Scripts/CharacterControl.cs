using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterControl : MonoBehaviour
{
    // Reference to Rigidbody component
    // Temporary probably
    private Rigidbody2D rb;
    // Unity New Input System Controls
    public Controls controls;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.Log("Rigidbody2D could not be found");

        // Enable Gameplay controls
        controls = new Controls();
        controls.Gameplay.Enable();
        controls.Gameplay.Move.performed += Move_performed;
    }

    private void Move_performed(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();
        // TODO: Movement along grid
        // TODO: animate movement
        // TODO: move all allies
        // TODO: move camera (actually probably not handled here)
        rb.AddForce(new Vector2(inputVector.x, inputVector.y) * 2f, ForceMode2D.Impulse);
    }
}
