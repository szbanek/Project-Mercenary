using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {
    [SerializeField] private Canvas _menu;

    [SerializeField] private Button _start;
    [SerializeField] private Button _instruction;
    [SerializeField] private Button _credits;
    [SerializeField] private Button _quit;

    [SerializeField] private LevelData _lvl;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake() {
        GameManager.OnGameStateChange += GameManagerOnGameStateChanged;
    }

    void OnDestroy() {
        GameManager.OnGameStateChange -= GameManagerOnGameStateChanged;
    }

    void GameManagerOnGameStateChanged(GameState state) {
        _menu.enabled = state == GameState.MenuMain;
    }

    public void StartClicked() {
        GameManager.Instance.UpdateGameState (GameState.MenuIntro);
    }

    public void InstructionClicked() {
        Debug.Log ("Instr");
        GameManager.Instance.UpdateGameState (GameState.MenuInstruction);
    }

    public void CreditsClicked() {
        GameManager.Instance.UpdateGameState (GameState.MenuCredits);
    }

    public void QuitClicked() {
        //GameManager.Instance.UpdateGameState (GameState.MenuQuit);
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
