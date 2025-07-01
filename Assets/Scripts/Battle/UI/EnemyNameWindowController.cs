using UnityEngine;

public class EnemyNameWindowController : MonoBehaviour, IBattleWindowController
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
}