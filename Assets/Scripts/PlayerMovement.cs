using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private int maxEnergy;
    [SerializeField] private int attackEnergy = 1;
    [SerializeField] private ProgressBar _gameProgressBar;
    private int energy;
    private int _health = 120;
    [SerializeField] private int attackDamage = 30;
    private TileManager tileManager;
    private RangeManager rangeManager;
    private EnemyManager enemyManager;
    private ScoreManager scoreManager;
    private EndManager endManager;
    private Vector3Int startPosition;
    private Vector3 coorPosition;
    private SpriteRenderer _rend;
    private List<Vector3Int> route;
    private List<Vector3Int> _visitedHubs;
    private int _hubsCount;
    private bool canMove = false;
    private bool isMoving = false;
    private bool done = false;


    void Start()
    {
        energy = maxEnergy;
        _rend = this.gameObject.GetComponent<SpriteRenderer> ();
        _visitedHubs = new List<Vector3Int> ();
    }
    void Awake()
    {
        tileManager = FindObjectOfType<TileManager>();
        rangeManager = FindObjectOfType<RangeManager>();
        enemyManager = FindObjectOfType<EnemyManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        endManager = FindObjectOfType<EndManager>();
        
        GameManager.OnGameStateChange += GameManagerOnGameStateChanged;
        GameManager.PlayerTurnBegin += GameManagerOnPTB;
        GameManager.PlayerTurnMain += GameManagerOnPTM;
        GameManager.PlayerTurnEnd += GameManagerOnPTE;
        GameManager.MapReady += GameManagerOnMapReady;
    }

    void OnDestroy()
    {
        GameManager.OnGameStateChange -= GameManagerOnGameStateChanged;
        GameManager.PlayerTurnBegin -= GameManagerOnPTB;
        GameManager.PlayerTurnMain -= GameManagerOnPTM;
        GameManager.PlayerTurnEnd -= GameManagerOnPTE;
        GameManager.MapReady -= GameManagerOnMapReady;
    }

    private void GameManagerOnMapReady() {
        _hubsCount = tileManager.GetNumOfRooms ();
        _gameProgressBar.SetMax (_hubsCount);
        _gameProgressBar.SetValue (0);
    }

    private void GameManagerOnPTB() {
        startPosition = tileManager.getPositionGrid(transform.position);
        energy = maxEnergy+tileManager.getMovementModifier(startPosition);
        canMove = false;
        GameManager.Instance.OnPTM ();
        var actTile = tileManager.GetTileType (startPosition);
        if (actTile.name == "TileHub")
        {
            if (!_visitedHubs.Contains (startPosition))
            {
                _visitedHubs.Add (startPosition);
            }
        }
        UpdateGameProgress ();
    }

    private void GameManagerOnPTM() {
        canMove = true;
        isMoving = false;
    }

    private void GameManagerOnPTE() {
        GameManager.Instance.OnEnemy ();
        scoreManager.ChangeScore(-1);
    }

    private void UpdateGameProgress() {
        _gameProgressBar.SetValue(_visitedHubs.Count);
        if (_visitedHubs.Count == _hubsCount)
        {
            endManager.SetWin (true);
            GameManager.Instance.UpdateGameState (GameState.End);
        }
    }


    private void GameManagerOnGameStateChanged(GameState state) {
    }
    async void Update()
    {
        if (canMove && !isMoving)
        {
            if (Input.GetMouseButtonDown(0))
            {
                done = false;
                Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int gridPos = tileManager.map.WorldToCell(targetPos);
                EnemyMovement enemy = enemyManager.GetEnemyByGrid(gridPos);
                if(gridPos == tileManager.getPositionGrid(transform.position))
                {
                    GameManager.Instance.OnPTE();
                }
                if(enemy != null && !done)
                {
                    if(tileManager.GetNeigbours(transform.position).Contains(gridPos))
                    {
                        await Attack(enemy);
                    }
                    else if(rangeManager.IsReachable(targetPos) && !done)
                    {
                        tileManager.SetOcupied (gridPos, false);
                        await Move(gridPos, true);
                        tileManager.SetOcupied (gridPos, true);
                        await Attack(enemy);
                    }
                }
                if (tileManager.isWalkable(targetPos) && rangeManager.IsReachable(targetPos) && !done)
                {
                    await Move(gridPos, false);
                }
            }
        }
        if (Input.GetKeyDown (KeyCode.Space))
        {
            int a = Random.Range (15, 35);
            TakeDamage(a);
        }
    }

    private async Task Move(Vector3Int gridPos, bool reduced)
    {
        isMoving = true;
        route = tileManager.CalculateRoute (tileManager.getPositionGrid (transform.position), gridPos);
        route.Reverse();
        if(reduced)
        {
            route.RemoveAt(route.Count - 1);
        }
        int dist = route.Count;
        energy -= dist;
        for(int i=0; i < route.Count; i++)
        {
            while(await MoveAnimation(tileManager.ToPix(route[i])));
            startPosition = tileManager.getPositionGrid(transform.position);
            coorPosition = transform.position;
        }
        GameManager.Instance.OnMove();
        isMoving = false;
        if (energy <= 0)
        {
            GameManager.Instance.OnPTE ();
        }
    }
    private async Task Attack(EnemyMovement enemy)
    {
        isMoving = true;
        Debug.Log (enemy.GetPosGrid ());
        done = true;
        if(energy >= attackEnergy)
        {
            energy -= attackEnergy;
            await AttackAnimation(enemy);
            GameManager.Instance.OnMove(); //GameManager.Instance.OnAttack();
            isMoving = false;
            if (energy == 0)
            {
                GameManager.Instance.OnPTE();
            }
        }
    }

    private async Task<bool> MoveAnimation(Vector3 goal)
    {
        transform.position = Vector3.MoveTowards(transform.position, goal, 4f * Time.deltaTime);
        await Task.Yield();
        if(transform.position == goal)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private async Task AttackAnimation(EnemyMovement enemy)
    { 
        var pos = transform.position;
        var goal = tileManager.CalculateMiddle(tileManager.ToPix(enemy.GetPosGrid()), pos);
        while(await MoveAnimation(goal));
        enemy.TakeDamage(attackDamage);
        while(await MoveAnimation(pos));
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        scoreManager.ChangeScore(-damage); //damage taken
        if (_health <= 0)
        {
            scoreManager.ChangeScore(_health); //overkill damage
            _rend.enabled = false;
            endManager.SetWin (false);
            GameManager.Instance.UpdateGameState (GameState.End);
        }
        else
        {
            GameManager.Instance.OnHurt(damage);
        }
    }
    public int GetEnergy()
    {
        return energy;
    }

    public Vector3 GetPosCoor()
    {
        return coorPosition;
    }

    public Vector3Int GetPosGrid()
    {
        return startPosition;
    }

    public int GetHealth() {
        return _health;
    }
}
