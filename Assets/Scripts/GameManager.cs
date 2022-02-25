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

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //UpdateGameState(GameState.PlayerTurnBeg);
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
    //     switch (newState)
    //     {
    //         case GameState.PlayerTurnBeg:
    //             break;
    //         case GameState.PlayerTurnMain:
    //             break;
    //         case GameState.PlayerTurnEnd:
    //             break;
    //         case GameState.EnemyTurnBeg:
				// HandleEnemyTurnBeg();
    //             return;
    //         case GameState.EnemyTurnMain:
    //             break;
    //         case GameState.EnemyTurnEnd:
    //             break;
    //         case GameState.Lose:
    //             break;
    //         case GameState.Menu:
    //             break;
    //         case GameState.Victory:
    //             break;
    //         default:
    //             throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
    //     }
    //     State = newState;
    //     OnGameStateChange?.Invoke(newState);
        
    }
	private void HandleEnemyTurnBeg() {
        Debug.Log ("In Enemy Turn");
        Thread.Sleep (1000);
        UpdateGameState (GameState.PlayerTurnBeg);
    }
}

public enum GameState {
    PlayerTurnBeg,
    PlayerTurnMain,
    PlayerTurnEnd,
    EnemyTurnBeg,
    EnemyTurnMain,
    EnemyTurnEnd,
    Lose,
    Menu,
    Victory
}
