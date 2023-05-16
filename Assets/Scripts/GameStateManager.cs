using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;
    [SerializeField] private GameState gameState = GameState.Player;
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
    public void EndTurn()
    {
        switch (gameState)
        {
            case GameState.Player:
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
