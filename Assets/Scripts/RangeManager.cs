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
    private List<Vector3Int> _reachable;

    public PlayerMovement player;
    // Start is called before the first frame update
    void Start()
    {}

    void Awake() {
        manager = FindObjectOfType<TileManager>();
        _reachable = new List<Vector3Int>();
        GameManager.PlayerTurnMain += OnPlayerTurnMain;
        GameManager.Move += OnMove;
    }

    void OnDestroy() {
        GameManager.PlayerTurnMain -= OnPlayerTurnMain;
        GameManager.Move -= OnMove;
    }

    void OnPlayerTurnMain() {
        GetReachableList (player.getRange ());
        SetRangeMap ();
    }

    void OnMove() {
        GetReachableList (player.getRange ());
        SetRangeMap ();
    }

    // Update is called once per frame
    void Update() {
        
    }


    private void SetRangeMap() {
        Vector3 pos = manager.ToCube (player.getPosGrid ());

        for (int x = mainMap.cellBounds.min.x; x < mainMap.cellBounds.max.x; x ++)
        {
            for (int y = mainMap.cellBounds.min.y; y < mainMap.cellBounds.max.y; y ++)
            {
                Vector3Int coordinates = new Vector3Int (x, y, 0);
                if (_reachable.Contains (coordinates))
                {
                    selfMap.SetTile (coordinates, Highlight);
                }
                else
                {
                    selfMap.SetTile (coordinates, null);
                }
            }
        }
    }

    private void GenerateRangeMap() {
        _reachable.Clear ();
        int range = player.getRange ();
        Vector3 pos = manager.ToCube (player.getPosGrid ());

        for (int x = mainMap.cellBounds.min.x; x < mainMap.cellBounds.max.x; x ++)
        {
            for (int y = mainMap.cellBounds.min.y; y < mainMap.cellBounds.max.y; y ++)
            {
                Vector3Int coordinates = new Vector3Int (x, y, 0);
                TileBase tile = mainMap.GetTile (coordinates);
                if (tile != null)
                {
                    Vector3 tilePos = manager.ToCube (coordinates);
                    if (-range <= (tilePos.x - pos.x) && (tilePos.x - pos.x) <= range)
                    {
                        if (-range <= (tilePos.y - pos.y) && (tilePos.y - pos.y) <= range)
                        {
                            if (-range <= (tilePos.z - pos.z) && (tilePos.z - pos.z) <= range)
                            {
                                if (tilePos.x - pos.x + tilePos.y - pos.y + tilePos.z - pos.z == 0)
                                {
                                    selfMap.SetTile (coordinates, Highlight);
                                    _reachable.Add (coordinates);
                                }
                                else
                                {
                                    selfMap.SetTile (coordinates, null);
                                }
                            }
                            else
                            {
                                selfMap.SetTile (coordinates, null);
                            }
                        }
                        else
                        {
                            selfMap.SetTile (coordinates, null);
                        }
                    }
                    else
                    {
                        selfMap.SetTile (coordinates, null);
                    }
                }
            }
        }
    }

    public bool IsReachable(Vector2 pos)
    {
        Vector3Int gridPos = mainMap.WorldToCell(pos);
        TileBase tile = mainMap.GetTile(gridPos);
        if (tile != null && _reachable.Contains(gridPos))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void GetReachableList(int range) {
        _reachable.Clear();
        Vector3Int start = player.getPosGrid ();
        _reachable.Add (start);
        List<List<Vector3Int>> fringes = new List<List<Vector3Int>> ();
        fringes.Add (new List<Vector3Int> {start});
        for (int k = 1; k <= range; k ++)
        {
            fringes.Add (new List<Vector3Int> ());
            foreach (Vector3Int coordinate in fringes[k - 1])
            {
                List<Vector3Int> nei = manager.GetNeigbours (coordinate);
                foreach (var neighbour in nei)
                {
                    if (!_reachable.Contains (neighbour) && manager.isWalkable (neighbour))
                    {
                        _reachable.Add (neighbour);
                        fringes[k].Add (neighbour);
                    }
                }
            }
        }
    }

}
