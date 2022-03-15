using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int score = 100;

    public void SetStartingValue(int v) {
        score = v;
    }

    public void ChangeScore(int addition)
    {
        score += addition;
        //Debug.Log("Addition = " + addition);
    }
    public int GetScore() {
        return score;
    }
}
