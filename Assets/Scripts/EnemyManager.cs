using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class EnemyManager : MonoBehaviour
{

    private TileManager tileManager;
    private EnemyMovement enemyMovement;
    [SerializeField] private List<GameObject> prefabs;
    private List<GameObject> enemies = new List<GameObject> ();
    private List<Vector3Int> enemiesPlacement = new List<Vector3Int>();
    int currentEnemy = 0;

    // Start is called before the first frame update
    void Start()
    {
        tileManager = FindObjectOfType<TileManager>();
        Create(0, new Vector3Int(1, 1, 0));
        Create(1, new Vector3Int(-1, 1, 0));
    }

    void Awake()
    {

        GameManager.Enemy += GameManagerOnEnemy;
    }

    void OnDestroy()
    {
        GameManager.Enemy -= GameManagerOnEnemy;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameManagerOnEnemy()
    {
        currentEnemy = 0;
        NextEnemyMove();
    }

    public void NextEnemyMove()
    {
        if(currentEnemy > 0)
        {
            enemyMovement = enemies[currentEnemy-1].GetComponent<EnemyMovement>();
            enemiesPlacement[currentEnemy-1] = enemyMovement.GetPosGrid();
        }
        if(currentEnemy < enemies.Count)
        {
            enemyMovement = enemies[currentEnemy].GetComponent<EnemyMovement>();
            StartCoroutine(enemyMovement.Action());
        }
        else
        {
            GameManager.Instance.OnPTB();
        }
        currentEnemy++;
    }

    public EnemyMovement GetEnemyByGrid(Vector3Int vector)
    {
        for(int i = 0; i < enemies.Count; i++)
        {
            enemyMovement = enemies[i].GetComponent<EnemyMovement> ();
            if(enemyMovement.GetPosGrid() == vector)
            {
                return enemyMovement;
            }
        }
        return null;
    }

    public void Create(int type, Vector3Int vector) //type 0-Alien1, 1-Robodog
    {
        enemies.Add(Instantiate(prefabs[type], tileManager.ToPix(vector), Quaternion.identity));
        enemiesPlacement.Add(vector);
    }
    public void Destroy(Vector3Int gridPos)
    {
        for(int i = 0; i < enemies.Count; i++)
        {
            enemyMovement = enemies[i].GetComponent<EnemyMovement> ();
            if(enemyMovement.GetPosGrid() == gridPos)
            {
                Destroy(enemies[i]);
                enemies.RemoveAt(i);
            }
        }
    }

    public List<Vector3Int> GetEnemiesPlacement()
    {
        return enemiesPlacement;
    }
}
