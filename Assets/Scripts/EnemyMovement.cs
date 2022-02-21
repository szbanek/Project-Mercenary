using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    private GameObject player;
    private TileManager tileManager;
    private EnemyManager enemyManager;
    [SerializeField] private int baseRange;
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private int currentHealth;
    private SpriteRenderer _rend;
    private bool canMove = false;
    private bool isMoving;
    private List<Vector3Int> route;
    private int routeCounter;


    // Start is called before the first frame update
    void Start()
    {
        this.transform.parent = GameObject.FindWithTag("Grid").transform;
        currentHealth = maxHealth;
        _rend = this.gameObject.GetComponent<SpriteRenderer> ();
        tileManager = FindObjectOfType<TileManager> ();
        enemyManager = FindObjectOfType<EnemyManager> ();
        player = GameObject.FindWithTag("Player");
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
            }
        }
    }

    public void takeDamage(int damage) {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            //Destroy(GameObject.FindWithTag("Enemy"));
        }
    }

    public void action()
    {
        canMove = true;
    }
}
