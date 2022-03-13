using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Linq;
using Random = UnityEngine.Random;

public class TileManager : MonoBehaviour
{
    public Tilemap map;
    private EnemyManager enemyManager;
    private float x_offset = 1.5f;

    [SerializeField] private Grid _grid;
    [SerializeField] private LevelData _data;
    private Dictionary<TileBase, TileData> dataFromTiles;
    private int[,,] _neighbours = {{{1, 0}, {0, 1}, {-1, 1}, {-1, 0}, {-1, -1}, {0, -1}}, {{1, 0}, {1, 1}, {0, 1}, {-1, 0}, {0, -1}, {1, -1}}};
    private List<Vector3Int> _cubeDirections = new List<Vector3Int> () {
        new Vector3Int (0,-1,1), new Vector3Int (1, -1, 0), new Vector3Int (1, 0, -1),
        new Vector3Int(0,1,-1),new Vector3Int (-1,1,0), new Vector3Int (-1,0,1)};

    private List<Vector3Int> _ocupied;
    void Start()
    {
        
    }
    void OnDestroy() {
        GameManager.OnGameStateChange -= GameManagerOnGameStateChanged;
        GameManager.NewMap -= GameManagerOnNewMap;
        map.ClearAllTiles ();
    }
    
    private void Awake()
    {
        enemyManager = FindObjectOfType<EnemyManager>();
        GameManager.OnGameStateChange += GameManagerOnGameStateChanged;
        GameManager.NewMap += GameManagerOnNewMap;
        dataFromTiles = new Dictionary<TileBase, TileData>();
        _ocupied = new List<Vector3Int> ();
        foreach (var tileData in _data.tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }
    }

    void GameManagerOnNewMap(LevelData lvl) {
        map.ClearAllTiles ();
        _data = lvl;
        GenerateMap ();
        GameManager.Instance.OnMapReady ();
    }

    void GameManagerOnGameStateChanged(GameState state) {
        _grid.enabled = state == GameState.Game;
    }

