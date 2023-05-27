using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

public class IntrodutionManager : MonoBehaviour {
    [SerializeField] private TMP_Text _introductionText;
    [SerializeField] private Button _skip;
    [SerializeField] private TMP_Text _skipLabel;
    [SerializeField] private Canvas _self;
    [SerializeField] private List<LevelData> _lvl;
    [SerializeField] private List<Image> _boxes;
    [SerializeField] private GameObject _choosePanel;
    [SerializeField] private Button _start;
    [SerializeField] private List<Sprite> _boxesGraphic;
    private bool _firstPlay = true;
    private int _activeButton = 1;
    private bool _skipped = false;
    void Start() {}

    void Awake() {
        GameManager.OnGameStateChange += GameManagerOnGameStateChanged;
        foreach (var box in _boxes)
        {
            box.overrideSprite = _boxesGraphic[0];
        }

        _boxes[_activeButton].overrideSprite = _boxesGraphic[1];
    }

    void OnDestroy() {
        GameManager.OnGameStateChange -= GameManagerOnGameStateChanged;
    }
    

    // Update is called once per frame
    void Update()
    {
        if (_skipped)
        {
            _skipLabel.text = "Close";
        }
        else
        {
            _skipLabel.text = "Skip";
        }
    }

    public void SkipClicked() {
        if (_skipped)
        {
            _choosePanel.SetActive (true);
        }
        _skipped = true;
    }

    public void StartClicked() {
        GameManager.Instance.UpdateGameState (GameState.Game);
        GameManager.Instance.OnNewMap (_lvl[_activeButton]);
    }

    public void BoxClicked() {
        var name = EventSystem.current.currentSelectedGameObject.name;
        int id = int.Parse(name.Substring (3))-1;
        _boxes[_activeButton].overrideSprite = _boxesGraphic[0];
        _boxes[id].overrideSprite = _boxesGraphic[1];
        _activeButton = id;
        
    }

    private void GameManagerOnGameStateChanged(GameState state) {
        _self.enabled = state == GameState.MenuIntro;
        if (_firstPlay)
        {
            _choosePanel.SetActive(false);
            if (state == GameState.MenuIntro)
            {
                StartCoroutine(TypeWriter ());
            }
        }
        else
        {
            _choosePanel.SetActive(true);
        }
        
    }

    private IEnumerator TypeWriter() {
        var txt = Resources.Load<TextAsset>("intro").text;
        float time = 0;
        int charIndex = 0;
        while (charIndex < txt.Length)
        {
            if (_skipped)
            {
                break;
            }
            time += Time.deltaTime * 75f;
            charIndex = Mathf.FloorToInt (time);
            charIndex = Mathf.Clamp (charIndex, 0, txt.Length);

            _introductionText.text = txt.Substring (0, charIndex);
            yield return null;
        }

        _introductionText.text = txt;
        _skipped = true;
        _firstPlay = false;
    }
}
