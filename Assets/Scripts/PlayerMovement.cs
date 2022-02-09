using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

public class PlayerMovement : MonoBehaviour
{
    private TileManager tileManager;
    private RangeManager rangeManager;
    [SerializeField] private int baseRange;
    private bool canMove;
    private Vector3Int startPosition;
    private int actRange;
    void Start()
    {
        actRange = baseRange;
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
        if (canMove)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 camPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int gridPos = tileManager.map.WorldToCell(camPos);
                if (tileManager.isWalkable(camPos) && rangeManager.IsReachable(camPos))
                {
                    transform.position = tileManager.getPosition(camPos);
                    Debug.Log(rangeManager.IsReachable(camPos));
                    int diff = tileManager.getDistance(startPosition, gridPos);
                    actRange -= diff;
                    if (actRange == 0)
                    {
                        //GameManager.Instance.UpdateGameState(GameState.PlayerTurnEnd);
                        GameManager.Instance.OnPTE ();
                    }

                    startPosition = gridPos;
                }
            }
        }
        
    }

    public int getRange()
    {
        return actRange;
    }

    public Vector3 getPosCoor()
    {
        return transform.position;
    }

    public Vector3Int getPosGrid()
    {
        return tileManager.getPositionGrid(transform.position);
    }
}
