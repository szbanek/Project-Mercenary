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
    [SerializeField] private LevelData _lvl;
    [SerializeField] private List<Image> _boxes;
    [SerializeField] private GameObject _choosePanel;
    [SerializeField] private Button _start;
    private bool skiped = false;
    void Start() {}

    void Awake() {
        GameManager.OnGameStateChange += GameManagerOnGameStateChanged;
    }

    void OnDestroy() {
        GameManager.OnGameStateChange -= GameManagerOnGameStateChanged;
    }
    

    // Update is called once per frame
    void Update()
    {
        if (skiped)
        {
            _skipLabel.text = "Close";
        }
        else
        {
            _skipLabel.text = "Skip";
        }
    }

    public void SkipClicked() {
        if (skiped)
        {
            _choosePanel.SetActive (true);
        }
        skiped = true;
    }

    public void StartClicked() {
        GameManager.Instance.UpdateGameState (GameState.Game);
        GameManager.Instance.OnNewMap (_lvl);
    }

    public void BoxClicked() {
        var name = EventSystem.current.currentSelectedGameObject.name;
        int id = int.Parse(name.Substring (3))-1;
    }

    private void GameManagerOnGameStateChanged(GameState state) {
        _self.enabled = state == GameState.MenuIntro;
        _choosePanel.SetActive(false);
        if (state == GameState.MenuIntro)
        {
            StartCoroutine(TypeWriter ());
        }
    }

    private IEnumerator TypeWriter() {
        var txt = System.IO.File.ReadAllText ("Assets/Data/intro.txt");
        float time = 0;
        int charIndex = 0;
        while (charIndex < txt.Length)
        {
            if (skiped)
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
    }
}
