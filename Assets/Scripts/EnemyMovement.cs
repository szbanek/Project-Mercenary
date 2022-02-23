using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

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
                Move();
            }
            else
            {
                Attack();
            }
        }

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, tileManager.ToPix(route[0]), 4f * Time.deltaTime);
            if (transform.position == tileManager.ToPix(route[0]))
            {
                isMoving = false;
                if(energy <= 0)
                {
                    canMove = false;
                }
            }
        }
    }

    private void Move()
    {
        route = tileManager.CalculateRoute (tileManager.getPositionGrid(transform.position), tileManager.getPositionGrid(player.transform.position));
        route.Reverse();
        if(energy >= moveEnergy && tileManager.getDistance(tileManager.getPositionGrid(transform.position), tileManager.getPositionGrid(player.transform.position)) > 1)
        {
            isMoving = true;
            energy -= moveEnergy;
        }
        else
        {
            canMove = false;
        }
    }
    private void Attack()
    {
        if(energy >= attackEnergy){
            if(attackRange == 1)
            {
                AttackAnimationMelee();
            }
            playerMovement.TakeDamage(attackDamage);
            energy -= attackEnergy;
            if (energy <= 0)
            {
                isMoving = false;
                canMove = false;
            }
        }
        else
        {
            Move();
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

    private async void AttackAnimationMelee()
    { 
        var pos = transform.position;
        var goal = tileManager.CalculateMiddle(tileManager.ToPix(playerMovement.GetPosGrid()), pos);
        while(await MoveAnimation(goal));
        while(await MoveAnimation(pos));
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            enemyManager.Destroy(GetPosGrid());
        }
    }

    public IEnumerator Action()
    {
        energy = maxEnergy;
        canMove = true;
        yield return new WaitUntil(() => canMove == false);
        enemyManager.NextEnemyMove();
    }

    public Vector3Int GetPosGrid()
    {
        return tileManager.getPositionGrid(transform.position);
    }
}
