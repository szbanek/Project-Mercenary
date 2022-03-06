using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Random = UnityEngine.Random;

// Aliens should be under the canvas

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
    [SerializeField] private int visibilityRange = 3; // don't set over 6
    [SerializeField] private GameObject attackBallPrefab;
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

    void OnDestroy() {
        Debug.Log ("destroyed");
        tileManager.SetOcupied (tileManager.getPositionGrid (transform.position), false);
    }

    private async Task Move(int distance = 1) {
        tileManager.SetOcupied (tileManager.getPositionGrid (transform.position), false);
        if (route.Count < distance)
        {
            distance = route.Count;
        }
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
        tileManager.SetOcupied (tileManager.getPositionGrid (transform.position), true);
    }

    private List <Vector3Int >  GetRandomPath(int distance) {
        List <Vector3Int > route = new List<Vector3Int> ();
        int iterationCount = 0;
        while (route.Count < distance)
        {
            var newPos = tileManager.GetGivenNeighbour (GetPosGrid (),Random.Range (0, 6));
            if (tileManager.isWalkable (newPos) && !tileManager.IsOcupied (newPos) && !route.Contains (newPos))
            {
                route.Add (newPos);
            }

            iterationCount ++;
            if (iterationCount > 6 * distance)
            {
                Debug.Log ("brak możliwości ruchu");
                break;
            }
        }

        return route;
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
                AttackAnimationRange();
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
        if(tileManager.getDistance(tileManager.getPositionGrid(transform.position), tileManager.getPositionGrid(player.transform.position)) > 12 &&
        tileManager.getDistance(tileManager.getPositionGrid(goal), tileManager.getPositionGrid(player.transform.position)) > 12)
        {
            transform.position = goal;
            return false;
        }
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

    private async void AttackAnimationRange()
    {
        Vector3 targ = player.transform.position;
        targ.z = 0f;
        targ.x = transform.position.x - targ.x;
        targ.y = transform.position.y - targ.y;
        float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;

        GameObject bullet = Instantiate(attackBallPrefab, transform.position, Quaternion.Euler(new Vector3(0, 0, angle)));
        AttackBall attackBall = bullet.GetComponent<AttackBall>();
        await attackBall.SetTarget(player.transform.position);
        Destroy(bullet);
        playerMovement.TakeDamage(attackDamage);
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
        var selfPos = tileManager.getPositionGrid (transform.position);
        var playerPos = tileManager.getPositionGrid (player.transform.position);
        var distToPlayer = tileManager.getDistance (selfPos, playerPos);
        int distToMove = distToPlayer;
        bool canAttack = false;
        if (distToPlayer <= visibilityRange)
        {
            canAttack = tileManager.CanSee (selfPos, playerPos);
            if (canAttack)
            {
                route = tileManager.CalculateRoute (selfPos, playerPos);
                            route.Reverse();
            }
            else
            {
                route = GetRandomPath(energy);
            }
        }
        else
        {
            route = GetRandomPath(energy);
        }
        if (canAttack)
        {
            distToMove= distToPlayer - attackRange;
        }
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
