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
    // 縦方向のカーソルが上にあるか下にあるか（false＝上、true＝下）
    private bool verticalSelector = false;
    // 横方向のカーソルが左にあるか右にあるか（false＝左、true＝右）
    private bool horizontalSelector = false;

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
        if (_inputSetting.GetLeftKeyDown() || _inputSetting.GetRightKeyDown())
        {
            moveVertical();
            _uiController.ShowSelectedCursor(_selectedCommand);
        }
        else if (_inputSetting.GetForwardKeyDown() || _inputSetting.GetBackKeyDown())
        {
            moveHorizontal();
            _uiController.ShowSelectedCursor(_selectedCommand);
        }
        else if (_inputSetting.GetDecideInputDown())
        {
            MenuManager.Instance.OnSelectedMenu(_selectedCommand);
        }
        else if (_inputSetting.GetCancelKeyDown() || _inputSetting.GetMenuKeyDown())
        {
            CloseMenu();
        }
    }

    private void moveVertical()
    {
        verticalSelector = !verticalSelector;
        _selectedCommand = GetMenuCommand(verticalSelector, horizontalSelector);
    }

    private void moveHorizontal()
    {
        horizontalSelector = !horizontalSelector;
        _selectedCommand = GetMenuCommand(verticalSelector, horizontalSelector);
    }

    /// <summary>
    /// 引数のtrue/falseによって、適切なメニューコマンド（キャラクター、スキル、アイテム、スキルツリー）を返す
    /// </summary>
    /// <param name="ver"></param>
    /// <param name="hor"></param>
    /// <returns></returns>
    private MenuCommand GetMenuCommand(bool ver, bool hor)
    {
        int menuCommand = 0;
        if (ver)
        {
            menuCommand += 1;
        }
        if (hor)
        {
            menuCommand += 2;
        }
        return (MenuCommand)menuCommand;
    }

    /// <summary>
    /// 選択の初期化
    /// </summary>
    public void InitializeCommand()
    {
        _selectedCommand = MenuCommand.Character;
        _uiController.ShowSelectedCursor(_selectedCommand);
        verticalSelector = false;
        horizontalSelector = false;
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
