using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class MenuCharacterWindowController : MonoBehaviour, IMenuWindowController
{
    MenuManager _menuManager;
    [SerializeField] MenuCharacterUIController _uiController;
    /// <summary>
    /// ステータス画面を閉じられるかどうかのフラグ
    /// </summary>
    bool _canClose;
    private InputSetting _inputSetting;

    public void SetUpController(MenuManager menuManager)
    {
        _menuManager = menuManager;
    }

    /// <summary>
    /// キャラクターのステータスを画面に表示する
    /// </summary>
    public void SetUpCharacter()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputSetting = InputSetting.Load();
    }

    // Update is called once per frame
    void Update()
    {
        CheckKeyInput();
    }

    void CheckKeyInput()
    {
        if (_menuManager == null)
        {
            return;
        }
        if (_menuManager.MenuPhase != MenuPhase.Character)
        {
            return;
        }
        if (!_canClose)
        {
            return;
        }
        if (_inputSetting.GetCancelKeyDown())
        {
            StartCoroutine(HideProcess());
        }
    }
    IEnumerator HideProcess()
    {
        _canClose = false;
        yield return null;
        _menuManager.OnCharacterCanceled();
        HideWindow();
    }

    public void ShowWindow()
    {
        // _uiController.InitializeText(); // テキストの初期化
        SetUpCharacter();
        _uiController.Show();
        _canClose = false;

        StartCoroutine(SetCloseStateDelay());
    }

    IEnumerator SetCloseStateDelay()
    {
        yield return null;
        _canClose = true;
    }

    public void HideWindow()
    {
        _uiController.Hide();
    }
}
