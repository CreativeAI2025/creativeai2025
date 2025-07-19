using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class MenuSkillWindowController : MonoBehaviour, IMenuWindowController
{
    MenuManager _menuManager;
    [SerializeField] MenuSkillUIController _uiController;
    bool _canClose;
    private InputSetting _inputSetting;

    public void SetUpController(MenuManager menuManager)
    {
        _menuManager = menuManager;
    }

    /// <summary>
    /// スキルの一覧を画面に表示する
    /// </summary>
    public void SetUpSkill()
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
        if (MenuManager.Instance.MenuPhase != MenuPhase.Skill)
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
        MenuManager.Instance.OnSkillCanceled();
        HideWindow();
    }

    public void ShowWindow()
    {
        // _uiController.InitializeText(); // テキストの初期化
        SetUpSkill();
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
