using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private GameInitializer _gameInitializer;
    [SerializeField] private Image _clickStartImage;
    [SerializeField] private Image[] _startGameImages;
    private int cursor = 0;
    private int startPhase = 0; // 「０」クリックスタート、「１」ゲームスタート
    private const int NEW_GAME_CURSOR = 0;
    private const int LOAD_GAME_CURSOR = 1;
    private InputSetting _inputSetting;
    private const float BRIGHT_ALPHA = 1.0f;
    private const float DARK_ALPHA = 0.5f;
    void Start()
    {
        _inputSetting = InputSetting.Load();
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        checkKeyInput();
    }

    private void checkKeyInput()
    {
        if (startPhase == 0)
        {
            if (_inputSetting.GetDecideInputDown())
            {
                SetNextPhase();
                SetDarkAllText();
                SetBrightText(cursor);
            }
        }
        else
        {
            if (_inputSetting.GetForwardKeyDown() || _inputSetting.GetBackKeyDown())
            {
                ChangeCursor();
            }
            else if (_inputSetting.GetDecideInputDown())
            {
                if (cursor == NEW_GAME_CURSOR)
                {
                    // 新しくゲームを始める処理
                    NewGame();
                }
                else if (cursor == LOAD_GAME_CURSOR)
                {
                    // 続きのゲームから始める処理
                    LoadGame();
                }
            }
        }
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Initialize()
    {
        cursor = 0;
        startPhase = 0;
        _clickStartImage.gameObject.SetActive(true);
        foreach (var image in _startGameImages)
        {
            image.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 「New Game」か「Load Game」のどちらかを選択するフェーズに移動する
    /// </summary>
    private void SetNextPhase()
    {
        cursor = 0;
        startPhase = 1;
        _clickStartImage.gameObject.SetActive(false);
        foreach (var image in _startGameImages)
        {
            image.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 引数の添え字に応じて、その添え字のテキストの配色を明るくする
    /// </summary>
    /// <param name="cursor"></param>
    private void SetBrightText(int cursor)
    {
        Color currentColor = _startGameImages[cursor].color;
        _startGameImages[cursor].color = new Color(currentColor.r, currentColor.g, currentColor.b, BRIGHT_ALPHA);
    }

    /// <summary>
    /// 全ての選択肢を暗くする
    /// </summary>
    private void SetDarkAllText()
    {
        foreach (var image in _startGameImages)
        {
            Color color = image.color;
            image.color = new Color(color.r, color.g, color.b, DARK_ALPHA);
        }
    }
    /// <summary>
    /// カーソルを移動させる
    /// </summary>
    private void ChangeCursor()
    {
        SoundManager.Instance.PlaySE(1, 0.7f); // 効果音をつける
        if (cursor == 0)
        {
            cursor = 1;
        }
        else if (cursor == 1)
        {
            cursor = 0;
        }
        SetDarkAllText();
        SetBrightText(cursor);
    }

    /// <summary>
    /// 「はじめから」が選択された時の処理
    /// </summary>
    private void NewGame()
    {
        _gameInitializer.InitializeGame();
        SceneManager.LoadScene("zophy_House");
        Debug.Log("ゲームを最初から開始します。");
    }

    /// <summary>
    /// 「つづきから」が選択された時の処理
    /// </summary>
    private void LoadGame()
    {
        // ここに具体的な処理を加える
    }

}
