using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;
    [SerializeField] private GameState gameState = GameState.Player;
    private UIManager uim;
    private int turn = 1;

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

        uim = GetComponent<UIManager>();
    }

    private void Update()
    {
        switch (gameState)
        {
            case GameState.Player:
                break;
            case GameState.Enemy:
                var enemies = new List<Enemy>(FindObjectsOfType<Enemy>()); // probably should not update enemies array every frame
                enemies.RemoveAll(enemy => !enemy.isActiveAndEnabled);
                if (enemies.All(enemy => enemy.hasFinishedTurn))
                    EndTurn();
                break;
        }
    }

    public void EndTurn()
    {
        switch (gameState)
        {
            case GameState.Player:
                CharacterControl.controls.Gameplay.Disable();
                gameState = GameState.Enemy;
                turn++;
                Debug.Log("Turn " + turn);
                uim.UpdateUI();
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    enemy.UpdateAIState();
                    enemy.CalculateAction();
                }
                
                break;
            case GameState.Enemy:
                gameState = GameState.Player;
                PartyManager pm = FindObjectOfType<PartyManager>();
                if (pm.allies[0].Health <= 0)
                    pm.PassTurn();
                CharacterControl.controls.Gameplay.Enable();
                break;
        }
    }

    public GameState GetGameState()
    {
        return gameState;
    }
}

public enum GameState
{
    Player,
    Enemy
}
