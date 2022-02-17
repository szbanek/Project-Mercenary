using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private TileManager tileManager;
    private RangeManager rangeManager;
    [SerializeField] private int baseRange;
    private bool canMove;
    private Vector3Int startPosition;
    private Vector3 coorPosition;
    private int actRange;
    private int _health = 120;
    private SpriteRenderer _rend;
    private bool isMoving=false;

    
    
    void Start()
    {
        actRange = baseRange;
        _rend = this.gameObject.GetComponent<SpriteRenderer> ();
    }
    void Awake()
    {
        tileManager = FindObjectOfType<TileManager>();
        rangeManager = FindObjectOfType<RangeManager>();
        
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
        actRange = baseRange+tileManager.getMovementModifier(startPosition);
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
            actRange = baseRange+tileManager.getMovementModifier(startPosition);
            GameManager.Instance.UpdateGameState(GameState.PlayerTurnMain);
        }
        else
        {
            canMove = state == GameState.PlayerTurnMain;
            if (state == GameState.PlayerTurnEnd)
            {
                //tu byłby kod na zbieranie bonusów, premie do zdrowia etc.
                // normalnie stan zmieniałby się na stan enemy, ale w ramach testów zmienia się na Player Begin
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
                Vector2 camPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int gridPos = tileManager.map.WorldToCell(camPos);
                if (tileManager.isWalkable(camPos) && rangeManager.IsReachable(camPos))
                {
                    coorPosition = camPos;
                    Debug.Log(rangeManager.IsReachable(camPos));
                    int diff = tileManager.getDistance(startPosition, gridPos);
                    actRange -= diff;
                    GameManager.Instance.OnMove ();
                    startPosition = gridPos;
                    isMoving = true;
                }
            }
        }

        if(isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, tileManager.getPosition(coorPosition), 4f * Time.deltaTime);
            GameManager.Instance.OnMove ();
            if (transform.position == tileManager.getPosition(coorPosition))
            {
                isMoving=false;
                if (actRange == 0)
                    {
                        //GameManager.Instance.UpdateGameState(GameState.PlayerTurnEnd);
                        GameManager.Instance.OnPTE ();
                    }
            }
        }

        if (Input.GetKeyDown (KeyCode.Space))
        {
            int a = Random.Range (15, 35);
            _health -= a;
            if (_health <= 0)
            {
                _rend.enabled = false;
                GameManager.Instance.OnLose ();
            }
            else
            {
                GameManager.Instance.OnHurt (a);
            }
            
        }
        
    }

    public int getRange()
    {
        return actRange;
    }

    public Vector3 getPosCoor()
    {
        return coorPosition;
    }

    public Vector3Int getPosGrid()
    {
        return startPosition;
    }

    public int GetHealth() {
        return _health;
    }
}
