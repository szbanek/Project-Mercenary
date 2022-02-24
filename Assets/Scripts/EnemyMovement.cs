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

    }

    private async Task Move(int distance = 1)
    {
        for(int i=0; i<distance; i++)
        {
             if(energy >= moveEnergy && tileManager.getDistance(tileManager.getPositionGrid(transform.position), tileManager.getPositionGrid(player.transform.position)) > 1)
            {
                while(await MoveAnimation(tileManager.ToPix(route[i])));
                energy -= moveEnergy;
            }
            else
            {
                break;
            }
        }
    }
    private async Task Attack()
    {
        if(energy >= attackEnergy){
            if(attackRange == 1)
            {
                AttackAnimationMelee();
            }
            else
            {
                playerMovement.TakeDamage(attackDamage);
            }
            energy -= attackEnergy;
            if (energy <= 0)
            {
                return;
            }
        }
        else
        {
            route = tileManager.CalculateRoute (tileManager.getPositionGrid(transform.position), tileManager.getPositionGrid(player.transform.position));
            route.Reverse();
            await Move((int)(energy/moveEnergy));
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
        playerMovement.TakeDamage(attackDamage);
        while(await MoveAnimation(pos));
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            enemyManager.Destroy(GetPosGrid());
        }
    }

    public async Task Action()
    {
        energy = maxEnergy;
        route = tileManager.CalculateRoute (tileManager.getPositionGrid(transform.position), tileManager.getPositionGrid(player.transform.position));
        route.Reverse();
        var distToMove = tileManager.getDistance(tileManager.getPositionGrid(transform.position), tileManager.getPositionGrid(player.transform.position)) - attackRange;
        if(distToMove > 0)
        {
            await Move(distToMove);
        }
        else
        {
            await Attack();
        }
    }

    public Vector3Int GetPosGrid()
    {
        return tileManager.getPositionGrid(transform.position);
    }

    public int CalculateScore()
    {
        return (int)((attackRange*attackDamage*maxEnergy*(maxHealth-currentHealth*2))/(attackEnergy*moveEnergy*25));
    }
}
