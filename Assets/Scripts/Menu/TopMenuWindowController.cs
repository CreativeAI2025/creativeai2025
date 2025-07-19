using UnityEngine;
using System.Collections;

public class TopMenuWindowController : MonoBehaviour, IMenuWindowController
{
    /// <summary>
    /// メニューのトップ画面のUIを制御するクラスへの参照
    /// </summary>
    [SerializeField] TopMenuUIController _uiController;

    /// <summary>
    /// 現在選択中のメニュー
    /// </summary>
    MenuCommand _selectedCommand;

    InputSetting _inputSetting;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputSetting = InputSetting.Load();
    }

    // Update is called once per frame
    void Update()
    {
        SelectCommand();
    }

    void SelectCommand()
    {
        if (MenuManager.Instance == null)
        {
            return;
        }

        if (MenuManager.Instance.MenuPhase != MenuPhase.Top)
        {
            return;
        }
        if (_inputSetting.GetLeftKeyDown())
        {
            SetPreCommand();
            _uiController.ShowSelectedCursor(_selectedCommand);
        }
        else if (_inputSetting.GetRightKeyDown())
        {
            SetNextCommand();
            _uiController.ShowSelectedCursor(_selectedCommand);
        }
        else if (_inputSetting.GetDecideInputDown())
        {
            MenuManager.Instance.OnSelectedMenu(_selectedCommand);
        }
        else if (_inputSetting.GetCancelKeyDown() || _inputSetting.GetMenuKeyDown())
        {
            CloseMenu();
            Debug.Log("Close Menu");
        }
    }

    /// <summary>
    /// １つ前のコマンドを選択する
    /// </summary>
    void SetPreCommand()
    {
        int currentCommand = (int)_selectedCommand;
        int nextCommand = currentCommand - 1;
        if (nextCommand < 0)
        {
            nextCommand = 3;    // 選択が４つ（０～４）あるので、最大の３に設定
        }
        _selectedCommand = (MenuCommand)nextCommand;
    }

    /// <summary>
    /// １つ後のコマンドを選択する
    /// </summary>
    void SetNextCommand()
    {
        int currentCommand = (int)_selectedCommand;
        int nextCommand = currentCommand + 1;
        if (nextCommand > 3)
        {
            nextCommand = 0;
        }
        _selectedCommand = (MenuCommand)nextCommand;
    }

    /// <summary>
    /// 選択の初期化
    /// </summary>
    public void InitializeCommand()
    {
        _selectedCommand = MenuCommand.Character;
        _uiController.ShowSelectedCursor(_selectedCommand);
    }

    /// <summary>
    /// メニュー画面を閉じる
    /// </summary>
    public void CloseMenu()
    {
        StartCoroutine(CloseMenuProcess());
    }

    /// <summary>
    /// メニュー画面を閉じる
    /// </summary>
    /// <returns></returns>
    IEnumerator CloseMenuProcess()
    {
        yield return null;
        MenuManager.Instance.OnCloseMenu();
        HideWindow();
    }

    /// <summary>
    /// ウィンドウを表示
    /// </summary>
    public void ShowWindow()
    {
        _uiController.Show();
    }

    /// <summary>
    /// ウィンドウを非表示
    /// </summary>
    public void HideWindow()
    {
        _uiController.Hide();
    }
}
