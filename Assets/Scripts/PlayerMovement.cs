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
    private bool isMoving = false;
    private bool done = false;

    
    
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
                done = false;
                Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int gridPos = tileManager.map.WorldToCell(targetPos);
                EnemyMovement enemy = enemyManager.GetEnemyByGrid(gridPos);
                if(gridPos == tileManager.getPositionGrid(transform.position))
                {
                    energy--;
                    done = true;
                    if (energy <= 0)
                    {
                        GameManager.Instance.OnPTE ();
                    }
                }
                if(enemy != null && !done)
                {
                    if(tileManager.GetNeigbours(transform.position).Contains(gridPos))
                    {
                        StartCoroutine(Attack(enemy));
                    }
                    else if(rangeManager.IsReachable(targetPos) && !done)
                    {
                        Move(gridPos, true);
                        StartCoroutine(Attack(enemy));
                    }
                }
                if (tileManager.isWalkable(targetPos) && rangeManager.IsReachable(targetPos) && !done)
                {
                    Move(gridPos, false);
                }
            }
        }

        if(isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, tileManager.ToPix(route[routeCounter]), 4f * Time.deltaTime);
            if (transform.position == tileManager.ToPix(route[routeCounter]))
            {
                routeCounter++;
            }
            if (transform.position == tileManager.ToPix(route[route.Count-1]))
            {
                isMoving=false;
                startPosition = tileManager.getPositionGrid(transform.position);
                coorPosition = transform.position;
                GameManager.Instance.OnMove();
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

    private void Move(Vector3Int gridPos, bool reduced)
    {
        route = tileManager.CalculateRoute (tileManager.getPositionGrid (transform.position), gridPos);
        route.Reverse();
        if(reduced)
        {
            route.RemoveAt(route.Count - 1);
        }
        int dist = route.Count;
        //Debug.Log ("dist = " + dist);
        energy -= dist;
        routeCounter = 0;
        isMoving = true;
    }
    private IEnumerator Attack(EnemyMovement enemy)
    {
        Debug.Log (enemy.GetPosGrid ());
        done = true;
        yield return new WaitUntil(() => isMoving == false);
        if(energy >= attackEnergy)
        {
            energy -= attackEnergy;
            AttackAnimation (enemy);
            enemy.TakeDamage(30);
            GameManager.Instance.OnMove(); //GameManager.Instance.OnAttack();
            if (energy == 0)
            {
                GameManager.Instance.OnPTE ();
            }
        }
        
    }

    async Task MoveAnimation(Vector3 goal) {
        transform.position = Vector3.MoveTowards (transform.position, goal, 4f * Time.deltaTime);
        await Task.Yield ();
    }

    private async void AttackAnimation(EnemyMovement enemy) { // może uda ci się dojść, jak to powinno działać
        var pos = transform.position;
        var goal = tileManager.ToPix (enemy.GetPosGrid ());
        await MoveAnimation (goal);
        await MoveAnimation (pos);
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
