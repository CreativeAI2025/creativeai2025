using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Opening : MonoBehaviour
{
    [SerializeField] GameObject tapToButtonObj;
    [SerializeField] GameObject newGameButton;
    [SerializeField] GameObject continueButton;
    [SerializeField] GameInitializer initializer;
    Image newGameButtonImage;
    Image continueButtonImage;
    int buttonCount = 0;
    private InputSetting _inputSetting;
    bool isTapToButton;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputSetting = InputSetting.Load();
        newGameButtonImage = newGameButton.gameObject.GetComponent<Image>();
        continueButtonImage = continueButton.gameObject.GetComponent<Image>();
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (isTapToButton) ButtonSelect();
    }

    /// <summary>
    /// シーン起動時の初期化
    /// </summary>
    void Reset()
    {
        isTapToButton = false;
        tapToButtonObj.SetActive(true);
        newGameButton.SetActive(false);
        continueButton.SetActive(false);
    }

    /// <summary>
    /// 初めの画面タッチ
    /// </summary>
    public void TapToStart()
    {
        SoundManager.Instance.PlaySE(3, 0.7f);
        SoundManager.Instance.PlayBGM(0, 1f);//BGMを流す
        isTapToButton = true;
        tapToButtonObj.SetActive(false);
        newGameButton.SetActive(true);
        continueButton.SetActive(true);
    }

    /// <summary>
    /// 新規ゲームのボタン処理
    /// </summary>
    public void NewGame()
    {
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlaySE(3, 0.7f);
        Debug.Log("新規ゲーム");
    }

    /// <summary>
    /// ゲームの続きから始める処理
    /// </summary>
    public void Continue()
    {
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlaySE(3, 0.7f);
        Debug.Log("続きから");
    }

    /// <summary>
    /// キーボードによるボタンの選択
    /// </summary>
    public void ButtonSelect()
    {
        if (0 <= buttonCount && buttonCount <= 1)
        {
            if (_inputSetting.GetForwardKeyDown())
            {
                Debug.Log("上" + buttonCount);
                buttonCount--;
                SoundManager.Instance.PlaySE(1, 0.7f);
            }
            else if (_inputSetting.GetBackKeyDown())
            {
                Debug.Log("下" + buttonCount);
                buttonCount++;
                SoundManager.Instance.PlaySE(1, 0.7f);
            }

            if (1 < buttonCount)
            {
                buttonCount = 1;
            }
            else if (buttonCount < 0)
            {
                buttonCount = 0;
            }
        }

        switch (buttonCount)
        {
            case 0:
                newGameButtonImage.color = new Color(255, 0, 0, 255);//赤の不透明
                continueButtonImage.color = new Color(0, 0, 0, 0);

                if (_inputSetting.GetDecideInputDown())
                {
                    NewGame();
                    GoStartGame();
                }
                break;
            case 1:
                continueButtonImage.color = new Color(255, 0, 0, 255);
                newGameButtonImage.color = new Color(0, 0, 0, 0);

                if (_inputSetting.GetDecideInputDown())
                {
                    Continue();
                    GoStartGame();
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// ゲームの初期化処理をここに入れる
    /// </summary>
    private void GoStartGame()
    {
        initializer.InitializeGame();
        SceneManager.LoadScene("zophy_House");
    }
}
