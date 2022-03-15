using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using UnityEngine;


public class EnemyManager : MonoBehaviour
{

    private TileManager tileManager;
    private ScoreManager scoreManager;
    [SerializeField] private List<GameObject> prefabs;
    private List<GameObject> enemies = new List<GameObject> ();
    private List<Vector3Int> enemiesPlacement = new List<Vector3Int>();
    private List<Tuple<Vector3Int, int>> enemiesStartingPoints = new List<Tuple<Vector3Int, int>> ();
    
    void Start()
    {
        tileManager = FindObjectOfType<TileManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    void Awake()
    {
        GameManager.Enemy += GameManagerOnEnemy;
        GameManager.MapReady += GameManagerOnMapReady;
        GameManager.OnGameStateChange += GameManagerOnGameStateChanged;
    }

    void OnDestroy()
    {
        GameManager.Enemy -= GameManagerOnEnemy;
        GameManager.MapReady -= GameManagerOnMapReady;
        GameManager.OnGameStateChange -= GameManagerOnGameStateChanged;
    }

    void GameManagerOnGameStateChanged(GameState state) {
        if (state == GameState.End)
        {
            DestroyAll ();
        }
    }

    public void GameManagerOnMapReady() {
        GameManager.Instance.OnPTB ();
    }

    public void SaveEnemyParameters(int type, Vector3Int pos) {
        enemiesStartingPoints.Add (new Tuple<Vector3Int, int> (pos, type));
    }

    public async void GameManagerOnEnemy()
    {
        await EnemiesMove();
        GameManager.Instance.OnPTB();
    }

    public async Task EnemiesMove()
    {
        for(int i=0; i<enemies.Count; i++)
        {
            EnemyMovement enemyMovement = enemies[i].GetComponent<EnemyMovement>();
            await enemyMovement.Action();
            enemiesPlacement[i] = enemyMovement.GetPosGrid();
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

    public void ResetEnemies() {
        DestroyAll ();
        enemiesPlacement.Clear();
        foreach (var info in enemiesStartingPoints)
        {
            Create (info.Item2, info.Item1);
        }
    }

    public void Create(int type, Vector3Int vector) //type 0-Robodog, 1-Alien1, 2-Alien2
    {
        enemies.Add(Instantiate(prefabs[type], tileManager.ToPix(vector), Quaternion.identity));
        enemiesPlacement.Add(vector);
        tileManager.SetOcupied (vector, true);
    }
    public void Destroy(Vector3Int gridPos)
    {
        for(int i = 0; i < enemies.Count; i++)
        {
            EnemyMovement enemyMovement = enemies[i].GetComponent<EnemyMovement> ();
            if(enemyMovement.GetPosGrid() == gridPos)
            {
                tileManager.SetOcupied (gridPos, false);
                scoreManager.ChangeScore(enemyMovement.CalculateScore()); //kill
                Destroy(enemies[i]);
                enemies.RemoveAt(i);
                break;
            }
        }
    }

    public void DestroyAll() {
        for(int i = 0; i < enemies.Count; i++)
        {
            EnemyMovement enemyMovement = enemies[i].GetComponent<EnemyMovement> ();
            tileManager.SetOcupied (enemyMovement.GetPosGrid(), false);
            scoreManager.ChangeScore(enemyMovement.CalculateScore()); //kill
            Destroy(enemies[i]);
        }
        enemies.Clear();
    }

    public List<Vector3Int> GetEnemiesPlacement()
    {
        return enemiesPlacement;
    }
}
