using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 戦闘関連のウィンドウ全体を管理するクラスです。
/// </summary>
public class BattleWindowManager : MonoBehaviour
{
    /// <summary>
    /// ステータス表示のウィンドウを制御するクラスへの参照です。
    /// </summary>
    [SerializeField]
    StatusWindowController _statusWindowController;

    /// <summary>
    /// 敵キャラクターの名前を表示するウィンドウを制御するクラスへの参照です。
    /// </summary>
    [SerializeField]
    EnemyNameWindowController _enemyNameWindowController;
    /// <summary>
    /// コマンドウィンドウを制御するクラスへの参照です。
    /// </summary>
    [SerializeField]
    CommandWindowController _commandWindowController;

    /// <summary>
    /// 選択ウィンドウを制御するクラスへの参照です。
    /// </summary>
    [SerializeField]
    SelectionWindowController _selectItemWindowController;

    /// <summary>
    /// メッセージウィンドウの動作を制御するクラスへの参照です。
    /// </summary>
    [SerializeField]
    MessageWindowController _messageWindowController;

    /// <summary>
    /// 追記：ターゲット選択ウィンドウを制御するクラスへの参照
    /// </summary>
    [SerializeField] TargetSelectionWindowController _targetSelectionWindowController;

    /// <summary>
    /// ウィンドウのコントローラのリストです。
    /// </summary>
    List<IBattleWindowController> _battleWindowControllers = new();

    void Start()
    {
        SetControllerList();
    }

    /// <summary>
    /// UIコントローラのリストをセットアップします。
    /// </summary>
    public void SetControllerList()
    {
        _battleWindowControllers = new()
            {
                _statusWindowController,
                _enemyNameWindowController,
                _commandWindowController,
                _selectItemWindowController,
                _messageWindowController,
                _targetSelectionWindowController,
            };
    }

    /// <summary>
    /// 各ウィンドウのコントローラをセットアップします。
    /// </summary>
    /// <param name="battleManager">戦闘に関する機能を管理するクラス</param>
    public void SetUpWindowControllers(BattleManager battleManager)
    {
        foreach (var controller in _battleWindowControllers)
        {
            controller.SetUpController(battleManager);
        }
    }

    /// <summary>
    /// 各UIを非表示にします。
    /// </summary>
    public void HideAllWindow()
    {
        foreach (var controller in _battleWindowControllers)
        {
            controller.HideWindow();
        }
    }

    /// <summary>
    /// ステータス表示のウィンドウを制御するクラスへの参照を取得します。
    /// </summary>
    public StatusWindowController GetStatusWindowController()
    {
        return _statusWindowController;
    }
    /// <summary>
    /// 敵キャラクターの名前を表示するウィンドウを制御するクラスへの参照を取得します。
    /// </summary>
    public EnemyNameWindowController GetEnemyNameWindowController()
    {
        return _enemyNameWindowController;
    }
    /// <summary>
    /// コマンドウィンドウを制御するクラスへの参照を取得します。
    /// </summary>
    public CommandWindowController GetCommandWindowController()
    {
        return _commandWindowController;
    }
    /// <summary>
    /// 選択ウィンドウを制御するクラスへの参照を取得します。
    /// </summary>
    public SelectionWindowController GetSelectionWindowController()
    {
        return _selectItemWindowController;
    }
    /// <summary>
    /// ターゲット選択ウィンドウを制御するクラスへの参照を取得します。
    /// </summary>
    public TargetSelectionWindowController GetTargetSelectionWindowController()
    {
        return _targetSelectionWindowController;
    }
    /// <summary>
    /// メッセージウィンドウの動作を制御するクラスへの参照を取得します。
    /// </summary>
    public MessageWindowController GetMessageWindowController()
    {
        return _messageWindowController;
    }
}
