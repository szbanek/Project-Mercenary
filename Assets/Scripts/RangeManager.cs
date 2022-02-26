using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RangeManager : MonoBehaviour
{
    private TileManager tileManager;
    private EnemyManager enemyManager; //GetEnemiesPlaement() daje liste z położeniem przeciwników
    public Tilemap mainMap;
    public Tilemap selfMap;
    [SerializeField] private TileBase _highlight;
    private List<Vector3Int> _reachable;

    public PlayerMovement player;
    // Start is called before the first frame update
    void Start()
    {}

    void Awake() {
        tileManager = FindObjectOfType<TileManager>();
        enemyManager = FindObjectOfType<EnemyManager>();
        _reachable = new List<Vector3Int>();
        GameManager.PlayerTurnMain += OnPlayerTurnMain;
        GameManager.Move += OnMove;
    }

    void OnDestroy() {
        GameManager.PlayerTurnMain -= OnPlayerTurnMain;
        GameManager.Move -= OnMove;
    }

    void OnPlayerTurnMain() {
        GetReachableList (player.GetEnergy ());
        SetRangeMap ();
    }

    void OnMove() {
        GetReachableList (player.GetEnergy ());
        SetRangeMap ();
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void SetRangeMap() {
        foreach (var tile in _reachable)
        {
            selfMap.SetTile (tile, _highlight);
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
        selfMap.ClearAllTiles ();
        _reachable.Clear();
        Vector3Int start = player.GetPosGrid ();
        var enemies = new List<Vector3Int> ();
        _reachable.Add (start);
        List<List<Vector3Int>> fringes = new List<List<Vector3Int>> ();
        fringes.Add (new List<Vector3Int> {start});
        for (int k = 1; k <= range; k ++)
        {
            fringes.Add (new List<Vector3Int> ());
            foreach (Vector3Int coordinate in fringes[k - 1])
            {
                List<Vector3Int> nei = tileManager.GetNeigbours (coordinate);
                foreach (var neighbour in nei)
                {
                    if (!_reachable.Contains (neighbour) && tileManager.isWalkable (neighbour))
                    {
                        if (tileManager.IsOcupied ((neighbour)))
                        {
                            if (!enemies.Contains (neighbour))
                            {
                                enemies.Add (neighbour);
                            }
                        }
                        else
                        {
                            _reachable.Add (neighbour);
                            fringes[k].Add (neighbour);
                        }
                        
                    }
                }
            }
        }

        foreach (var enemy in enemies)
        {
            tileManager.SetOcupied (enemy, false);
            var route = tileManager.CalculateRoute (start, enemy);
            tileManager.SetOcupied (enemy, true);
            if (route.Count <= range)
            {
                _reachable.Add (enemy);
            }
        }
    }

}
