using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InstructionManager : MonoBehaviour {
    [SerializeField] private Button _back;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private Canvas _canvas;
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
        _canvas.enabled = (state == GameState.MenuInstruction || state == GameState.MenuCredits);
        if (state == GameState.MenuInstruction)
        {
            _title.text = "Instruction";
        }
        else if (state == GameState.MenuCredits)
        {
            _title.text = "Credits";
        }
    }

    public void BackClicked() {
        GameManager.Instance.UpdateGameState (GameState.MenuMain);
    }

}
