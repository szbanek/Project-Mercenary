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
    private int energy;
    private int _health = 120;
    private TileManager tileManager;
    private RangeManager rangeManager;
    private EnemyManager enemyManager;
    private Vector3Int startPosition;
    private Vector3 coorPosition;
    private SpriteRenderer _rend;
    private List<Vector3Int> route;
    private int routeCounter;
    private bool canMove;
    private bool isMoving=false;
    private bool attacked = false;

    
    
    void Start()
    {
        energy = maxEnergy;
        _rend = this.gameObject.GetComponent<SpriteRenderer> ();
    }
    void Awake()
    {
        tileManager = FindObjectOfType<TileManager>();
        rangeManager = FindObjectOfType<RangeManager>();
        enemyManager = FindObjectOfType<EnemyManager>();
        
        GameManager.OnGameStateChange += GameManagerOnGameStateChanged;
        GameManager.PlayerTurnBegin += GameManagerOnPTB;
        GameManager.PlayerTurnMain += GameManagerOnPTM;
        GameManager.PlayerTurnEnd += GameManagerOnPTE;
    }

    void OnDestroy()
    {
        GameManager.OnGameStateChange -= GameManagerOnGameStateChanged;
        GameManager.PlayerTurnBegin -= GameManagerOnPTB;
        GameManager.PlayerTurnMain -= GameManagerOnPTM;
        GameManager.PlayerTurnEnd -= GameManagerOnPTE;
    }

    private void GameManagerOnPTB() {
        startPosition = tileManager.getPositionGrid(transform.position);
        energy = maxEnergy+tileManager.getMovementModifier(startPosition);
        canMove = false;
        GameManager.Instance.OnPTM ();
        
    }

    private void GameManagerOnPTM() {
        canMove = true;
    }

    private void GameManagerOnPTE() {
        GameManager.Instance.OnEnemy ();
    }


    private void GameManagerOnGameStateChanged(GameState state)
    {
        if (state == GameState.PlayerTurnBeg)
        {
            startPosition = tileManager.getPositionGrid(transform.position);
            energy = maxEnergy+tileManager.getMovementModifier(startPosition);
            GameManager.Instance.UpdateGameState(GameState.PlayerTurnMain);
        }
        else
        {
            canMove = state == GameState.PlayerTurnMain;
            if (state == GameState.PlayerTurnEnd)
            {
                //tu byłby kod na zbieranie bonusów, premie do zdrowia etc.
                GameManager.Instance.UpdateGameState(GameState.EnemyTurnBeg);
            }
        }
    }
    void Update()
    {
        if (canMove && !isMoving)
        {
            if (Input.GetMouseButtonDown(0))
            {
                attacked = false;
                Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int gridPos = tileManager.map.WorldToCell(targetPos);
                EnemyMovement enemy = enemyManager.GetEnemyByGrid(gridPos);
                if(gridPos == tileManager.getPositionGrid(transform.position))
                {
                    energy--;
                    attacked = true;
                    if (energy <= 0)
                    {
                        GameManager.Instance.OnPTE ();
                    }
                }
                if(enemy != null && !attacked)
                {
                    if(tileManager.GetNeigbours(transform.position).Contains(gridPos))
                    {
                        Attack(enemy);
                        attacked = true;
                    }
                }
                if (tileManager.isWalkable(targetPos) && rangeManager.IsReachable(targetPos) && !attacked)
                {
                    coorPosition = targetPos;
                    route = tileManager.CalculateRoute (tileManager.getPositionGrid (transform.position), gridPos);
                    route.Reverse();
                    int dist = route.Count;
                    //Debug.Log ("dist = " + dist);
                    energy -= dist;
                    GameManager.Instance.OnMove ();
                    startPosition = gridPos;
                    routeCounter = 0;
                    isMoving = true;
                }
            }
        }

        if(isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, tileManager.ToPix(route[routeCounter]), 4f * Time.deltaTime);
            GameManager.Instance.OnMove ();
            if (transform.position == tileManager.ToPix(route[routeCounter]))
            {
                routeCounter++;
            }
            if (transform.position == tileManager.getPosition(coorPosition))
            {
                isMoving=false;
                if (energy <= 0)
                {
                    GameManager.Instance.OnPTE ();
                }
            }
        }

        if (Input.GetKeyDown (KeyCode.Space))
        {
            int a = Random.Range (15, 35);
            TakeDamage(a);
        }
    }

    public void Attack(EnemyMovement enemy)
    {
        if(energy >= attackEnergy)
        {
            energy -= attackEnergy;
            enemy.TakeDamage(30);
            GameManager.Instance.OnMove();
            if (energy == 0)
            {
                GameManager.Instance.OnPTE ();
            }
            //GameManager.Instance.OnAttack();
        }
        
    }

    public void TakeDamage(int damage) {
        _health -= damage;
        if (_health <= 0)
        {
            _rend.enabled = false;
            GameManager.Instance.OnLose ();
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
