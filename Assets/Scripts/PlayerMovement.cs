using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private TileManager tileManager;
    private RangeManager rangeManager;

    [SerializeField] private int range;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Awake()
    {
        tileManager = FindObjectOfType<TileManager>();
        rangeManager = FindObjectOfType<RangeManager>();
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            transform.position = transform.position + new Vector3(-0.73f, 0.8659766f/2);
        }

        if(Input.GetKeyDown(KeyCode.W))
        {
            transform.position = transform.position + new Vector3(0f, 0.8659766f);
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            transform.position = transform.position + new Vector3(0.73f, 0.8659766f/2);
        }

        if(Input.GetKeyDown(KeyCode.A))
        {
            transform.position = transform.position + new Vector3(-0.73f, -0.8659766f/2);
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            transform.position = transform.position + new Vector3(0f, -0.8659766f);
        }

        if(Input.GetKeyDown(KeyCode.D))
        {
            transform.position = transform.position + new Vector3(0.73f, -0.8659766f/2);
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 camPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPos = tileManager.map.WorldToCell(camPos);
            if (tileManager.isWalkable(camPos) && rangeManager.IsReachable(camPos))
            {
                transform.position = tileManager.getPosition(camPos);
                Debug.Log(rangeManager.IsReachable(camPos));
            }
        }
    }

    public int getRange()
    {
        return range;
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
