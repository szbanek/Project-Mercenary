using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour {
    [SerializeField] private Image _sound;
    [SerializeField] private Image _pause;
    [SerializeField] private List <Sprite> _skins;
    [SerializeField] private GameObject _pausePanel;
    private AudioSource _music;
    private PlayerMovement _playerMovement;
    private bool _pauseState = false;
    private int id = 0;
    void Start() {
       _music = FindObjectOfType<AudioSource> ();
       _playerMovement = FindObjectOfType<PlayerMovement> ();
       _pausePanel.SetActive (false);
    }

    void Awake() {
        GameManager.OnGameStateChange += GameManagerOnGameStateChanged;
        GameManager.PlayerTurnBegin += GameManagerOnPTB;
        GameManager.Enemy += GameManagerOnEnemy;
    }

    void OnDestroy() {
        GameManager.OnGameStateChange -= GameManagerOnGameStateChanged;
        GameManager.PlayerTurnBegin -= GameManagerOnPTB;
        GameManager.Enemy -= GameManagerOnEnemy;
    }

    private void GameManagerOnPTB() {
        _pause.GetComponent<Button> ().interactable = true;
    }
    private void GameManagerOnEnemy() {
        _pause.GetComponent<Button> ().interactable = false;
    }
    

    private void GameManagerOnGameStateChanged(GameState state) {
        _pause.enabled = state == GameState.Game;
        
    }
    public void SoundClicked() {
        id = id ^ 1;
        _sound.overrideSprite = _skins[id];
        if (_music.isPlaying)
        {
            _music.Pause ();
        }
        else
        {
            _music.Play ();
        }
    }

    public void PauseClicked() {
        Debug.Log ("Pause");
        _playerMovement.Pause();
        _pausePanel.SetActive (true);
        _pause.GetComponent<Button> ().interactable = false;
    }

    public void ResumeClicked() {
        Debug.Log ("unpaused");
        _playerMovement.Pause();
        _pausePanel.SetActive (false);
        _pause.GetComponent<Button> ().interactable = true;
    }

    public void BackToMenuClicked() {
        _playerMovement.Pause();
        _pausePanel.SetActive (false);
        _pause.GetComponent<Button> ().interactable = true;
        GameManager.Instance.UpdateGameState (GameState.MenuMain);
    }

}
