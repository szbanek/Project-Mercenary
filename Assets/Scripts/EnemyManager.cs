using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class EnemyManager : MonoBehaviour
{

    [SerializeField] private List<GameObject> prefabs;
    List<GameObject> enemies = new List<GameObject> ();

    // Start is called before the first frame update
    void Start()
    {
        enemies.Add(Instantiate(prefabs[0]));
    }

    void Awake() {

        GameManager.Enemy += GameManagerOnEnemy;
    }

    void OnDestroy() {
        GameManager.Enemy -= GameManagerOnEnemy;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameManagerOnEnemy() {
        EnemiesMove();
        GameManager.Instance.OnPTB ();
    }

    private void EnemiesMove()
    {
        for(int i = 0; i < enemies.Count; i++)
        {
            EnemyMovement enemyMovement = enemies[i].GetComponent<EnemyMovement> ();
            enemyMovement.Action();
        }
    }

    public EnemyMovement GetEnemyByGrid(Vector3Int vector)
    {
        for(int i = 0; i < enemies.Count; i++)
        {
            EnemyMovement enemyMovement = enemies[i].GetComponent<EnemyMovement> ();
            if(enemyMovement.GetPosGrid() == vector)
            {
                return enemyMovement;
            }
        }
        return null;
    }

    public void Destroy(Vector3Int gridPos)
    {
        for(int i = 0; i < enemies.Count; i++)
        {
            EnemyMovement enemyMovement = enemies[i].GetComponent<EnemyMovement> ();
            if(enemyMovement.GetPosGrid() == gridPos)
            {
                Destroy(enemies[i]);
                enemies.RemoveAt(i);
            }
        }
    }
}
