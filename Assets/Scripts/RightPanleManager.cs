using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RightPanleManager : MonoBehaviour
{
    [SerializeField] private Button _endTurnButton;
    [SerializeField] private Button _loseButton;
    [SerializeField] private PlayerMovement _player;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Sprite _energyOrb;
    [SerializeField] private Sprite _lifeOrb;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private GameObject _progressPanel;
    private float _UIwidth, _UIheight, _panelWidth;
    private bool _playerTurn = true;
    private float _energyBarX;
    private float _lifeBarX;
    private float _energyBarY;
    private float _offset = 10.0f;
    private float _imageSize = 40.0f;
    private List<GameObject> _energyList;
    private List<GameObject> _lifeList;
    



    void Start() {
       
        _energyList = new List<GameObject>();
        _lifeList = new List<GameObject> ();
        _loseButton.gameObject.SetActive (false);
        RectTransform tr = _canvas.GetComponent<RectTransform>();
        _UIwidth = tr.rect.width;
        _UIheight = tr.rect.height;
        tr = gameObject.GetComponent<RectTransform>();
        _panelWidth = tr.rect.width;
        tr = _endTurnButton.GetComponent<RectTransform>();
        var buttonCenter = tr.rect.yMax;
        float buttonWidth = tr.rect.width;
        float buttonHeight = tr.rect.height;
        _imageSize = 0.075f * _UIheight;
        _offset = 0.25f * _imageSize;
        _energyBarY = -0.5f*_UIheight + buttonCenter + buttonHeight/2 + _offset + _imageSize/2;
        _lifeBarX = _UIwidth / 2 - _panelWidth + _offset+ _imageSize/2;
        _energyBarX = _lifeBarX + _imageSize + _offset;
        SetLife ();
    }
    void Awake()
    {
        GameManager.OnGameStateChange += GameManagerOnGameStateChanged;
        GameManager.PlayerTurnMain += GameManagerOnPTM;
        GameManager.PlayerTurnEnd += GameManagerOnPTE;
        GameManager.UIOn += GameManagerOnUIOn;
        GameManager.UIOff += GameManagerOnUIOff;
        GameManager.Hurt += GameManagerOnHurt;
        GameManager.Lose += GameManagerOnLose;
    }

    void OnDestroy()
    {
        GameManager.OnGameStateChange -= GameManagerOnGameStateChanged;
        GameManager.PlayerTurnMain -= GameManagerOnPTM;
        GameManager.PlayerTurnEnd -= GameManagerOnPTE;
        GameManager.UIOn -= GameManagerOnUIOn;
        GameManager.UIOff -= GameManagerOnUIOff;
        GameManager.Hurt -= GameManagerOnHurt;
        GameManager.Lose -= GameManagerOnLose;
    }

    void Update() {
        _scoreText.text = Convert.ToString( _scoreManager.GetScore ());
        SetEnergyPoints ();
    }

    private void GameManagerOnLose() {
        _loseButton.gameObject.SetActive (true);
    }
    private void GameManagerOnHurt(int hp) {
        while (hp > 0)
        {
            Debug.Log ("Loses: " + hp.ToString ());
            GameObject topOrb = _lifeList[_lifeList.Count - 1];
            int topHp = (int) (Math.Round(topOrb.transform.localScale.x, 2) * 40);
            if (topHp > hp)
            {
                float downScale = (float) -hp / 40;
                topOrb.transform.localScale += new Vector3 (downScale, downScale, downScale);
                hp -= topHp;
            }
            else if (topHp <= hp)
            {
                Destroy(topOrb);
                _lifeList.RemoveAt (_lifeList.Count - 1);
                hp -= topHp;
            }
        }
        
    }

    private void SetLife() {
        int life = _player.GetHealth ();
        for (int i = 0; i < life/40; i ++)
        {
            GameObject newEP = new GameObject ("LifePoint_" + i);
            RectTransform trans = newEP.AddComponent<RectTransform>();
            trans.transform.SetParent(_canvas.transform); // setting parent
            trans.localScale = Vector3.one;
            trans.anchoredPosition3D = new Vector3(_lifeBarX,_energyBarY+(i*(_imageSize+_offset)),0); // setting position, will be on center
            trans.sizeDelta= new Vector2(_imageSize, _imageSize); // custom size
            Image image = newEP.AddComponent<Image>();
            image.sprite = _lifeOrb;
            newEP.transform.SetParent(_canvas.transform);
            _lifeList.Add (newEP);
        }
    }

    private void SetEnergyPoints() {
        int energy = _player.GetEnergy ();
        if (_playerTurn)
        {
            if (_energyList.Count != energy)
            {
                foreach (var obj in _energyList)
                {
                    Destroy(obj);
                }
                _energyList.Clear ();
                for (int i = 0; i < energy; i ++)
                {
                    GameObject newEP = new GameObject ("EnergyPoint_" + i);
                    RectTransform trans = newEP.AddComponent<RectTransform>();
                    trans.transform.SetParent(_canvas.transform); // setting parent
                    trans.localScale = Vector3.one;
                    trans.anchoredPosition3D = new Vector3(_energyBarX,_energyBarY+(i*(_imageSize+_offset)),0); // setting position, will be on center
                    trans.sizeDelta= new Vector2(_imageSize, _imageSize); // custom size
    
                    Image image = newEP.AddComponent<Image>();
                    image.sprite = _energyOrb;
                    
                    image.overrideSprite = Resources.Load<Sprite> ("../Sprites/LifeOrb");
                    newEP.transform.SetParent(_canvas.transform);
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
