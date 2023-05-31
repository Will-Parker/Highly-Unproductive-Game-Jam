using UnityEngine;
using UnityEngine.Tilemaps;


public class CursorTileDisplay : MonoBehaviour
{
    private Grid grid;
    private ActionUIManager auim;
    private PartyManager pm;
    [SerializeField] private Tilemap cursorMap = null;
    [SerializeField] private Tile cursorTile = null;
    [SerializeField] private ExtendedRuleTile bombTile = null;

    private Vector3Int previousMousePos = new Vector3Int();

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
            if (auim.mode == UIActionMode.HeavyAttack || auim.mode == UIActionMode.Heal || auim.mode == UIActionMode.Stun || auim.mode == UIActionMode.Move || auim.mode == UIActionMode.Attack)
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
            else if (auim.mode == UIActionMode.Bomb)
            {
                // Mouse over -> bomb tile
                Vector3Int mousePos = GetMousePosition();
                if (!mousePos.Equals(previousMousePos))
                {
                    cursorMap.SetTile(previousMousePos, null); // Remove old bombTile
                    cursorMap.SetTile(mousePos, bombTile);
                    var a = (ExtendedRuleTile) cursorMap.GetTile(mousePos);
                    Transform bombRad = a.m_DefaultGameObject.transform.GetChild(0);
                    bombRad.localScale = Vector3.one 
                        * Mathf.FloorToInt(((pm.allies[0].BombStat 
                        + pm.allies[0].partnerBuffs[pm.allies[3].type][StatType.Bomb] 
                        + pm.allies[0].partnerBuffs[pm.allies[1].type][StatType.Bomb]) * 2) + 1);
                    previousMousePos = mousePos;
                }
            }
            else if (cursorMap.GetTile(previousMousePos) != null)
            {
                cursorMap.SetTile(previousMousePos, null);
            }
        } 
        else if (cursorMap.GetTile(previousMousePos) != null)
        {
            cursorMap.SetTile(previousMousePos, null);
        }

        //// Left mouse click -> add path tile
        //if (Input.GetMouseButton(0))
        //{
        //    pathMap.SetTile(mousePos, pathTile);
        //}

        //// Right mouse click -> remove path tile
        //if (Input.GetMouseButton(1))
        //{
        //    pathMap.SetTile(mousePos, null);
        //}
    }

    public Vector3Int GetMousePosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return grid.WorldToCell(mouseWorldPos);
    }

}