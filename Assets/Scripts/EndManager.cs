using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndManager : MonoBehaviour
{
    [SerializeField] private GameObject _background;
    [SerializeField] private TMP_Text _message;
    [SerializeField] private TMP_Text _points;
    [SerializeField] private Button _back;
    [SerializeField] private Button _tryAgain;
    private ScoreManager scoreManager;
    private bool _win;

    void Start() {
        scoreManager = FindObjectOfType<ScoreManager>();
    }
    
    void Awake() {
        GameManager.OnGameStateChange += GameManagerOnGameStateChanged;
    }

    void OnDestroy() {
        GameManager.OnGameStateChange -= GameManagerOnGameStateChanged;
    }

    void GameManagerOnGameStateChanged(GameState state) {
        if (state == GameState.End)
        {
            _background.SetActive (true);
            SetData ();
        }
        else
        {
            _background.SetActive (false);
        }
        
    }

    void SetData() {
        if (_win)
        {
            _message.text = "You won!";
        }
        else
        {
            _message.text = "You lost!";
        }
        _points.text = "You got " + scoreManager.GetScore() + " points.";
    }

    public void BackClicked() {
        GameManager.Instance.UpdateGameState (GameState.MenuMain);
    }

    public void TryAgainClicked() {
        
    }

    public void SetWin(bool state) {
        _win = state;
    }
}
