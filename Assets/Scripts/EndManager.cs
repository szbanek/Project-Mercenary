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
    private PlayerMovement _player;
    private EnemyManager _enemy;
    private bool _win;

    void Start() {
        scoreManager = FindObjectOfType<ScoreManager>();
        _player = FindObjectOfType<PlayerMovement>();
        _enemy = FindObjectOfType<EnemyManager>();
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
        _player.ResetGame();
        _enemy.ResetEnemies ();
        GameManager.Instance.UpdateGameState (GameState.Game);
    }

    public void SetWin(bool state) {
        _win = state;
    }
}
