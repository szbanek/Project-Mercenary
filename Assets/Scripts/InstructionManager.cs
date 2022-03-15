using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class InstructionManager : MonoBehaviour {
    [SerializeField] private Button _backToMenu;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Button _donate;
    [SerializeField] private Button _prev;
    [SerializeField] private Button _next;
    [SerializeField] private Button _close;
    [SerializeField] private GameObject _instructionPanel;
    [SerializeField] private GameObject _creditsPanel;
    [SerializeField] private GameObject _donatePanel;
    [SerializeField] private TMP_Text _smallTitle;
    [SerializeField] private TMP_Text _contentText;
    [SerializeField] private TMP_Text _creditsText;
    [SerializeField] private Image _inner;
    [SerializeField] private List<Sprite> _graphics;
    private int _numOfPages;
    private int _actPage = 0;
    private GameState _state;
    private List<string> _smallTitles;
    private List<string> _content;
    void Update()
    {
        _next.gameObject.SetActive (_state == GameState.MenuInstruction && _actPage < _numOfPages);
        _prev.gameObject.SetActive (_state == GameState.MenuInstruction && _actPage > 0);
    }

    void Awake() {
        GameManager.OnGameStateChange += GameManagerOnGameStateChanged;
        GenerateTitleList ();
        GenerateContentList ();
        _donatePanel.SetActive (false);
    }

    void GenerateTitleList() {
        _smallTitles = new List<string>();
        _smallTitles.Add ("Move");
        _smallTitles.Add ("Attack");
        _smallTitles.Add ("End of Turn");
        _smallTitles.Add ("Enemies");
        _smallTitles.Add ("Goal");
        _smallTitles.Add ("Bonuses");
        _smallTitles.Add ("Score");
        _numOfPages = _smallTitles.Count-1;
    }

    void GenerateContentList() {
        _content = new List<string>(System.IO.File.ReadAllLines ("Assets/Data/InstructionContent.txt"));
    }

    void OnDestroy() {
        GameManager.OnGameStateChange -= GameManagerOnGameStateChanged;
    }

    void GameManagerOnGameStateChanged(GameState state) {
        _state = state;
        _canvas.enabled = (state == GameState.MenuInstruction || state == GameState.MenuCredits);
        _next.gameObject.SetActive (state == GameState.MenuInstruction && _actPage < _numOfPages);
        _prev.gameObject.SetActive (state == GameState.MenuInstruction && _actPage > 0);
        _donate.gameObject.SetActive (state == GameState.MenuCredits);
        if (state == GameState.MenuInstruction)
        {
            ManageInstruction ();
        }
        else if (state == GameState.MenuCredits)
        {
            ManageCredits ();
        }
    }

    private void ManageInstruction() {
        _actPage = 0;
        _title.text = "Instruction";
        _instructionPanel.SetActive (true);
        _creditsPanel.SetActive (false);
        _smallTitle.text = _smallTitles[_actPage];
        _inner.overrideSprite = _graphics[_actPage];
        _contentText.text = _content[_actPage];
    }

    private void ManageCredits() {
        _title.text = "Credits";
        _instructionPanel.SetActive (false);
        _creditsPanel.SetActive (true);
        _creditsText.text = System.IO.File.ReadAllText ("Assets/Data/credits.txt");
    }

    public void NextClicked() {
        _actPage ++;
        _smallTitle.text = _smallTitles[_actPage];
        _inner.overrideSprite = _graphics[_actPage];
        _contentText.text = _content[_actPage];
    }
    public void PrevClicked() {
        _actPage --;
        _smallTitle.text = _smallTitles[_actPage];
        _inner.overrideSprite = _graphics[_actPage];
        _contentText.text = _content[_actPage];
    }
    public void DonateClicked() {
        //Debug.Log ("Donate");
        _donatePanel.SetActive (true);
    }

    public void BackClicked() {
        GameManager.Instance.UpdateGameState (GameState.MenuMain);
    }

    public void CloseClicked() {
        _donatePanel.SetActive (false);
    }

}
