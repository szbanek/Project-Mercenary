using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;



public class EnemyManager : MonoBehaviour
{
    private TileManager tileManager;
    [SerializeField] private int baseRange;
    private SpriteRenderer _rend;
    private bool canMove = false;
    private bool isMoving;
    private GameObject player;
    private List<Vector3Int> route;
    private int routeCounter;


    // Start is called before the first frame update
    void Start()
    {
        _rend = this.gameObject.GetComponent<SpriteRenderer> ();
    }

    void Awake() {

        tileManager = FindObjectOfType<TileManager>();
        player = GameObject.Find("Player");
        GameManager.Enemy += GameManagerOnEnemy;
    }

    void OnDestroy() {
        GameManager.Enemy -= GameManagerOnEnemy;
    }
    // Update is called once per frame
    void Update()
    {
        if (canMove && !isMoving)
        {
            route = tileManager.CalculateRoute (tileManager.getPositionGrid(transform.position), tileManager.getPositionGrid(player.transform.position));
            route.Reverse();
            routeCounter = 0;
            isMoving = true;
        }

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, tileManager.ToPix(route[routeCounter]), 4f * Time.deltaTime);
            if (transform.position == tileManager.ToPix(route[routeCounter]))
            {
                routeCounter += 1;
            }
            if (routeCounter >= baseRange)
            {
                isMoving = false;
                canMove = false;
                GameManager.Instance.OnPTB ();
            }
        }
        
    }


    private async Task DoSth() {
        Debug.Log ("in enemy state");
        await Task.Delay (1000);
    }
    public async void GameManagerOnEnemy() {
        canMove = true;
        //await DoSth ();
        //GameManager.Instance.OnPTB ();
    }
}
