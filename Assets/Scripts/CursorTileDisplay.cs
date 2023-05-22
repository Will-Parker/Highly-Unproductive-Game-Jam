using UnityEngine;
using UnityEngine.Tilemaps;


public class CursorTileDisplay : MonoBehaviour
{
    private Grid grid;
    private ActionUIManager auim;
    [SerializeField] private Tilemap cursorMap = null;
    [SerializeField] private Tile cursorTile = null;

    private Vector3Int previousMousePos = new Vector3Int();

    // Start is called before the first frame update
    void Start()
    {
        grid = gameObject.GetComponent<Grid>();
        auim = FindObjectOfType<ActionUIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (auim.mode ==  UIActionMode.HeavyAttack || auim.mode == UIActionMode.Heal || auim.mode == UIActionMode.Stun || auim.mode == UIActionMode.Bomb || auim.mode == UIActionMode.Move || auim.mode == UIActionMode.Attack)
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

    Vector3Int GetMousePosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return grid.WorldToCell(mouseWorldPos);
    }

}