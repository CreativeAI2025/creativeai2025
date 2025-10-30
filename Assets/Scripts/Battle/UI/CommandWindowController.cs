using UnityEngine;

/// <summary>
/// コマンドウィンドウを制御するクラスです。
/// </summary>
public class CommandWindowController : MonoBehaviour, IBattleWindowController
{
    /// <summary>
    /// コマンドウィンドウのUIを制御するクラスへの参照です。
    /// </summary>
    [SerializeField]
    CommandUIController uiController;

    /// <summary>
    /// 戦闘に関する機能を管理するクラスへの参照です。
    /// </summary>
    BattleManager _battleManager;

    /// <summary>
    /// 現在選択中のコマンドです。
    /// </summary>
    BattleCommand _selectedCommand;

    private InputSetting _inputSetting;

    /// <summary>
    /// コントローラの状態をセットアップします。
    /// </summary>
    public void SetUpController(BattleManager battleManager)
    {
        _battleManager = battleManager;
    }

    void Start()
    {
        _inputSetting = InputSetting.Load();
    }

    void Update()
    {
        SelectCommand();
    }

    /// <summary>
    /// コマンドを選択します。
    /// </summary>
    void SelectCommand()
    {
        if (BattleManager.Instance == null)
        {
            return;
        }

        if (BattleManager.Instance.BattlePhase != BattlePhase.InputCommand)
        {
            return;
        }

        if (_inputSetting.GetForwardKeyDown())
        {
            SetPreCommand();
            uiController.ShowSelectedCursor(_selectedCommand);
        }
        else if (_inputSetting.GetBackKeyDown())
        {
            SetNextCommand();
            uiController.ShowSelectedCursor(_selectedCommand);
        }
        else if (_inputSetting.GetDecideInputDown())
        {
            BattleManager.Instance.OnCommandSelected(_selectedCommand);
        }
    }

    /// <summary>
    /// ひとつ前のコマンドを選択します。
    /// </summary>
    void SetPreCommand()
    {
        int currentCommand = (int)_selectedCommand;
        int nextCommand = currentCommand - 1;
        if (nextCommand < 0)
        {
            nextCommand = 3;
        }
        _selectedCommand = (BattleCommand)nextCommand;
    }

    /// <summary>
    /// ひとつ後のコマンドを選択します。
    /// </summary>
    void SetNextCommand()
    {
        int currentCommand = (int)_selectedCommand;
        int nextCommand = currentCommand + 1;
        if (nextCommand > 3)
        {
            nextCommand = 0;
        }
        _selectedCommand = (BattleCommand)nextCommand;
    }

    /// <summary>
    /// コマンド選択を初期化します。
    /// </summary>
    public void InitializeCommand()
    {
        _selectedCommand = BattleCommand.Attack;
        uiController.ShowSelectedCursor(_selectedCommand);
    }

    /// <summary>
    /// コマンドウィンドウを表示します。
    /// </summary>
    public void ShowWindow()
    {
        uiController.Show();
    }

    /// <summary>
    /// コマンドウィンドウを非表示にします。
    /// </summary>
    public void HideWindow()
    {
        uiController.Hide();
    }
}