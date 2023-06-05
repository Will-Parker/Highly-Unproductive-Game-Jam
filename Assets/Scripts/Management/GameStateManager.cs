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
    }

    private void Start()
    {
        StartCoroutine(AmbientSounds());
        CharacterControl.controls.Gameplay.Enable();
        CharacterControl.instance.SubToAllGameplayActions();
        CharacterControl.instance.SubToPause();
    }

    private void Update()
    {
        //Time.timeScale = 0.1f;
        switch (gameState)
        {
            case GameState.Player:
                break;
            case GameState.Enemy:
                var enemies = new List<Enemy>(FindObjectsOfType<Enemy>()); // probably should not update enemies array every frame
                enemies.RemoveAll(enemy => !enemy.isActiveAndEnabled);
                if (enemies.Count == 0)
                {
                    UIManager.instance.BeginMatch();
                    gameState = GameState.Player;
                    CharacterControl.controls.Gameplay.Enable();
                    return;
                }
                List<Enemy> stunnedEnemies = enemies.FindAll(enemy => enemy.turnsStunned > 0);
                enemies.RemoveAll(enemy => enemy.turnsStunned > 0);
                if (enemies.All(enemy => enemy.hasFinishedTurn))
                {
                    foreach (Enemy stunnedEnemy in stunnedEnemies)
                    {
                        stunnedEnemy.turnsStunned--;
                        stunnedEnemy.stunText.text = stunnedEnemy.turnsStunned.ToString();
                        if (stunnedEnemy.turnsStunned <= 0)
                        {
                            stunnedEnemy.anim.SetBool("isStunned", false);
                            stunnedEnemy.stunText.enabled = false;
                        }
                    }
                    EndTurn();
                }
                    
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
                UIManager.instance.UpdateUI();
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    if (enemy.turnsStunned == 0)
                    {
                        enemy.UpdateAIState();
                        enemy.CalculateAction();
                    }
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

    private IEnumerator AmbientSounds()
    {
        while (isActiveAndEnabled)
        {
            yield return new WaitForSeconds(Random.Range(10f, 20f));
            if (Random.Range(0f, 1f) > 0.5f)
            {
                float uh = Random.Range(0f, 1f);
                if (uh < 0.33f)
                {
                    AudioManager.instance.Play("Bird 1");
                }
                else if (uh < 0.66f)
                {
                    AudioManager.instance.Play("Bird 2");
                }
                else
                {
                    AudioManager.instance.Play("Cricket");
                }
            }
        }
    }
}

public enum GameState
{
    Player,
    Enemy
}
