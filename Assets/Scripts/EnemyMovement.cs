using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private int attackRange = 1;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private int attackEnergy = 1;
    [SerializeField] private int moveEnergy = 1;
    [SerializeField] private int maxEnergy = 1;
    [SerializeField] private int energy;
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private int currentHealth;
    private GameObject player;
    private PlayerMovement playerMovement;
    private TileManager tileManager;
    private EnemyManager enemyManager;
    private SpriteRenderer _rend;
    private List<Vector3Int> route;
    private int routeCounter;
    private bool canMove = false;
    private bool isMoving = false;


    // Start is called before the first frame update
    void Start()
    {
        this.transform.parent = GameObject.FindWithTag("Grid").transform;
        energy = maxEnergy;
        currentHealth = maxHealth;
        _rend = this.gameObject.GetComponent<SpriteRenderer> ();
        tileManager = FindObjectOfType<TileManager> ();
        enemyManager = FindObjectOfType<EnemyManager> ();
        playerMovement = FindObjectOfType<PlayerMovement> ();
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove && !isMoving)
        {
            if(tileManager.getDistance(tileManager.getPositionGrid(transform.position), tileManager.getPositionGrid(player.transform.position)) > attackRange)
            {
                route = tileManager.CalculateRoute (tileManager.getPositionGrid(transform.position), tileManager.getPositionGrid(player.transform.position));
                route.Reverse();
                routeCounter = 0;
                if(energy >= moveEnergy)
                {
                    isMoving = true;
                    energy -= moveEnergy;
                }
            }
            else
            {
                Attack();
            }
        }

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, tileManager.ToPix(route[routeCounter]), 4f * Time.deltaTime);
            if (transform.position == tileManager.ToPix(route[routeCounter]))
            {
                if(tileManager.getDistance(tileManager.getPositionGrid(transform.position), tileManager.getPositionGrid(player.transform.position)) > attackRange)
                {
                    if (energy <= 0)
                    {
                        isMoving = false;
                        canMove = false;
                    }
                    routeCounter++;
                    energy -= moveEnergy;
                }
                else
                {
                    Attack();
                }
            }
        }
    }

    private void Attack()
    {
        if(energy > 0){
            playerMovement.TakeDamage(attackDamage);
            energy -= attackEnergy;
            if (energy <= 0)
            {
                isMoving = false;
                canMove = false;
            }
        }
    }
    public void TakeDamage(int damage) {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            enemyManager.Destroy(GetPosGrid());
        }
    }

    public void Action()
    {
        energy = maxEnergy;
        canMove = true;
    }

    public Vector3Int GetPosGrid()
    {
        return tileManager.getPositionGrid(transform.position);
    }
}
