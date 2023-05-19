using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;
    [SerializeField] private GameState gameState = GameState.Player;
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
    }

    private void Update()
    {
        switch (gameState)
        {
            case GameState.Player:
                break;
            case GameState.Enemy:
                Enemy[] enemies = FindObjectsOfType<Enemy>(); // probably should not update enemies array every frame
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
                turn++;
                Debug.Log("Turn " + turn);
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    enemy.UpdateAIState();
                    enemy.CalculateAction();
                }
                gameState = GameState.Enemy;
                break;
            case GameState.Enemy:
                gameState = GameState.Player;
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
