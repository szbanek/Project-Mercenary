using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class IntrodutionManager : MonoBehaviour {
    [SerializeField] private TMP_Text _introductionText;
    [SerializeField] private Button _skip;
    [SerializeField] private TMP_Text _skipLabel;
    [SerializeField] private Canvas _self;
    [SerializeField] private LevelData _lvl;
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
            GameManager.Instance.UpdateGameState (GameState.Game);
            GameManager.Instance.OnNewMap (_lvl);
        }
        skiped = true;
    }

    private void GameManagerOnGameStateChanged(GameState state) {
        _self.enabled = state == GameState.MenuIntro;
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
