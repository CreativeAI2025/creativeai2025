using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Tracing;
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

    [SerializeField]
    SkillAnimationManager _skillAnimationManager;
    /// <summary>
    /// 戦闘のフェーズです。
    /// </summary>
    public BattlePhase BattlePhase { get; private set; }

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

    /// <summary>
    /// 現在コマンドを選択しているキャラクターのリストのn番目
    /// </summary>
    /// <value></value>
    public int CharacterCursor { get; private set; }
    bool RunSelect = false;
    public event Action OnBattleStart { add => _onBattleStart += value; remove => _onBattleStart -= value; }
    private Action _onBattleStart;
    public event Action OnBattleEnd { add => _onBattleEnd += value; remove => _onBattleEnd -= value; }
    private Action _onBattleEnd;
    // 戦闘データ
    public BattleData BattleData { get; private set; }

    /// <summary>
    /// jsonファイルを指定して、戦闘を開始する
    /// </summary>
    /// <param name="filename"></param> <summary>
    /// jsonファイル名（拡張子.jsonは不要）
    /// </summary>
    /// <param name="filename"></param>
    public void InitializeFromJson(string fileName)
    {
        string filePath = string.Join('/', "BattleData", fileName + ".json");
        IFileAssetLoader loader = SaveUtility.FileAssetLoaderFactory();
        string assetsPath = loader.GetPath(filePath);
        BattleData = SaveUtility.JsonToData<BattleData>(assetsPath);
        SetUpEnemyStatus(new List<int>(BattleData.EnemyIds));
        Initialize();
    }

    /// <summary>
    /// 直接IDを指定して戦闘を開始する
    /// </summary>
    /// <param name="enemyIds"></param>
    public void InitializeFromIds(List<int> enemyIds)
    {
        BattleData = new BattleData();
        BattleData.EnemyIds = enemyIds.ToArray();
        BattleData.BGM = "bgm_5";    // BGMの設定（エンカウントなので、基本的には雑魚戦）
        int enemyId = enemyIds[0];
        var enemyData = EnemyDataManager.Instance.GetEnemyDataById(enemyId);
        // エンカウントした敵の数に応じて、敵出現メッセージを変える
        if (enemyIds.Count == 1)
        {
            StringBuilder sb = new StringBuilder(enemyData.enemyName);
            sb.Append(BattleMessage.EnemyAppearSuffix);
            BattleData.EncounterMessage = sb.ToString();
        }
        else if (enemyIds.Count <= 4)
        {
            StringBuilder sb = new StringBuilder(enemyData.enemyName);
            sb.Append(BattleMessage.EnemiesAppearSuffix);
            BattleData.EncounterMessage = sb.ToString();
        }
        else
        {
            BattleData.EncounterMessage = BattleMessage.EnemyMaxAppearText;
        }
        SetUpEnemyStatus(enemyIds);
        Initialize();
    }

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
        EnemyStatusManager.Instance.SetUpEnemyStatus(ids);
    }

    /// <summary>
    /// 戦闘の開始処理
    /// </summary>
    private void Initialize()
    {
        //SetPlayerStatus();  // プレイヤー周りの情報をセットする
        Logger.Instance.Log("戦闘を開始します。");
        _onBattleStart?.Invoke();
        SoundManager.Instance.ChangeBGM(BattleData.BGM);
        //  GameStateManager.ChangeToBattle();
        SetBattlePhase(BattlePhase.ShowEnemy);
        TurnCount = 1;
        IsBattleFinished = false;
        CharacterCursor = 0;    // キャラクターメンバーのリストの添え字を「０」にする
        _battleWindowManager.SetUpWindowControllers(this);
        var messageWindowController = _battleWindowManager.GetMessageWindowController();
        messageWindowController.HidePager();
        _battleActionProcessor.InitializeProcessor(this);
        _battleActionRegister.InitializeRegister(_battleActionProcessor);
        _enemyCommandSelector.SetReferences(this, _battleActionRegister);
        _battleResultManager.SetReferences(this);
        _skillAnimationManager.SetReferences(this);
        statusEffectManager = GetStatusEffectManager();
        statusEffectManager.SetBattleManager(this);
        // _characterMoverManager.StopCharacterMover();
        _battleStarter.StartBattle(this);
        _battleSpriteController.ShowBackground();
        ShowEnemyAppearMessage(BattleData.EncounterMessage);
    }

    /// <summary>
    /// エンカウント時のテキストメッセージを表示する
    /// </summary>
    /// <param name="message"></param>
    private void ShowEnemyAppearMessage(string message)
    {
        var controller = GetWindowManager().GetMessageWindowController();
        controller.ShowWindow();
        controller.GenerateEnemyAppearMessageDirect(message, 2.0f);
    }

    /// <summary>
    /// 味方キャラクターのステータスをセットします。
    /// 多分、ここはいらない（デバッグ用）
    /// </summary>
    private void SetPlayerStatus()
    {

        // 所持アイテムをセットします。
        PartyItemInfo item = new()
        {
            itemId = 101,
            itemNum = 5,
            usedNum = 1
        };
        CharacterStatusManager.Instance.partyItemInfoList = new()
        {
            item
        };
    }

    /// <summary>
    /// 追記: 次の行動可能な味方キャラクターのインデックスを取得します。
    /// -1 は全員行動済み、または次にアクティブなキャラクターが見つからないことを示します。
    /// </summary>
    private int GetNextActiveCharacterIndex(int startIndex)
    {
        for (int i = startIndex; i < CharacterStatusManager.Instance.partyCharacter.Count; i++)
        {
            int charaId = CharacterStatusManager.Instance.partyCharacter[i];

            // 行動不能（Defeated または Stop）ではないことを確認
            if (!CharacterStatusManager.Instance.IsCharacterDefeated(charaId) &&
                !CharacterStatusManager.Instance.IsCharacterStop(charaId))
            {
                return i; // 次の行動可能なキャラクターのインデックス
            }
        }
        return -1; // 全員行動済み
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
public SkillAnimationManager GetSkillAnimationManager()
    {
        return _skillAnimationManager;
    }

    /// <summary>
    /// コマンド入力を開始（敵が現れたあとや、ターンが終わったあとに呼ばれる）
    /// </summary>
    public void StartInputCommandPhase()
    {
        Logger.Instance.Log($"コマンド入力のフェーズを開始します。現在のターン数: {TurnCount}");
        // 最初の行動可能なキャラクターのカーソル位置を設定
        CharacterCursor = GetNextActiveCharacterIndex(0);
        if (CharacterCursor == -1)
        {
            Logger.Instance.LogWarning("味方全員が行動不能です。即座に敵フェーズへ移行します。");
            // 全員行動不能の場合、敵の行動をスキップしてターン終了しても良いが、ここではPostCommandSelectに任せる
            PostCommandSelect();
            return;
        }
        var messageWindowController = _battleWindowManager.GetMessageWindowController();
        messageWindowController.HideWindow();
        BattlePhase = BattlePhase.InputCommand;
        _battleActionProcessor.InitializeActions();

        // コマンドウィンドウを現在のアクターに合わせて再表示・初期化
        _battleWindowManager.GetCommandWindowController().ShowWindow();
        _battleWindowManager.GetCommandWindowController().InitializeCommand();
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
                StartTargetSelection(BattleCommand.Attack, 0);
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
    public void OnItemSelected(int selectedItemId)
    {
        Logger.Instance.Log($"項目が選択されました: ItemID/SkillID = {selectedItemId}");

        // 選択ウィンドウ（アイテム/スキルリスト）を閉じる
        _battleWindowManager.GetSelectionWindowController().HideWindow();

        // ターゲット選択へ移行
        StartTargetSelection(SelectedCommand, selectedItemId);

    }
    /// <summary>
    /// 💡 新規: ターゲット選択フェーズを開始します。
    /// </summary>
    void StartTargetSelection(BattleCommand command, int itemId)
    {
        Logger.Instance.Log($"ターゲット選択フェーズを開始します。コマンド: {command}");

        // ターゲット選択フェーズへ移行
        SetBattlePhase(BattlePhase.SelectTarget);

        var targetSelectionController = _battleWindowManager.GetTargetSelectionWindowController(); // 仮の新規コントローラー

        // ターゲット選択に必要な情報（アクションの種類、ID）を渡す
        targetSelectionController.SetUpTargets(command, itemId, CharacterCursor);
        targetSelectionController.ShowWindow();
    }

    /// <summary>
    /// 💡 新規: ターゲット選択ウィンドウでターゲットが決定された時のコールバックです。
    /// </summary>
    public void OnTargetSelected(List<int> targetIds, bool isTargetFriend, int itemId)
    {
        Logger.Instance.Log($"ターゲットが決定されました。ターゲット数: {targetIds.Count}");

        // ターゲット選択ウィンドウを非表示にする（ここではTargetSelectionControllerが実行すると仮定）


        // 選択されたコマンドに応じてアクションを登録
        switch (SelectedCommand)
        {
            case BattleCommand.Attack:
                SetAttackCommandAction(targetIds);
                break;
            case BattleCommand.Skill:
                SetSkillCommandAction(targetIds, isTargetFriend, itemId);
                break;
            case BattleCommand.Item:
                SetItemCommandAction(targetIds, isTargetFriend, itemId);
                break;
        }
        StartCoroutine(DelayPostCommandSelect());
    }
    private IEnumerator DelayPostCommandSelect()
    {
        // 1フレーム待つことでUIの非表示処理を完了させる
        yield return null;

        SetBattlePhase(BattlePhase.InputCommand);
        PostCommandSelect();
    }

    /// <summary>
    /// 攻撃コマンドを選択した際の処理です。（ターゲット選択後の最終登録）
    /// </summary>
    /// <param name="targetIds">ターゲットのIDリスト</param>
    void SetAttackCommandAction(List<int> targetIds)
    {
        int actorId = CharacterStatusManager.Instance.partyCharacter[CharacterCursor];

        // 💡 修正: 複数ターゲットに対応したRegisterActionを呼び出す
        _battleActionRegister.SetFriendAttackAction(actorId, targetIds);

        Logger.Instance.Log($"攻撃するキャラクターのID: {actorId} || 攻撃対象のキャラクターの数: {targetIds.Count}");
    }

    /// <summary>
    /// 魔法コマンドを選択した際の処理です。（ターゲット選択後の最終登録）
    /// </summary>
    /// <param name="targetIds">ターゲットのIDリスト</param>
    /// <param name="isTargetFriend">ターゲットが味方か</param>
    /// <param name="skillId">魔法のID</param>
    void SetSkillCommandAction(List<int> targetIds, bool isTargetFriend, int skillId)
    {
        int actorId = CharacterStatusManager.Instance.partyCharacter[CharacterCursor];

        // 💡 修正: 複数ターゲットに対応したRegisterActionを呼び出す
        _battleActionRegister.SetFriendSkillAction(actorId, targetIds, isTargetFriend, skillId);

        Logger.Instance.Log($"攻撃するキャラクターのID: {actorId} || 攻撃対象のキャラクターの数: {targetIds.Count}");
    }

    /// <summary>
    /// アイテムコマンドを選択した際の処理です。（ターゲット選択後の最終登録）
    /// </summary>
    /// <param name="targetIds">ターゲットのIDリスト</param>
    /// <param name="isTargetFriend">ターゲットが味方か</param>
    /// <param name="itemId">アイテムのID</param>
    void SetItemCommandAction(List<int> targetIds, bool isTargetFriend, int itemId)
    {
        Logger.Instance.Log($"SetItemCommandAction()が呼ばれました。選択されたアイテムのID : {itemId}");
        int actorId = CharacterStatusManager.Instance.partyCharacter[CharacterCursor];

        // 💡 修正: 複数ターゲットに対応したRegisterActionを呼び出す
        _battleActionRegister.SetFriendItemAction(actorId, targetIds, isTargetFriend, itemId);

        // ... (Logger)
    }

    /// <summary>
    /// 逃げるコマンドを選択した際の処理です。
    /// </summary>
    void SetRunCommandAction()
    {
        int actorId = CharacterStatusManager.Instance.partyCharacter[0];
        _battleActionRegister.SetFriendRunAction(actorId);
        RunSelect = true;
        Logger.Instance.Log($"逃げるコマンドが選択されました");
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
        // 修正: 次のキャラクターへカーソルを移動させるか、敵フェーズへ移行
        int nextIndex = GetNextActiveCharacterIndex(CharacterCursor + 1);

        if (nextIndex != -1 && RunSelect == false)
        {
            // 次のキャラクターへ入力を移行
            CharacterCursor = nextIndex;
            Logger.Instance.Log($"次のキャラクターの入力へ移行します。Cursor: {CharacterCursor}");

            // UIの再表示 (次のキャラクターのステータスやコマンドUIへ切り替える処理が別途必要)
            _battleWindowManager.GetCommandWindowController().ShowWindow();
            _battleWindowManager.GetCommandWindowController().InitializeCommand();

            SetBattlePhase(BattlePhase.InputCommand);
            Logger.Instance.Log($"今の状態{BattlePhase}");
        }
        else
        {
            // 味方全員の入力が完了
            Logger.Instance.Log("味方全員のコマンド入力が完了しました。敵のコマンド入力を行います。");
            _battleWindowManager.GetCommandWindowController().HideWindow(); // コマンドウィンドウを非表示
            _enemyCommandSelector.SelectEnemyCommand();
        }
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
        OnBattleWin();  // デバッグ用に勝ち判定にする
        //OnFinishBattle();
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
    /// 絶対にこれを呼ぶこと
    /// </summary>
    private void OnFinishBattle()
    {
        Logger.Instance.Log("戦闘終了");
        _onBattleEnd?.Invoke(); // 戦闘が終了したことを伝える

        _battleWindowManager.HideAllWindow();
        _battleSpriteController.HideBackground();
        _battleSpriteController.HideEnemy();
        EnemyStatusManager.Instance.InitializeEnemyStatusList();
        _battleActionProcessor.InitializeActions();
        _battleActionProcessor.StopActions();

        //_characterMoverManager.ResumeCharacterMover();
        BattlePhase = BattlePhase.NotInBattle;
        SoundManager.Instance.SetCurrentSceneBGM();
    }

    public void OnBattleWin()
    {
        Debug.Log("勝利！！");
        var nextFlags = BattleData.WinFlags;
        if (nextFlags != null)
            ChangeFlag(nextFlags);

        OnFinishBattle();
    }

    public void OnBattleLose()
    {
        Debug.Log("敗北者じゃけぇ");
        var nextFlags = BattleData.LoseFlags;
        if (nextFlags != null)
            ChangeFlag(nextFlags);

        OnFinishBattle();
    }

    /// <summary>
    /// 引数で与えられたフラグ名を、
    /// 同じく引数で与えられた状態（true/false）に変える関数
    /// </summary>
    /// <param name="nextFlag"></param>
    private void ChangeFlag(KeyValuePair<string, bool>[] nextFlags)
    {
        foreach (KeyValuePair<string, bool> flag in nextFlags)
        {
            string flagName = flag.Key;
            bool flagValue = flag.Value;
            Debug.Log(flagName + ":" + flagValue);
            if (flagValue)
            {
                FlagManager.Instance.AddFlag(flagName);
            }
            else
            {
                FlagManager.Instance.DeleteFlag(flagName);
            }
        }
    }
}