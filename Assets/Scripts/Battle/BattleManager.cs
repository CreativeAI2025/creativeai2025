using UnityEngine;
using System.Collections;
/// <summary>
/// 戦闘に関する機能を管理するクラスです。
/// </summary>
public class BattleManager : MonoBehaviour
{
    /// <summary>
    /// 戦闘開始の処理を行うクラスへの参照です。
    /// </summary>
    [SerializeField]
    BattleStarter _battleStarter;
    /// <summary>
    /// 戦闘関連のウィンドウ全体を管理するクラスへの参照です。
    /// </summary>
    [SerializeField]
    BattleWindowManager _battleWindowManager;
    /// <summary>
    /// 戦闘関連のスプライトを制御するクラスへの参照です。
    /// </summary>
    [SerializeField]
    BattleSpriteController _battleSpriteController;

    /// <summary>
    /// キャラクターの移動を行うクラスを管理するクラスへの参照です。
    /// </summary>
    [SerializeField]
    CharacterMoverManager _characterMoverManager;

    /// <summary>
    /// 戦闘のフェーズです。
    /// </summary>
    public BattlePhase BattlePhase { get; private set; }

    /// <summary>
    /// 選択されたコマンドです。
    /// </summary>
    public BattleCommand SelectedCommand { get; private set; }

    /// <summary>
    /// エンカウントした敵キャラクターのIDです。
    /// </summary>
    public int EnemyId { get; private set; }

    /// <summary>
    /// 戦闘開始からのターン数です。
    /// </summary>
    public int TurnCount { get; private set; }

    /// <summary>
    /// 戦闘のフェーズを変更します。
    /// </summary>
    /// <param name="battlePhase">変更後のフェーズ</param>
    public void SetBattlePhase(BattlePhase battlePhase)
    {
        BattlePhase = battlePhase;
    }
    /// <summary>
    /// 敵キャラクターのステータスをセットします。
    /// </summary>
    /// <param name="enemyId">敵キャラクターのID</param>
    public void SetUpEnemyStatus(int enemyId)
    {
        EnemyId = enemyId;
    }

    /// <summary>
    /// 戦闘の開始処理を行います。
    /// </summary>
    public void StartBattle()
    {
        Logger.Instance.Log("戦闘を開始します。");
        // GameStateManager.ChangeToBattle();
        SetBattlePhase(BattlePhase.ShowEnemy);

        _battleWindowManager.SetUpWindowControllers(this);
        var messageWindowController = _battleWindowManager.GetMessageWindowController();
        messageWindowController.HidePager();
        // _characterMoverManager.StopCharacterMover();
        _battleStarter.StartBattle(this);
    }
    /// <summary>
    /// ウィンドウの管理を行うクラスへの参照を取得します。
    /// </summary>
    public BattleWindowManager GetWindowManager()
    {
        return _battleWindowManager;
    }
    /// <summary>
    /// 戦闘関連のスプライトを制御するクラスへの参照を取得します。
    /// </summary>
    public BattleSpriteController GetBattleSpriteController()
    {
        return _battleSpriteController;
    }
    /// <summary>
    /// コマンド入力を開始します。
    /// </summary>
    public void StartInputCommandPhase()
    {
        Logger.Instance.Log($"コマンド入力のフェーズを開始します。現在のターン数: {TurnCount}");
        var messageWindowController = _battleWindowManager.GetMessageWindowController();
        messageWindowController.HideWindow();
        BattlePhase = BattlePhase.InputCommand;
    }

    /// <summary>
    /// コマンドが選択された時のコールバックです。
    /// </summary>
    public void OnCommandSelected(BattleCommand selectedCommand)
    {
        Logger.Instance.Log($"コマンドが選択されました: {selectedCommand}");
        SelectedCommand = selectedCommand;
        HandleCommand();
    }

    /// <summary>
    /// コマンド入力に応じた処理を行います。
    /// </summary>
    void HandleCommand()
    {
        Logger.Instance.Log($"入力されたコマンドに応じた処理を行います。選択されたコマンド: {SelectedCommand}");
        switch (SelectedCommand)
        {
            case BattleCommand.Attack:
            case BattleCommand.Run:
                BattlePhase = BattlePhase.Action;
                break;
            case BattleCommand.Skill:
            case BattleCommand.Item:
                ShowSelectionWindow();
                break;
        }
    }

    /// <summary>
    /// 選択ウィンドウを表示します。
    /// </summary>
    void ShowSelectionWindow()
    {
        Logger.Instance.Log($"ShowSelectionWindow()が呼ばれました。選択されたコマンド: {SelectedCommand}");
        StartCoroutine(ShowSelectionWindowProcess());
    }

    /// <summary>
    /// 選択ウィンドウを表示する処理です。
    /// </summary>
    IEnumerator ShowSelectionWindowProcess()
    {
        yield return null;
        BattlePhase = BattlePhase.SelectItem;
        var selectionWindowController = _battleWindowManager.GetSelectionWindowController();
        selectionWindowController.SetUpWindow();
        selectionWindowController.SetPageElement();
        selectionWindowController.ShowWindow();
        selectionWindowController.SetCanSelectState(true);
    }

    /// <summary>
    /// 選択ウィンドウで項目が選択された時のコールバックです。
    /// </summary>
    public void OnItemSelected(int itemId)
    {
        switch (SelectedCommand)
        {
            case BattleCommand.Skill:
                Logger.Instance.Log($"選択された魔法のID: {itemId}");
                break;
            case BattleCommand.Item:
                Logger.Instance.Log($"選択されたアイテムのID: {itemId}");
                break;
        }
    }

    /// <summary>
    /// 選択ウィンドウでキャンセルボタンが押された時のコールバックです。
    /// </summary>
    public void OnItemCanceled()
    {
        BattlePhase = BattlePhase.InputCommand;
    }
         /// <summary>
        /// メッセージウィンドウでメッセージの表示が完了した時のコールバックです。
        /// </summary>
        public void OnFinishedShowMessage()
        {
            switch (BattlePhase)
            {
                case BattlePhase.ShowEnemy:
                    Logger.Instance.Log("敵の表示が完了しました。");
                    StartInputCommandPhase();
                    break;
            }
        }
}