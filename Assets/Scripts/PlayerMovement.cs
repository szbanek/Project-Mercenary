using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.Rendering;

[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private int maxEnergy;
    [SerializeField] private int attackEnergy = 1;
    [SerializeField] private ProgressBar _gameProgressBar;
    private int energy;
    private int _health = 120;
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private Sprite idleSprite;
    private Animator animator;
    private TileManager tileManager;
    private RangeManager rangeManager;
    private EnemyManager enemyManager;
    private ScoreManager scoreManager;
    private EndManager endManager;
    private CameraManager cameraManager;
    private CompassManager compass;
    private Vector3Int startPosition;
    private Vector3 coorPosition;
    private SpriteRenderer _rend;
    private List<Vector3Int> route;
    private List<Vector3Int> _visitedHubs;
    private int _hubsCount;
    private bool canMove = false;
    private bool isMoving = false;
    private bool done = false;
    private bool _reaction = false;


    void Start()
    {
        energy = maxEnergy;
        _visitedHubs = new List<Vector3Int> ();
    }

    
    void Awake()
    {
        _rend = gameObject.GetComponent<SpriteRenderer>();
        tileManager = FindObjectOfType<TileManager>();
        rangeManager = FindObjectOfType<RangeManager>();
        enemyManager = FindObjectOfType<EnemyManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        endManager = FindObjectOfType<EndManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        compass = FindObjectOfType<CompassManager>();
        animator = GetComponent<Animator>();
        animator.enabled = false;
        
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

    private void GameManagerOnMapReady(int i) {
        _hubsCount = i;
        _gameProgressBar.SetMax (_hubsCount);
        // ResetGame ();
        // _reaction = true;
    }

    private void GameManagerOnPTB() {
        OnDemandRendering.renderFrameInterval = 30;
        startPosition = tileManager.getPositionGrid(transform.position);
        energy = maxEnergy+tileManager.getMovementModifier(startPosition);
        canMove = false;
        var actTile = tileManager.GetTileType (startPosition);
        if (actTile.name == "TileHub")
        {
            if (!_visitedHubs.Contains (startPosition))
            {
                _visitedHubs.Add (startPosition);
                UpdateGameProgress (startPosition);
            }
        }
        GameManager.Instance.OnPTM ();
    }

    private void GameManagerOnPTM() {
        canMove = true;
        isMoving = false;
    }

    private void GameManagerOnPTE() {
        GameManager.Instance.OnEnemy ();
        scoreManager.ChangeScore(-1);
    }

    private void UpdateGameProgress(Vector3Int pos) {
        
        _gameProgressBar.SetValue(_visitedHubs.Count);
        tileManager.UpdateHub (pos);
        compass.DeleteHub (pos);
        if (_visitedHubs.Count == _hubsCount)
        {
            endManager.SetWin (true);
            GameManager.Instance.UpdateGameState (GameState.End);
        }
    }

    public void SetHubCount(int v) {
        _hubsCount = v;
    }


    private void GameManagerOnGameStateChanged(GameState state) {
        if (state == GameState.Game)
        {
            ResetGame();
        }
        _rend.enabled = state == GameState.Game;
        _reaction = state == GameState.Game;
    }

    public void ResetGame() {
        scoreManager.SetStartingValue (100);
        _gameProgressBar.SetValue (0);
        _gameProgressBar.SetMax (_hubsCount);
        _health = 120;
        transform.position = Vector3.zero;
        canMove = false;
        isMoving = false;
        done = false;
        _visitedHubs.Clear ();
    }
    public void Pause() {
        _reaction = _reaction ^ true;
    }
    async void Update()
    {
        // Debug.Log(OnDemandRendering.renderFrameInterval);
        if (_reaction)
        {
            if (canMove && !isMoving)
            {
                if (Input.GetMouseButtonDown (0))
                {
                    done = false;
                    Vector2 targetPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
                    Vector3Int gridPos = tileManager.map.WorldToCell (targetPos);
                    EnemyMovement enemy = enemyManager.GetEnemyByGrid (gridPos);
                    if (gridPos == tileManager.getPositionGrid (transform.position))
                    {
                        OnDemandRendering.renderFrameInterval = 1;
                        canMove = false;
                        GameManager.Instance.OnPTE();
                        return;
                    }

                    if (enemy != null && !done)
                    {
                        if (tileManager.GetNeigbours (transform.position).Contains (gridPos))
                        {
                            await Attack (enemy);
                        }
                        else if (rangeManager.IsReachable (targetPos) && !done)
                        {
                            tileManager.SetOcupied (gridPos, false);
                            await Move (gridPos, true);
                            tileManager.SetOcupied (gridPos, true);
                            await Attack (enemy);
                        }
                    }

                    if (tileManager.isWalkable (targetPos) && rangeManager.IsReachable (targetPos) && !done)
                    {
                        await Move (gridPos, false);
                    }
                }
            }
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

        animator.enabled = true;
        animator.Play(0);
        OnDemandRendering.renderFrameInterval = 1;

        for(int i=0; i < route.Count; i++)
        {
            while(await MoveAnimation(tileManager.ToPix(route[i])));
            startPosition = tileManager.getPositionGrid(transform.position);
            coorPosition = transform.position;
        }

        animator.enabled = false;
        OnDemandRendering.renderFrameInterval = 30;
        _rend.sprite = idleSprite;
        GameManager.Instance.OnMove();
        cameraManager.UpdateCamera();
        isMoving = false;
        if (energy <= 0)
        {
            GameManager.Instance.OnPTE();
        }
    }
    private async Task Attack(EnemyMovement enemy)
    {
        isMoving = true;
        //Debug.Log (enemy.GetPosGrid ());
        done = true;
        if(energy >= attackEnergy)
        {
            energy -= attackEnergy;
            OnDemandRendering.renderFrameInterval = 1;
            await AttackAnimation(enemy);
            OnDemandRendering.renderFrameInterval = 30;
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
