using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightPanleManager : MonoBehaviour
{
    [SerializeField] private Button _endTurnButton;
    [SerializeField] private PlayerMovement _player;
    [SerializeField] private Canvas canvas;
    private bool _playerTurn = true;
    private float _energyBarX = 360.6f;
    private float _energyBarY = -190.0f;
    private List<GameObject> _energyList;
    void Start() {
        _energyList = new List<GameObject>();
    }
    void Awake()
    {
        GameManager.OnGameStateChange += GameManagerOnGameStateChanged;
        GameManager.PlayerTurnMain += GameManagerOnPTM;
        GameManager.PlayerTurnEnd += GameManagerOnPTE;
        GameManager.UIOn += GameManagerOnUIOn;
        GameManager.UIOff += GameManagerOnUIOff;
    }

    void OnDestroy()
    {
        GameManager.OnGameStateChange -= GameManagerOnGameStateChanged;
        GameManager.PlayerTurnMain += GameManagerOnPTM;
        GameManager.PlayerTurnEnd += GameManagerOnPTE;
        GameManager.UIOn -= GameManagerOnUIOn;
        GameManager.UIOff -= GameManagerOnUIOff;
    }

    void Update() {
        int energy = _player.getRange ();
        if (_playerTurn)
        {
            if (_energyList.Count != energy)
            {
                foreach (var obj in _energyList)
                {
                    Destroy(obj);
                }
                _energyList.Clear ();
                float offset = 10.0f;
                float imageSize = 40.0f;
                for (int i = 0; i < energy; i ++)
                {
                    GameObject newEP = new GameObject ("EnergyPoint_" + i);
                    RectTransform trans = newEP.AddComponent<RectTransform>();
                    trans.transform.SetParent(canvas.transform); // setting parent
                    trans.localScale = Vector3.one;
                    trans.anchoredPosition3D = new Vector3(_energyBarX,_energyBarY+(i*(imageSize+offset)),0); // setting position, will be on center
                    trans.sizeDelta= new Vector2(imageSize, imageSize); // custom size
    
                    Image image = newEP.AddComponent<Image>();
                    
                    image.overrideSprite = Resources.Load<Sprite> ("alien1");
                    newEP.transform.SetParent(canvas.transform);
                    _energyList.Add (newEP);
                }
            }
            
            
        }
    }

    private void GameManagerOnUIOn() {
        _endTurnButton.interactable = true;
        _playerTurn = true;
    }

    private void GameManagerOnUIOff() {
        _endTurnButton.interactable = false;
        _playerTurn = false;
    }

    private void GameManagerOnPTM() {}
    private void GameManagerOnPTE() {}
    

    public void GameManagerOnGameStateChanged(GameState state)
    {
        Debug.Log (state);
        _endTurnButton.interactable = (state == GameState.PlayerTurnMain);
        _playerTurn = (state == GameState.PlayerTurnMain);
    }
    
    public void EndTurn()
    {
        //GameManager.Instance.UpdateGameState(GameState.PlayerTurnEnd);
        GameManager.Instance.OnPTE ();
    }
}
