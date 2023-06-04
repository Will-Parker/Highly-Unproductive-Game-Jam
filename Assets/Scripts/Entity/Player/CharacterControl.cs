using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Helpers;
using UnityEngine.SceneManagement;

public class CharacterControl : MonoBehaviour
{
    public static CharacterControl instance;

    // Unity New Input System Controls
    [HideInInspector] public static Controls controls;

    private static bool progressDialoguePressed = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // Enable Gameplay controls
        controls = new Controls();
        controls.Gameplay.Enable();
        controls.Gameplay.Move.performed += Move_performed;
        controls.Gameplay.SpecialAction.performed += SpecialAction_performed;
        controls.Gameplay.Click.performed += Click_performed;
        controls.Gameplay.Detonate.performed += Detonate_performed;
        controls.Gameplay.Pause.performed += Pause_performed;
        // controls.Gameplay.ProgressDialogue.performed += ProgressDialogue_performed;
        // controls.Gameplay.DebugOpenDialogue.performed += DebugOpenDialogue_performed;

        controls.Gameplay.SpecialAction.canceled += SpecialAction_canceled;

        controls.Gameplay.TestAction.performed += TestAction_performed;
    }

    private void TestAction_performed(InputAction.CallbackContext obj)
    {
        SceneManager.LoadSceneAsync("Level Template");
    }

    private void Move_performed(InputAction.CallbackContext context)
    {
        PartyManager pm = FindObjectOfType<PartyManager>();
        if (pm == null)
        {
            Debug.LogWarning("PartyManager is not in the scene");
            return;
        }
        ActionUIManager auim = FindObjectOfType<ActionUIManager>();
        if (auim == null)
        {
            Debug.LogWarning("Action UI Manager is not in the scene");
            return;
        }
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
                            Debug.LogWarning("Heavy Attack Removed");
                            //pm.AttemptHeavyAttack(enemy, moveDir);
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
        PartyManager pm = FindObjectOfType<PartyManager>();
        if (pm == null)
        {
            Debug.LogWarning("PartyManager is not in the scene");
            return;
        }
        ActionUIManager auim = FindObjectOfType<ActionUIManager>();
        if (auim == null)
        {
            Debug.LogWarning("Action UI Manager is not in the scene");
            return;
        }
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
        ActionUIManager auim = FindObjectOfType<ActionUIManager>();
        if (auim == null)
        {
            Debug.LogWarning("Action UI Manager is not in the scene");
            return;
        }
        auim.UpdateActionUI(UIActionMode.None);
    }

    private void Click_performed(InputAction.CallbackContext context)
    {
        PartyManager pm = FindObjectOfType<PartyManager>();
        if (pm == null)
        {
            Debug.LogWarning("PartyManager is not in the scene");
            return;
        }
        ActionUIManager auim = FindObjectOfType<ActionUIManager>();
        if (auim == null)
        {
            Debug.LogWarning("Action UI Manager is not in the scene");
            return;
        }
        CursorTileDisplay ctd = FindObjectOfType<CursorTileDisplay>();
        if (ctd == null)
        {
            Debug.LogWarning("CursorTileDisplay is not in the scene");
            return;
        }
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
                    Debug.LogWarning("Heavy Attack Removed");
                    //if (moveDir.magnitude <= 1.05)
                    //{
                    //    foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                    //    {
                    //        if (Vector3.Distance(enemy.transform.position, mousePos) <= 0.05)
                    //        {
                    //            pm.AttemptHeavyAttack(enemy, moveDir);
                    //        }
                    //    }
                    //}
                    break;
                case UIActionMode.Move:
                    if (moveDir.sqrMagnitude <= 1.05 && moveDir.sqrMagnitude > 0.05)
                    {
                        pm.AttemptMove(moveDir);
                    }
                    break;
                case UIActionMode.Heal:
                    foreach (Ally ally in pm.allies)
                    {
                        if (Vector3.Distance(ally.transform.position, mousePos) <= 0.05)
                        {
                            pm.AttemptHealAlly(ally);
                        }
                    }
                    break;
                case UIActionMode.Stun:
                    foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                    {
                        if (Vector3.Distance(enemy.transform.position, mousePos) <= 0.05)
                        {
                            pm.AttemptStun(enemy);
                        }
                    }
                    break;
                case UIActionMode.Bomb:
                    pm.AttemptPlaceBomb(mousePos);
                    break;
            }
        }
    }

    private void Detonate_performed(InputAction.CallbackContext obj)
    {
        PartyManager pm = FindObjectOfType<PartyManager>();
        if (pm == null)
        {
            Debug.LogWarning("PartyManager is not in the scene");
            return;
        }
        ActionUIManager auim = FindObjectOfType<ActionUIManager>();
        if (auim == null)
        {
            Debug.LogWarning("Action UI Manager is not in the scene");
            return;
        }
        if (FindObjectsOfType<Bomb>().Length > 0)
        {
            auim.UpdateActionUI(UIActionMode.Detonate);
            pm.AttemptDetonate();
        }
    }

    private void ProgressDialogue_performed(InputAction.CallbackContext obj)
    {
        if (!UIManager.instance.pausem.isActiveAndEnabled)
        {
            progressDialoguePressed = true;

            if (!DialogueManager.GetInstance().dialogueIsPlaying)
            {
                SubToAllGameplayActions();
                UnsubFromAllDialogueActions();
            }
        }
    }

    private void ProgressDialogue_canceled(InputAction.CallbackContext obj)
    {
        progressDialoguePressed = false;

        if (!DialogueManager.GetInstance().dialogueIsPlaying)
        {
            SubToAllGameplayActions();
            UnsubFromAllDialogueActions();
        }
    }

    public static bool GetProgressDialoguePressed()
    {
        bool result = progressDialoguePressed;
        progressDialoguePressed = false;
        return result;
    }

    public void OpenDialogue(TextAsset inkJSON)
    {
        DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
        UnsubFromAllGameplayActions();
        SubToAllDialogueActions();
    }

    private void Pause_performed(InputAction.CallbackContext obj)
    {
        FindObjectOfType<UIManager>().Pause();
    }

    public void UnsubFromAllGameplayActions()
    {
        controls.Gameplay.Move.performed -= Move_performed;
        controls.Gameplay.SpecialAction.performed -= SpecialAction_performed;
        controls.Gameplay.Click.performed -= Click_performed;
        controls.Gameplay.Detonate.performed -= Detonate_performed;

        controls.Gameplay.SpecialAction.canceled -= SpecialAction_canceled;
    }

    public void SubToAllGameplayActions()
    {
        controls.Gameplay.Move.performed += Move_performed;
        controls.Gameplay.SpecialAction.performed += SpecialAction_performed;
        controls.Gameplay.Click.performed += Click_performed;
        controls.Gameplay.Detonate.performed += Detonate_performed;

        controls.Gameplay.SpecialAction.canceled += SpecialAction_canceled;
    }

    public void UnsubFromAllDialogueActions()
    {
        controls.Gameplay.ProgressDialogue.performed -= ProgressDialogue_performed;
        controls.Gameplay.ProgressDialogue.canceled -= ProgressDialogue_canceled;
    }

    public void SubToAllDialogueActions()
    {
        controls.Gameplay.ProgressDialogue.performed += ProgressDialogue_performed;
        controls.Gameplay.ProgressDialogue.canceled += ProgressDialogue_canceled;
    }

    public void UnsubFromEverything()
    {
        controls.Gameplay.Move.performed -= Move_performed;
        controls.Gameplay.SpecialAction.performed -= SpecialAction_performed;
        controls.Gameplay.Click.performed -= Click_performed;
        controls.Gameplay.Detonate.performed -= Detonate_performed;
        controls.Gameplay.Pause.performed -= Pause_performed;
        controls.Gameplay.ProgressDialogue.performed -= ProgressDialogue_performed;
        controls.Gameplay.ProgressDialogue.canceled -= ProgressDialogue_canceled;
        controls.Gameplay.SpecialAction.canceled -= SpecialAction_canceled;
    }

    public void SubToPause()
    {
        controls.Gameplay.Pause.performed += Pause_performed;
    }
}
