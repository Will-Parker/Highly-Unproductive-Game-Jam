using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Helpers;

public class CursorTileDisplay : MonoBehaviour
{
    private Grid grid;
    private ActionUIManager auim;
    private PartyManager pm;
    [SerializeField] private Tilemap cursorMap = null;
    [SerializeField] private Tile cursorTile = null;
    [SerializeField] private ExtendedRuleTile bombTile = null;

    [SerializeField] private Tilemap guidanceOverlayMap = null;
    [SerializeField] private Tile positiveTile = null;
    [SerializeField] private Tile negativeTile = null;

    private Vector3Int previousMousePos = new Vector3Int();
    private Vector3Int previousClosestTilePos = Vector3Int.one * int.MaxValue;

    // Start is called before the first frame update
    void Start()
    {
        grid = gameObject.GetComponent<Grid>();
        auim = FindObjectOfType<ActionUIManager>();
        pm = FindObjectOfType<PartyManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pm.moveState == MoveState.NotMoving)
        {
            if (auim.mode == UIActionMode.Heal || auim.mode == UIActionMode.Stun)
            {
                // Mouse over -> highlight tile
                Vector3Int mousePos = GetMousePosition();
                if (!mousePos.Equals(previousMousePos))
                {
                    cursorMap.SetTile(previousMousePos, null); // Remove old hoverTile
                    cursorMap.SetTile(mousePos, cursorTile);
                    previousMousePos = mousePos;
                }
            }
            else if (auim.mode == UIActionMode.Move || auim.mode == UIActionMode.Attack)
            {
                // Mouse over -> highlight tile adjacent to head
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 closestTilePos = Vector3.one * int.MaxValue;
                var dirs = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
                foreach (var dir in dirs)
                {
                    Vector3 targetNeighbor = pm.allies[0].transform.position + Vec2ToVec3(dir);
                    if (Vector3.Distance(targetNeighbor, mousePos) < Vector3.Distance(closestTilePos, mousePos))
                        closestTilePos = targetNeighbor;
                }
                if (!Vec3ToVec3Int(closestTilePos).Equals(previousClosestTilePos))
                {
                    cursorMap.SetTile(previousClosestTilePos, null); // Remove old hoverTile
                    cursorMap.SetTile(Vec3ToVec3Int(closestTilePos), cursorTile);
                    previousMousePos = grid.WorldToCell(mousePos);
                    previousClosestTilePos = Vec3ToVec3Int(closestTilePos);
                }
            }
            else if (auim.mode == UIActionMode.Bomb)
            {
                // Mouse over -> bomb tile
                Vector3Int mousePos = GetMousePosition();
                if (!mousePos.Equals(previousMousePos))
                {
                    cursorMap.SetTile(previousMousePos, null); // Remove old bombTile
                    cursorMap.SetTile(mousePos, bombTile);
                    var a = (ExtendedRuleTile) cursorMap.GetTile(mousePos);
                    int intBombRadius = Mathf.FloorToInt(((GameData.GetStatSum(pm.allies[0].type, pm.allies[3].type, pm.allies[1].type, StatType.Unique)) * 2) + 1);
                    Transform bombRad = a.m_DefaultGameObject.transform.GetChild(0);
                    bombRad.localScale = Vector3.one  * intBombRadius;
                    foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                    {
                        enemy.healthbar.DisableTemporaryDamage();
                        if (Vector3.Distance(enemy.transform.position, Vec2IntToVec3(Vec3IntToVec2Int(mousePos))) 
                            < GameData.GetStatSum(pm.allies[0].type, pm.allies[3].type, pm.allies[1].type, StatType.Unique) + 0.5)
                        {
                            enemy.healthbar.gameObject.SetActive(true);
                            enemy.healthbar.SetTemporaryDamage(GameData.GetStatSum(pm.allies[0].type, pm.allies[3].type, pm.allies[1].type, StatType.Attack));
                        } 
                        else
                        {
                            enemy.healthbar.gameObject.SetActive(false);
                            enemy.healthbar.DisableTemporaryDamage();
                        }
                    }
                    previousMousePos = mousePos;
                }
            }
            else if (cursorMap.GetTile(previousMousePos) != null || cursorMap.GetTile(previousClosestTilePos) != null)
            {
                cursorMap.SetTile(previousMousePos, null);
                cursorMap.SetTile(previousClosestTilePos, null);
                previousClosestTilePos = Vector3Int.one * int.MaxValue;
            }
        } 
        else if (cursorMap.GetTile(previousMousePos) != null || cursorMap.GetTile(previousClosestTilePos) != null)
        {
            cursorMap.SetTile(previousMousePos, null);
            cursorMap.SetTile(previousClosestTilePos, null);
            previousClosestTilePos = Vector3Int.one * int.MaxValue;
        }
    }

    public Vector3Int GetMousePosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return grid.WorldToCell(mouseWorldPos);
    }


    public void ClearOverlay()
    {
        guidanceOverlayMap.ClearAllTiles();
    }

    public void SetScreenToNegative()
    {
        Vector3Int c1 = grid.WorldToCell(Camera.main.ScreenToWorldPoint(Vector3.zero));
        Vector3Int c2 = grid.WorldToCell(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)));
        for (int x = c1.x; x <= c2.x; x++)
        {
            for (int y = c1.y; y <= c2.y; y++)
            {
                guidanceOverlayMap.SetTile(new Vector3Int(x, y), negativeTile);
            }
        }
    }

    public void SetHealOverlay()
    {
        //SetScreenToNegative();
        foreach (Ally ally in pm.allies)
        {
            guidanceOverlayMap.SetTile(Vec3ToVec3Int(ally.transform.position), positiveTile);
        }
    }

    public void SetStunOverlay()
    {
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            guidanceOverlayMap.SetTile(Vec3ToVec3Int(enemy.transform.position), positiveTile);
        }
    }

    public void SetBombOverlay()
    {
        Vector3Int c1 = grid.WorldToCell(Camera.main.ScreenToWorldPoint(Vector3.zero));
        Vector3Int c2 = grid.WorldToCell(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)));
        for (int x = c1.x; x <= c2.x; x++)
        {
            for (int y = c1.y; y <= c2.y; y++)
            {
                guidanceOverlayMap.SetTile(new Vector3Int(x, y), positiveTile);
            }
        }
    }

    public void SetMoveOverlay()
    {
        List<Vector3> emptyNeighbors = pm.allies[0].GetEmptyNeighbors();
        if (emptyNeighbors != null)
        {
            foreach (Vector3 emptyNeighbor in pm.allies[0].GetEmptyNeighbors())
            {
                guidanceOverlayMap.SetTile(Vec3ToVec3Int(emptyNeighbor), positiveTile);
            }
        }
        List<Vector3> enemyNeighbors = pm.allies[0].GetEnemyNeighbors();
        if (enemyNeighbors != null)
        {
            foreach (Vector3 enemyNeighbor in enemyNeighbors)
            {
                guidanceOverlayMap.SetTile(Vec3ToVec3Int(enemyNeighbor), negativeTile);
            }
        }
        guidanceOverlayMap.SetTile(Vec3ToVec3Int(pm.allies[1].transform.position), negativeTile);
    }

    public void SetAttackOverlay()
    {
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            guidanceOverlayMap.SetTile(Vec3ToVec3Int(enemy.transform.position), negativeTile);
        }

        List<Vector3> enemyNeighbors = pm.allies[0].GetEnemyNeighbors();
        if (enemyNeighbors != null)
        {
            foreach (Vector3 enemyNeighbor in enemyNeighbors)
            {
                guidanceOverlayMap.SetTile(Vec3ToVec3Int(enemyNeighbor), positiveTile);
            }
        }
    }

    public void SetCommandOverlay()
    {
        List<Vector3> nonAllyNeighbors = pm.allies[1].GetNonAllyNeighbors();
        if (nonAllyNeighbors != null)
        {
            foreach (Vector3 nonAllyNeighbor in nonAllyNeighbors)
            {
                guidanceOverlayMap.SetTile(Vec3ToVec3Int(nonAllyNeighbor), positiveTile);
            }
        }
        nonAllyNeighbors = pm.allies[2].GetNonAllyNeighbors();
        if (nonAllyNeighbors != null)
        {
            foreach (Vector3 nonAllyNeighbor in nonAllyNeighbors)
            {
                guidanceOverlayMap.SetTile(Vec3ToVec3Int(nonAllyNeighbor), positiveTile);
            }
        }
        nonAllyNeighbors = pm.allies[3].GetNonAllyNeighbors();
        if (nonAllyNeighbors != null)
        {
            foreach (Vector3 nonAllyNeighbor in nonAllyNeighbors)
            {
                guidanceOverlayMap.SetTile(Vec3ToVec3Int(nonAllyNeighbor), positiveTile);
            }
        }
    }

    public void SetSwapOverlay()
    {
        guidanceOverlayMap.SetTile(Vec3ToVec3Int(pm.allies[1].transform.position), positiveTile);
    }
}