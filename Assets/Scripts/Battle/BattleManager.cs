using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 戦闘に関する機能を管理するクラスです。
/// </summary>
public class BattleManager : DontDestroySingleton<BattleManager>
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
    /// 状態異常の処理を行うクラスへの参照です。
    /// </summary>
    [SerializeField]
    StatusEffectManager statusEffectManager;
    /// <summary>
    /// キャラクターの移動を行うクラスを管理するクラスへの参照です。
    /// </summary>
    // [SerializeField]
    // CharacterMoverManager _characterMoverManager;

    /// <summary>
    /// 敵キャラクターのコマンドを選択するクラスへの参照です。
    /// </summary>
    [SerializeField]
    EnemyCommandSelector _enemyCommandSelector;

    /// <summary>
    /// 戦闘中のアクションを登録するクラスへの参照です。
    /// </summary>
    [SerializeField]
    BattleActionRegister _battleActionRegister;
    /// <summary>
    /// 戦闘中のアクションを処理するクラスへの参照です。
    /// </summary>
    [SerializeField]
    BattleActionProcessor _battleActionProcessor;
    /// <summary>
    /// 戦闘の結果処理を管理するクラスへの参照です。
    /// </summary>
    [SerializeField]
    BattleResultManager _battleResultManager;
    /// <summary>
    /// 戦闘のフェーズです。
    /// </summary>
    public BattlePhase BattlePhase { get; private set; }

    /// <summary>
    /// 戦闘を行う敵のIDのリスト
    /// </summary>
    /// <value></value>
    public List<int> EnemyIds { get; private set; }

    /// <summary>
    /// 選択されたコマンドです。
    /// </summary>
    public BattleCommand SelectedCommand { get; private set; }

    /// <summary>
    /// 戦闘開始からのターン数です。
    /// </summary>
    public int TurnCount { get; private set; }
    /// <summary>
    /// 戦闘が終了したかどうかのフラグです。
    /// </summary>
    public bool IsBattleFinished { get; private set; }

    public event Action OnBattleStart { add => _onBattleStart += value; remove => _onBattleStart -= value; }
    private Action _onBattleStart;
    public event Action OnBattleEnd { add => _onBattleEnd += value; remove => _onBattleEnd -= value; }
    private Action _onBattleEnd;

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
    public void SetUpEnemyStatus(List<int> ids)
    {
        EnemyIds = ids;
        EnemyStatusManager.Instance.SetUpEnemyStatus(ids);
    }

    /// <summary>
    /// 戦闘の開始処理の一部
    /// StartBattle()関数で、実際に戦闘開始処理を行う。
    /// StartBattle関数に引数がある場合は、敵のIDを入れることで、任意の敵との戦闘が始まる。
    /// 引数がない場合は、今いるシーンに適した敵がランダムに出現する。
    /// </summary>
    private void SetUpBattle()
    {
        SetPlayerStatus();  // プレイヤー周りの情報をセットする
        Logger.Instance.Log("戦闘を開始します。");
        _onBattleStart?.Invoke();
        //  GameStateManager.ChangeToBattle();
        SetBattlePhase(BattlePhase.ShowEnemy);
        TurnCount = 1;
        IsBattleFinished = false;

        _battleWindowManager.SetUpWindowControllers(this);
        var messageWindowController = _battleWindowManager.GetMessageWindowController();
        messageWindowController.HidePager();
        _battleActionProcessor.InitializeProcessor(this);
        _battleActionRegister.InitializeRegister(_battleActionProcessor);
        _enemyCommandSelector.SetReferences(this, _battleActionRegister);
        _battleResultManager.SetReferences(this);
        statusEffectManager = GetStatusEffectManager();
        statusEffectManager.SetBattleManager(this);
        // _characterMoverManager.StopCharacterMover();
        _battleStarter.StartBattle(this);
    }

    /// <summary>
    /// 引数なしでのバトル（＝ランダムエンカウント）開始処理
    /// </summary>
    public void StartBattle(List<int> enemyIds)
    {
        SetUpEnemyStatus(enemyIds);
        SetUpBattle();
    }

    public void StartBattle(int enemyId)
    {
        List<int> ids = new List<int>() { enemyId };
        SetUpEnemyStatus(ids);
        Debug.Log("敵IDのセット完了");
        SetUpBattle();
    }

    /// <summary>
    /// 味方キャラクターのステータスをセットします。
    /// </summary>
    private void SetPlayerStatus()
    {
        int _playerLevel = 1;
        // 経験値表を使って、レベルから経験値を取得します。
        var expTable = CharacterDataManager.Instance.GetExpTable();
        var expRecord = expTable.expRecords.Find(record => record.Level == _playerLevel);
        var exp = expRecord.Exp;

        // レベルに対応するパラメータデータを取得します。
        int charcterId = 1;
        var parameterTable = CharacterDataManager.Instance.GetParameterTable(charcterId);
        var parameterRecord = parameterTable.parameterRecords.Find(record => record.Level == _playerLevel);

        // 指定したレベルまでに覚えている魔法のIDをリスト化します。（要改善）
        List<int> skillList = new List<int>() { 1, 2, 3 };

        // キャラクターのステータスを設定します。
        CharacterStatus status = new()
        {
            characterId = charcterId,
            level = _playerLevel,
            exp = exp,
            currentHp = parameterRecord.HP,
            currentMp = parameterRecord.MP,

            skillList = skillList,
        };

        CharacterStatusManager.Instance.characterStatuses = new()
            {
                status
            };

        // パーティにいるキャラクターのIDをセットします。
        CharacterStatusManager.Instance.partyCharacter = new()
            {
                charcterId
            };

        // 所持アイテムをセットします。
        CharacterStatusManager.Instance.partyItemInfoList = new();
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
    /// 状態異常の処理を行うクラスへの参照を取得します。
    /// </summary>
    public StatusEffectManager GetStatusEffectManager()
    {
        return statusEffectManager;
    }

    /// <summary>
    /// コマンド入力を開始（敵が現れたあとや、ターンが終わったあとに呼ばれる）
    /// </summary>
    public void StartInputCommandPhase()
    {
        Logger.Instance.Log($"コマンド入力のフェーズを開始します。現在のターン数: {TurnCount}");
        var messageWindowController = _battleWindowManager.GetMessageWindowController();
        messageWindowController.HideWindow();
        BattlePhase = BattlePhase.InputCommand;
        _battleActionProcessor.InitializeActions();
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
                SetAttackCommandAction();
                break;
            case BattleCommand.Run:
                SetRunCommandAction();
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
                SetSkillCommandAction(itemId);
                break;
            case BattleCommand.Item:
                SetItemCommandAction(itemId);
                break;
        }
    }
    /// <summary>
    /// 攻撃コマンドを選択した際の処理です。
    /// </summary>
    void SetAttackCommandAction()
    {
        // 1対1の戦闘のため、最初のキャラクターのIDを取得します。
        int actorId = CharacterStatusManager.Instance.partyCharacter[0];
        int targetId = EnemyStatusManager.Instance.GetEnemyStatusList()[0].enemyBattleId;
        _battleActionRegister.SetFriendAttackAction(actorId, targetId);

        Logger.Instance.Log($"攻撃するキャラクターのID: {actorId} || 攻撃対象のキャラクターのID: {targetId}");

        PostCommandSelect();
    }

    /// <summary>
    /// 魔法コマンドを選択した際の処理です。
    /// </summary>
    /// <param name="itemId">魔法のID</param>
    void SetSkillCommandAction(int itemId)
    {
        int actorId = CharacterStatusManager.Instance.partyCharacter[0];
        int targetId = EnemyStatusManager.Instance.GetEnemyStatusList()[0].enemyBattleId;
        _battleActionRegister.SetFriendSkillAction(actorId, targetId, itemId);

        PostCommandSelect();
    }

    /// <summary>
    /// アイテムコマンドを選択した際の処理です。
    /// </summary>
    /// <param name="itemId">アイテムのID</param>
    void SetItemCommandAction(int itemId)
    {
        Logger.Instance.Log($"SetItemCommandAction()が呼ばれました。選択されたアイテムのID : {itemId}");
        int actorId = CharacterStatusManager.Instance.partyCharacter[0];
        var itemData = ItemDataManager.Instance.GetItemDataById(itemId);
        if (itemData == null)
        {
            Logger.Instance.LogError($"選択されたIDのアイテムは見つかりませんでした。ID : {itemId}");
            return;
        }

        int targetId = EnemyStatusManager.Instance.GetEnemyStatusList()[0].enemyBattleId;
        _battleActionRegister.SetFriendItemAction(actorId, targetId, itemId);

        PostCommandSelect();
    }

    /// <summary>
    /// 逃げるコマンドを選択した際の処理です。
    /// </summary>
    void SetRunCommandAction()
    {
        int actorId = CharacterStatusManager.Instance.partyCharacter[0];
        _battleActionRegister.SetFriendRunAction(actorId);

        PostCommandSelect();
    }
    /// <summary>
    /// 選択ウィンドウでキャンセルボタンが押された時のコールバックです。
    /// </summary>
    public void OnItemCanceled()
    {
        BattlePhase = BattlePhase.InputCommand;
        var selectionWindowController = _battleWindowManager.GetSelectionWindowController();
        selectionWindowController.HideWindow();
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
            case BattlePhase.Action:
                _battleActionProcessor.ShowNextMessage();
                break;
            case BattlePhase.Result:
                _battleResultManager.ShowNextMessage();
                break;
        }
    }
    /// <summary>
    /// ターン内の行動が完了した時のコールバックです。
    /// </summary>
    public void OnFinishedActions()
    {
        if (IsBattleFinished)
        {
            Logger.Instance.Log("OnFinishedActions() || 戦闘が終了しているため、処理を中断します。");
            return;
        }

        Logger.Instance.Log("ターン内の行動が完了しました。");
        // ここで状態異常処理をまとめて実行
        statusEffectManager.ProcessTurnEffects();
        TurnCount++;
        StartInputCommandPhase();
    }
    /// <summary>
    /// コマンド選択が完了した後の処理です。
    /// </summary>
    void PostCommandSelect()
    {
        Logger.Instance.Log("敵のコマンド入力を行います。");
        _enemyCommandSelector.SelectEnemyCommand();
    }

    /// <summary>
    /// 敵キャラクターのコマンドが選択された時のコールバックです。
    /// </summary>
    public void OnEnemyCommandSelected()
    {
        StartAction();
    }
    /// <summary>
    /// 各キャラクターの行動を開始します。
    /// </summary>
    void StartAction()
    {
        Logger.Instance.Log("選択したアクションを実行します。");
        BattlePhase = BattlePhase.Action;
        var messageWindowController = _battleWindowManager.GetMessageWindowController();
        messageWindowController.ShowWindow();
        _battleActionProcessor.SetPriorities();
        _battleActionProcessor.StartActions();
    }
    /// <summary>
    /// ステータスの値が更新された時のコールバックです。
    /// </summary>
    public void OnUpdateStatus()
    {
        _battleWindowManager.GetStatusWindowController().UpdateAllCharacterStatus();
    }

    /// <summary>
    /// 敵を全て倒した時のコールバックです。
    /// </summary>
    public void OnEnemyDefeated()
    {
        Logger.Instance.Log("敵を全て倒しました。");
        BattlePhase = BattlePhase.Result;
        IsBattleFinished = true;
        _battleResultManager.OnWin();
    }

    /// <summary>
    /// ゲームオーバーになった時のコールバックです。
    /// </summary>
    public void OnGameover()
    {
        Logger.Instance.Log("ゲームオーバーになりました。");
        BattlePhase = BattlePhase.Result;
        IsBattleFinished = true;
        _battleResultManager.OnLose();
    }
    /// <summary>
    /// 味方が逃走に成功した時のコールバックです。
    /// </summary>
    public void OnRunaway()
    {
        Logger.Instance.Log("逃走に成功しました。");
        IsBattleFinished = true;
        OnFinishBattle();
    }

    /// <summary>
    /// 敵が逃走に成功した時のコールバックです。
    /// </summary>
    public void OnEnemyRunaway()
    {
        Logger.Instance.Log("敵が逃走に成功しました。");
        BattlePhase = BattlePhase.Result;
        IsBattleFinished = true;
        _battleResultManager.OnWin();
    }

    /// <summary>
    /// 戦闘を終了する時のコールバックです。
    /// </summary>
    public void OnFinishBattle()
    {
        Logger.Instance.Log("戦闘に勝利して終了します。");
        _onBattleEnd?.Invoke();

        _battleWindowManager.HideAllWindow();
        _battleSpriteController.HideBackground();
        _battleSpriteController.HideEnemy();
        EnemyStatusManager.Instance.InitializeEnemyStatusList();
        _battleActionProcessor.InitializeActions();
        _battleActionProcessor.StopActions();

        //_characterMoverManager.ResumeCharacterMover();
        BattlePhase = BattlePhase.NotInBattle;
    }

    /// <summary>
    /// 戦闘を終了する時のコールバックです。
    /// </summary>
    public void OnFinishBattleWithGameover()
    {
        Logger.Instance.Log("ゲームオーバーとして戦闘を終了します。");
        _battleWindowManager.HideAllWindow();
        _battleSpriteController.HideBackground();
        _battleSpriteController.HideEnemy();
        EnemyStatusManager.Instance.InitializeEnemyStatusList();
        _battleActionProcessor.InitializeActions();
        _battleActionProcessor.StopActions();

        // _characterMoverManager.ResumeCharacterMover();
        BattlePhase = BattlePhase.NotInBattle;
    }
}