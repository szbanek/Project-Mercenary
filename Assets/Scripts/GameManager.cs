using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State;
    public static event Action<GameState> OnGameStateChange;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateGameState(GameState.PlayerTurnBeg);
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;
        switch (newState)
        {
            case GameState.PlayerTurnBeg:
                break;
            case GameState.PlayerTurnMain:
                break;
            case GameState.PlayerTurnEnd:
                break;
            case GameState.EnemyTurnBeg:
                break;
            case GameState.EnemyTurnMain:
                break;
            case GameState.EnemyTurnEnd:
                break;
            case GameState.Lose:
                break;
            case GameState.Menu:
                break;
            case GameState.Victory:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChange?.Invoke(newState);
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
