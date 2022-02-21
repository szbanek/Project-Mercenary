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

    private async Task DoSth() {
        Debug.Log ("in enemy state");
        await Task.Delay (1000);
    }
    public async void GameManagerOnEnemy() {
        EnemiesMove();
        //await DoSth ();
        GameManager.Instance.OnPTB ();
    }

    private void EnemiesMove()
    {
        for(int i = 0; i < enemies.Count; i++)
        {
            var enemyMovement = enemies[i].GetComponent<EnemyMovement> ();
            enemyMovement.action();
        }
    }
}
