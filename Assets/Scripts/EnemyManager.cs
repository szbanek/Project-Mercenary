using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
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
        await DoSth ();
        GameManager.Instance.OnPTB ();
    }
}
