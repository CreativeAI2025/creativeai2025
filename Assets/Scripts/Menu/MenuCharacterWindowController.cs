using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class MenuCharacterWindowController : MonoBehaviour, IMenuWindowController
{
    [SerializeField] MenuCharacterUIController _uiController;
    /// <summary>
    /// ステータス画面を閉じられるかどうかのフラグ
    /// </summary>
    bool _canClose;
    private InputSetting _inputSetting;

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
        if (MenuManager.Instance == null)
        {
            return;
        }
        if (MenuManager.Instance.MenuPhase != MenuPhase.Character)
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
    private IEnumerator HideProcess()
    {
        _canClose = false;
        yield return null;
        MenuManager.Instance.OnCharacterCanceled();
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

    private IEnumerator SetCloseStateDelay()
    {
        yield return null;
        _canClose = true;
    }

    public void HideWindow()
    {
        _uiController.Hide();
    }
}
