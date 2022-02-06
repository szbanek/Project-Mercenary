using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    void OnDestroy()
    {
        GameManager.OnGameStateChange -= GameManagerOnGameStateChanged;
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
                GameManager.Instance.UpdateGameState(GameState.PlayerTurnBeg);
            }
        }
    }
    void Update()
    {
        // if(Input.GetKeyDown(KeyCode.Q))
        // {
        //     transform.position = transform.position + new Vector3(-0.73f, 0.8659766f/2);
        // }
        //
        // if(Input.GetKeyDown(KeyCode.W))
        // {
        //     transform.position = transform.position + new Vector3(0f, 0.8659766f);
        // }
        //
        // if(Input.GetKeyDown(KeyCode.E))
        // {
        //     transform.position = transform.position + new Vector3(0.73f, 0.8659766f/2);
        // }
        //
        // if(Input.GetKeyDown(KeyCode.A))
        // {
        //     transform.position = transform.position + new Vector3(-0.73f, -0.8659766f/2);
        // }
        //
        // if(Input.GetKeyDown(KeyCode.S))
        // {
        //     transform.position = transform.position + new Vector3(0f, -0.8659766f);
        // }
        //
        // if(Input.GetKeyDown(KeyCode.D))
        // {
        //     transform.position = transform.position + new Vector3(0.73f, -0.8659766f/2);
        // }
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
                        GameManager.Instance.UpdateGameState(GameState.PlayerTurnEnd);
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
