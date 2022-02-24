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
    [SerializeField] private int attackDamage = 30;
    private TileManager tileManager;
    private RangeManager rangeManager;
    private EnemyManager enemyManager;
    private ScoreManager scoreManager;
    private Vector3Int startPosition;
    private Vector3 coorPosition;
    private SpriteRenderer _rend;
    private List<Vector3Int> route;
    private bool canMove = false;
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
        scoreManager = FindObjectOfType<ScoreManager>();
        
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
        isMoving = false;
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
                        await Move(gridPos, true);
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
        //Debug.Log ("dist = " + dist);
        energy -= dist;
        scoreManager.ChangeScore(-dist); //distance traveled
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
            scoreManager.ChangeScore(-attackEnergy); //attacks done
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
