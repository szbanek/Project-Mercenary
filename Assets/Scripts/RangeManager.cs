using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RangeManager : MonoBehaviour
{
    private TileManager manager;
    public Tilemap mainMap;
    public Tilemap selfMap;
    [SerializeField] private TileBase Highlight;
    private List<Vector3Int> reachable;

    public PlayerMovement player;
    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<TileManager>();
        reachable = new List<Vector3Int>();
    }

    // Update is called once per frame
    void Update()
    {
        reachable.Clear();
        int range = player.getRange();
        Vector3 pos = manager.ToCube(player.getPosGrid());
        
        for (int x = mainMap.cellBounds.min.x; x < mainMap.cellBounds.max.x; x++)
        {
            for (int y = mainMap.cellBounds.min.y; y < mainMap.cellBounds.max.y; y++)
            {
                Vector3Int coordinates = new Vector3Int(x, y, 0);
                TileBase tile = mainMap.GetTile(coordinates);
                if (tile != null)
                {
                    Vector3 tilePos = manager.ToCube(coordinates);
                    if (-range <= (tilePos.x - pos.x) && (tilePos.x - pos.x) <= range)
                    {
                        if (-range <= (tilePos.y - pos.y) && (tilePos.y - pos.y)  <= range)
                        {
                            if (-range <= (tilePos.z - pos.z) && (tilePos.z - pos.z) <= range)
                            {
                                if (tilePos.x - pos.x + tilePos.y - pos.y + tilePos.z - pos.z == 0)
                                {
                                    selfMap.SetTile(new Vector3Int(x,y, 0), Highlight);
                                    reachable.Add(new Vector3Int(x,y, 0));
                                }
                                else
                                {
                                    selfMap.SetTile(new Vector3Int(x,y, 0), null);
                                }
                            }
                            else
                            {
                                selfMap.SetTile(new Vector3Int(x,y, 0), null);
                            }
                        }
                        else
                        {
                            selfMap.SetTile(new Vector3Int(x,y, 0), null);
                        }
                    }
                    else
                    {
                        selfMap.SetTile(new Vector3Int(x,y, 0), null);
                    }
                }
            }
        }
    }

    public bool IsReachable(Vector2 pos)
    {
        Vector3Int gridPos = mainMap.WorldToCell(pos);
        TileBase tile = mainMap.GetTile(gridPos);
        if (tile != null && reachable.Contains(gridPos))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
