using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int score = 100;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScore(int addition)
    {
        score += addition;
        Debug.Log("Addition = " + addition);
        UpdateScore();
    }

    private void UpdateScore()
    {
        //graphics
        //Debug.Log("Score = " + score);
    }

    public int GetScore() {
        return score;
    }
}
