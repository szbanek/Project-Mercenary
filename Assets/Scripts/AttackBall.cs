using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class AttackBall : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private async Task<bool> MoveAnimation(Vector3 goal)
    {
        transform.position = Vector3.MoveTowards(transform.position, goal, 10f * Time.deltaTime);
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

    public async Task SetTarget(Vector3 target)
    {
        while(await MoveAnimation(target));
    }
}
