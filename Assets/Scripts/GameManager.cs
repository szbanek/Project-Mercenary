using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State;
    public static event Action<GameState> OnGameStateChange;
    public static event Action PlayerTurnBegin;
    public static event Action PlayerTurnMain;
    public static event Action PlayerTurnEnd;
    public static event Action Enemy;
    public static event Action UIOn;
    public static event Action UIOff;
    public static event Action Lose;
    public static event Action<int> Hurt;
    public static event Action Move;
    public static event Action MapReady;
    public static event Action<LevelData> NewMap;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateGameState(GameState.MenuMain);
    }

    public void OnNewMap(LevelData lvl) {
        NewMap ?.Invoke (lvl);
    }
    public void OnMapReady() {
        MapReady?.Invoke ();
    }
    public void OnMove() {
        Move?.Invoke ();
    }

    public void OnHurt(int i) {
        Hurt?.Invoke (i);
    }
    
    public void OnPTB() {
        PlayerTurnBegin?.Invoke ();
    }

    public void OnPTM() {
        UIOn?.Invoke ();
        PlayerTurnMain?.Invoke ();
    }

    public void OnPTE() {
        UIOff?.Invoke ();
        PlayerTurnEnd?.Invoke ();
    }

    public void OnEnemy() {
        Enemy?.Invoke ();
    }

    public void OnLose() {
        Lose?.Invoke ();
    }
    
    

    public void UpdateGameState(GameState newState)
    {
        switch (newState)
        {
            case GameState.MenuMain:
                break;
            case GameState.Game:
                break;
            case GameState.MenuInstruction:
                break;
            case GameState.MenuCredits:
                break;
            case GameState.MenuIntro:
                break;
            case GameState.End:
                //Debug.Log ("End");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        State = newState;
        OnGameStateChange?.Invoke(newState);
    }
	
}

public enum GameState {
    MenuMain,
    MenuIntro,
    MenuInstruction,
    MenuCredits,
    MenuQuit,
    Game,
    Pause,
    End
}