    public TileData GetTileType(Vector3Int pos) {
        TileBase tile = map.GetTile (pos);
        return dataFromTiles[tile];
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("clicked");
            Vector2 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPos = map.WorldToCell(mPos);
            TileBase tile = map.GetTile(gridPos);
            bool walkable = dataFromTiles[tile].walkable;
            Debug.Log("Clicked at: " + dataFromTiles[tile]);
        }
    }

    public void UpdateHub(Vector3Int pos) {
        var tile = _data.tiles[4];
        map.SetTransformMatrix (pos, Matrix4x4.TRS (Vector3.zero, Quaternion.Euler(0f, 0f, 0f), Vector3.one));
        map.SetTile (pos, tile);
    }

    public int GetNumOfRooms() {
        return _data.numOfRooms;
    }

    public bool IsVisible(Vector3Int gridPos) {
        TileBase tile = map.GetTile(gridPos);
        return dataFromTiles[tile].visible;
    }

    public bool IsSeeThrough(Vector3Int gridPos) {
        TileBase tile = map.GetTile(gridPos);
        return dataFromTiles[tile].seeThrough;
    }

    public bool isWalkable(Vector2 pos)
    {
        Vector3Int gridPos = map.WorldToCell(pos);
        return isWalkable (gridPos);
    }

    public bool IsOcupied(Vector3Int gridPos) {
        return _ocupied.Contains (gridPos);
    }

    public void SetOcupied(Vector3Int gridPos, bool state) {
        if (state)
        {
            _ocupied.Add (gridPos);
        }
        else
        {
            _ocupied.Remove (gridPos);
        }
    }
    

    public bool isWalkable(Vector3Int gridPos) {
        TileBase tile = map.GetTile(gridPos);
        if (tile == null)
        {
            return false;
        }
        else
        {
            return dataFromTiles[tile].walkable;
        }
    }

    public int getMovementModifier(Vector3Int gridPos)
    {
        TileBase tile = map.GetTile(gridPos);
        return dataFromTiles[tile].movementModifier;
    }
    
    public Vector3 ToCube(Vector3Int gridPos)
    {
        float q = gridPos.y;
        float r = gridPos.x - (gridPos.y - (Mathf.Abs(gridPos.y) %2))/2f;
        return new Vector3(q, r, -q-r);
    }
    Vector3Int ToOffset(Vector3 cube) {
        var row = (int) cube.x;
        var col = (int) (cube.y + (cube.x - (Mathf.Abs(cube.x) %2))/2f);
        return new Vector3Int (col, row, 0);
    }

    Vector3Int ToOffset(Vector3Int cube) {
        var row =  cube.x;
        var col = (int)(cube.y + (cube.x - (Mathf.Abs(cube.x) %2))/2f);
        return new Vector3Int (col, row, 0);
    }

    public Vector3 ToPix(Vector3Int gridPos)
    {
        float size = x_offset/3f;
        float x = size*3/2*gridPos.y;
        float y = size * Mathf.Sqrt(3) * (gridPos.x + 0.5f * (Mathf.Abs(gridPos.y) %2));
        return new Vector3(x, y, 0);
    }
    public Vector3 getPosition(Vector2 pos)
    {
        Vector3Int gridPos = map.WorldToCell(pos);
        TileBase tile = map.GetTile(gridPos);
        if (tile == null)
        {
            return new Vector3(0,0,0);
        }
        else
        {
            return ToPix(gridPos);
        }
    }

    public Vector3Int getPositionGrid(Vector3 pos)
    {
        Vector2 p = new Vector2(pos.x, pos.y);
        return map.WorldToCell(pos);
    }

    public Vector3 CalculateMiddle(Vector3 vector1, Vector3 vector2)
    {
        var x = (vector1.x + vector2.x)/2;
        var y = (vector1.y + vector2.y)/2;
        return new Vector3(x, y);
    }

    Vector3 subtractCube(Vector3 A, Vector3 B)
    {
        return new Vector3(A.x - B.x, A.y - B.y, A.z - B.z);
    }
    Vector3 CubeScale(Vector3 A, float factor) {
        return new Vector3 (A.x * factor, A.y * factor, A.z * factor);
    }

    Vector3 CubeAdd(Vector3 A, Vector3 B) {
        return new Vector3(A.x + B.x, A.y + B.y, A.z + B.z);
    }


    Vector3Int CubeDirection(int direction) {
        return _cubeDirections[direction];
    }
    float LinearInterpolation(float a, float b, float t) {
        return a+(b-a)*t;
    }

    Vector3 CubeInterpolation(Vector3 a, Vector3 b, float t) {
        return new Vector3 (LinearInterpolation (a.x, b.x, t), LinearInterpolation (a.y, b.y, t), LinearInterpolation (a.z, b.z, t));
    }

    public Vector3Int CubeRound(Vector3 a) {
        int q = Mathf.RoundToInt (a.x);
        int r = Mathf.RoundToInt (a.y);
        int s = Mathf.RoundToInt (a.z);
        float q_diff = Mathf.Abs (q - a.x);
        float r_diff = Mathf.Abs (r - a.y);
        float s_diff = Mathf.Abs (s - a.z);
        if (q_diff > r_diff && q_diff > s_diff)
        {
            q = -r - s;
        }
        else if (r_diff > s_diff)
        {
            r = -q - s;
        }
        else
        {
            s = -q - r;
        }

        return new Vector3Int (q, r, s);
    }

    public int getDistance(Vector3Int A, Vector3Int B)
    {
        Vector3 diff = subtractCube(ToCube(A), ToCube(B));
        return (int) Mathf.Max(Mathf.Abs(diff.x), Mathf.Max(Mathf.Abs(diff.y), Mathf.Abs(diff.z)));
    }
    
    public List<Vector3Int> GetNeigbours(Vector3 pos) {
        return GetNeigbours(getPositionGrid(pos));
    }
    
    public List<Vector3Int> GetNeigbours(Vector3Int pos) {
        TileBase tile = map.GetTile(pos);
        List<Vector3Int> res = new List<Vector3Int> (); 
        if (tile != null)
        {
            int parity = Math.Abs(pos.y) % 2;
            for (int i = 0; i < 6; i++)
            {
                res.Add (new Vector3Int (pos.x + _neighbours[parity, i, 0], pos.y + _neighbours[parity, i, 1], 0));
            }
        }

        return res;
    }
    public List<Vector3Int> GetAllNeighbours(Vector3Int pos) {
        List<Vector3Int> res = new List<Vector3Int> (); 
        int parity = Math.Abs(pos.y) % 2;
        for (int i = 0; i < 6; i++)
        {
            res.Add (new Vector3Int (pos.x + _neighbours[parity, i, 0], pos.y + _neighbours[parity, i, 1], 0));
        }

        return res;
    }

    public Vector3Int GetGivenNeighbour(Vector3Int pos, int num) {
        int parity = Math.Abs(pos.y) % 2;
        return new Vector3Int (pos.x + _neighbours[parity, num, 0], pos.y + _neighbours[parity, num, 1], 0);
    }

    public List<Vector3Int> CalculateRoute(Vector3Int startPoint, Vector3Int goal) {
        float distance = getDistance (startPoint, goal);
        var toSearch = new List < Tuple <Vector3Int, List <float> > >();
        toSearch.Add (new Tuple<Vector3Int, List<float>> (startPoint, new List<float> {0f, distance, distance})); // g,h,f
		var porcessed = new List <Vector3Int> ();

        var connections = new Dictionary<Vector3Int, Vector3Int> ();
        
        while (toSearch.Count > 0)
        {
            var current = toSearch[0];
            int size = toSearch.Count;
            for (int i = 0; i < size; i++)
            {
                var vertex = toSearch[0];
                if (vertex.Item2[2] < current.Item2[2] || vertex.Item2[2] == current.Item2[2] && vertex.Item2[1] < current.Item2[1])
                {
                    current = vertex;
                }

                porcessed.Add (current.Item1);
                toSearch.Remove (current);

                if (current.Item1 == goal)
                {
                    var currentPathTile = current.Item1;
                    var path = new List<Vector3Int> ();
                    while (currentPathTile != startPoint)
                    {
                        path.Add (currentPathTile);
                        currentPathTile = connections[currentPathTile];
                    }
                    return path;
                }
                
                
                var nei = GetNeigbours (current.Item1);
                foreach (var n in nei)
                {
                    if (isWalkable (n) && !porcessed.Contains (n) && !IsOcupied (n))
                    {
                        bool isIn = false;
                        foreach (var v in toSearch)
                        {
                            isIn = isIn || (v.Item1 == n);
                        }

                        var data = new Tuple<Vector3Int, List<float>> (n, new List<float>() );
                        data.Item2.Add (current.Item2[0]+1);
                        data.Item2.Add (getDistance (n, goal));
                        data.Item2.Add (data.Item2[0]+data.Item2[1]);
                        
                        float costToN = current.Item2[0] + getDistance (current.Item1, n);
                        if (!isIn || costToN < data.Item2[0])
                        {
                            data.Item2[0] = costToN;
                            if (connections.ContainsKey (n))
                            {
                                connections[n] = current.Item1;
                            }
                            else
                            {
                                connections.Add (n, current.Item1);
                            }
                            //set connection;
                            if (!isIn)
                            {
                                toSearch.Add (data);
                            }
                        }
                        
                    }
                }
            }
        }

        return null;
    }
    
    void GenerateMap() {
        List<Vector3Int> roomSeeds = new List<Vector3Int> ();
        roomSeeds.Add (new Vector3Int (0, 0, 0));
        for (int i = 1; i < _data.numOfRooms; i ++)
        {
            int lim = roomSeeds.Count;
            int rad = Random.Range (_data.distanceBetweenRooms[0], _data.distanceBetweenRooms[1]);
            var newSeed = GetRandomTileFromRing (roomSeeds[UnityEngine.Random.Range (0, roomSeeds.Count)], rad);
            roomSeeds.Add (newSeed);
        }

        foreach (var tile in roomSeeds)
        {
            int[] dir = {0,0,0,0,0,0};
            for (int i = 0; i < 6; i++)
            {
                dir[i] = UnityEngine.Random.Range (10, 100);
            }
            GenerateRoomRandomWalk (tile, Random.Range (_data.iterations[0],_data.iterations[1]), Random.Range (_data.walkLength[0],_data.walkLength[1]), _data.startRandomly);
        }

        var corridors = ConnectRooms (roomSeeds);
        foreach (var tile in corridors)
        {
            map.SetTile (tile, _data.tiles[3]);
        }

        foreach (var tile in roomSeeds)
        {
            map.SetTile (tile, _data.tiles[2]);
        }
        GenerateBoundries ();
    }

    private HashSet<Vector3Int> ConnectRooms(List<Vector3Int> roomsSeeds) {
        List<Vector3Int> rooms = new List<Vector3Int> (roomsSeeds);
        var connections = new List<Tuple<Vector3Int, Vector3Int>> ();
        for (int i = 0; i < rooms.Count; i ++)
        {
            for (int j = i+1; j < rooms.Count; j ++)
            {
                connections.Add (new Tuple<Vector3Int, Vector3Int> (rooms[i], rooms[j]));
            }
        }
        HashSet<Vector3Int> corridors = new HashSet<Vector3Int> ();
        var current = roomsSeeds[Random.Range (0, roomsSeeds.Count)];
        rooms.Remove (current);
        while (rooms.Count > 0)
        {
            Vector3Int closest = FindClosest (current, rooms);
            rooms.Remove (closest);
            var toDelete = new Tuple<Vector3Int, Vector3Int> (current, closest);
            connections.Remove (toDelete);
            connections.Remove (new Tuple<Vector3Int, Vector3Int> (toDelete.Item2, toDelete.Item1));
            HashSet<Vector3Int> newCorridor = GenerateCorridor (current, closest);
            current = closest;
            corridors.UnionWith (newCorridor);
        }

        foreach (var connection in connections)
        {
            if (Random.Range (0, 100) < _data.corridorPercentage)
            {
                HashSet<Vector3Int> newCorridor = GenerateCorridor (connection.Item1, connection.Item2);
                corridors.UnionWith (newCorridor);
            }
        }

        return corridors;
    }

    private Vector3Int FindClosest(Vector3Int center, List<Vector3Int> tileList) {
        Vector3Int closest = Vector3Int.zero;
        float dist = float.MaxValue;
        foreach (var tile in tileList)
        {
            float actDist = getDistance (center, tile);
            if (actDist < dist)
            {
                dist = actDist;
                closest = tile;
            }
        }
        
        return closest;
    }
    

    HashSet <Vector3Int> RandomWalk(Vector3Int start, int len) {
        HashSet<Vector3Int> path = new HashSet<Vector3Int>();
        var prevPos = start;
        path.Add (start);
        for (int i = 0; i < len; i ++)
        {
            var newPos = GetGivenNeighbour (prevPos, Random.Range (0, 6));
            path.Add (newPos);
            prevPos = newPos;
        }
        return path;
    }

    void GenerateRoomRandomWalk(Vector3Int start, int iteration, int length, bool startRandomly) {
        HashSet<Vector3Int> floor = new HashSet<Vector3Int>();
        var currentPos = start;
        for (int i = 0; i < iteration; i ++)
        {
            floor.UnionWith (RandomWalk(currentPos, length));
            if (startRandomly)
            {
                currentPos = floor.ElementAt (Random.Range (0, floor.Count));
            }
        }
        int tmp = floor.Count/_data.enemiesPerBase;
        int counter1 = 0, counter2 = 1;
        int enemies = Random.Range(0, tmp);
        foreach (var pos in floor)
        {
            map.SetTile (pos, _data.tiles[0]);
            if(counter1 == enemies)
            {
                enemyManager.Create(Random.Range(0, 3), pos);
                enemies = Random.Range(tmp*counter2, tmp*(counter2+1));
                counter2++;
            }
            counter1++;
        }
            
    }
    void GenerateRoom(int size, int[] dir, Vector3Int seed) {
        List<Vector3Int> tileList = new List<Vector3Int> ();
        //map.SetTile (new Vector3Int (0, 0, 0), _tiles[0]);
        tileList.Add (seed);
        int actId = 0;
        while (tileList.Count < size)
        {
            int ct = tileList.Count;
            for (int i = actId; i < ct; i ++)
            {
                for (int j = 0; j < 6; j ++)
                {
                    var pos = GetGivenNeighbour (tileList[i], j);
                    TileBase tile = map.GetTile (pos);
                    if (tile == null)
                    {
                        int a = UnityEngine.Random.Range (0, 100);
                        if (a < dir[j])
                        {
                            int proc = UnityEngine.Random.Range (0, 100);
                            int choice = 0;
                            if (proc < 10)
                            {
                                choice = 1;
                            }
                            map.SetTile (new Vector3Int (pos.x, pos.y, 0), _data.tiles[choice]);
                            tileList.Add (new Vector3Int (pos.x, pos.y, 0));
                        }
                    }
                }
            }
            actId = ct;
        }
        tileList.Clear ();
    }

    private void GenerateBoundries() {
        List<Vector3Int> boundries = new List<Vector3Int> ();
        foreach (var position in map.cellBounds.allPositionsWithin)
        {
            List<Vector3Int> nei = GetNeigbours (position);
            foreach (Vector3Int entry in nei)
            {
                TileBase tile = map.GetTile (entry);
                if (tile == null)
                {
                    boundries.Add (entry);
                }
            }
        }
        foreach (Vector3Int entry in boundries)
        {
            map.SetTile (entry, _data.tiles[1]);
        }
        boundries.Clear ();
    }
    List<Vector3Int> GenerateRing(Vector3Int center, int radius) {
        var res = new List <Vector3Int> ();
        var tile = ToOffset(CubeAdd (ToCube (center), CubeScale (CubeDirection (5), radius)));
        for (int i = 0; i < 6; i ++)
        {
            for (int j = 0; j < radius; j ++)
            {
                res.Add (tile);
                var nei = GetGivenNeighbour (tile, i);
                tile = nei;
            }
        }
        return res;
    }

    Vector3Int GetRandomTileFromRing(Vector3Int center, int radius) {
        int steps = 6 * radius;
        int choice = Random.Range (0, steps);
        var tile = ToOffset(CubeAdd (ToCube (center), CubeScale (CubeDirection (5), radius)));
        for (int i = 0; i < 6; i ++)
        {
            for (int j = 0; j < radius; j ++)
            {
                var nei = GetGivenNeighbour (tile, i);
                tile = nei;
                choice --;
                if (choice == 0)
                {
                    return tile;
                }
            }
        }

        return center;
    }

    public void ShowTilesFromList(IEnumerable<Vector3Int> list) {
        foreach (var tile in list)
        {
            map.SetTile (tile, _data.tiles[2]);
        }
    }

    public void ShowRing(Vector3Int center, int radius) {
        var ring = GenerateRing (center, radius);
        foreach (var tile in ring)
        {
            map.SetTile (tile, _data.tiles[2]);
        }
    }
    
    HashSet<Vector3Int> GenerateLine(Vector3Int start, Vector3Int end) {
        int dist = getDistance (start, end);
        var results = new HashSet<Vector3Int> ();
        for (int i = 0; i <= dist; i ++)
        {
            results.Add (ToOffset(CubeRound (CubeInterpolation (ToCube (start), ToCube(end), 1f / dist * i))));
        }
        return results;
    }
    
    public bool CanSee(Vector3Int self, Vector3Int target) {
        bool res = true;
        if (IsVisible (target))
        {
            var lineOfVision = GenerateLine (self, target);
            foreach (var tile in lineOfVision)
            {
                res = res && IsSeeThrough (tile);
            }
        }
        else
        {
            res =  false;
        }
        return res;
    }

    private HashSet<Vector3Int> GenerateCorridor(Vector3Int start, Vector3Int end) {
        return GenerateLine (start, end);
    }
}