using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleActionProcessorSkill : MonoBehaviour
{
    BattleActionProcessor _actionProcessor;
    BattleManager _battleManager;
    MessageWindowController _messageWindowController;
    BattleSpriteController _battleSpriteController;

    // 追加: 状態異常マネージャー
    [SerializeField] private StatusEffectManager statusEffectManager;

    bool _pauseSkillEffect;

    // 追記: ターゲットIDリストを生成するヘルパーメソッド
    private List<int> GetEffectiveTargetIds(BattleAction action, SkillData skillData)
    {
        var skillEffect = skillData.skillEffects.FirstOrDefault(); // 最初の効果の範囲を使うと仮定
        if (skillEffect == null) return new List<int>();

        // ターゲット属性を判定し、対象リストを生成
        switch (skillEffect.EffectTarget)
        {
            case EffectTarget.Own:
                return new List<int> { action.actorId };

            case EffectTarget.FriendSolo:
                return action.targetIds; // ターゲット選択UIで選択されたID（単体）をそのまま使用

            case EffectTarget.FriendAll:
                // 全味方キャラクターのIDを返す
                return CharacterStatusManager.Instance.partyCharacter.Where(id =>
                    !CharacterStatusManager.Instance.IsCharacterDefeated(id)).ToList();

            case EffectTarget.EnemySolo:
                return action.targetIds; // ターゲット選択UIで選択された敵ID（単体）をそのまま使用

            case EffectTarget.EnemyAll:
                // 全ての敵キャラクターの戦闘中IDを返す
                return EnemyStatusManager.Instance.GetEnemyStatusList().Where(status =>
                    !status.isDefeated && !status.isRunaway).Select(status => status.enemyBattleId).ToList();

            default:
                return action.targetIds; // デフォルトで登録されたターゲットIDリストを返す
        }
    }

    // 追記: ターゲットIDが味方か敵か判定するヘルパーメソッド
    private bool IsTargetFriend(int targetId, bool isActorFriend, SkillEffect skillEffect)
    {
        // 味方がアクションを行った場合、回復/バフは味方、ダメージは敵
        if (isActorFriend)
        {
            return skillEffect.EffectTarget == EffectTarget.Own ||
                   skillEffect.EffectTarget == EffectTarget.FriendSolo ||
                   skillEffect.EffectTarget == EffectTarget.FriendAll;
        }
        // 敵がアクションを行った場合
        else
        {
            // 敵の回復/バフは敵、ダメージは味方
            return skillEffect.EffectTarget != EffectTarget.EnemySolo &&
                   skillEffect.EffectTarget != EffectTarget.EnemyAll;
        }
    }


    IEnumerator ProcessSkillActionCoroutine(BattleAction action, SkillData skillData)
    {
        // ... (MP消費処理) ...

        _actionProcessor.SetPauseProcess(true);

        // 修正: 有効なターゲットのみでリストを再構築（他のアクションで倒された敵を除外）
        List<int> effectiveTargetIds = _actionProcessor.GetValidTargets(action.targetIds, action.isTargetFriend);


        // 魔法詠唱メッセージを一度だけ表示（ここは変更なし）
        string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
        _actionProcessor.SetPauseMessage(true);
        _messageWindowController.GenerateSkillCastMessage(actorName, skillData.skillName);
        while (_actionProcessor.IsPausedMessage) yield return null;

        // 追記: 詠唱メッセージの後に、メッセージウィンドウをクリアして次のメッセージに備えます
        // これにより、詠唱メッセージとダメージメッセージが混ざるのを防ぎます
        //_messageWindowController.GetMessageUIController().ClearMessage();

        // ----------------------------------------------------
        // 💡 複数ターゲットへの効果適用ループ
        // ----------------------------------------------------
        foreach (int currentTargetId in effectiveTargetIds)
        {
            // 💡 実行直前チェック: 既に戦闘終了している場合は、即座に中断
            if (_battleManager.IsBattleFinished)
            {
                yield break;
            }

            Logger.Instance.Log($"ターゲット ID:{currentTargetId} への処理を開始。");

            foreach (var skillEffect in skillData.skillEffects)
            {
                // ... (ターゲット判定、ダメージ計算、ステータス変更のロジックはそのまま) ...

                // --- ダメージ計算と適用 ---
                if (skillEffect.skillCategory == SkillCategory.Damage)
                {
                    // ... (ダメージ計算とステータス変更のロジックはそのまま) ...
                    int damageValue = 100;
                    int hpDelta = -damageValue;
                    bool isTargetFriend = IsTargetFriend(currentTargetId, action.isActorFriend, skillEffect);
                    bool isTargetDefeated = false;

                    // ステータス変更
                    if (isTargetFriend)
                    {
                        CharacterStatusManager.Instance.ChangeCharacterStatus(currentTargetId, hpDelta, 0);
                        isTargetDefeated = CharacterStatusManager.Instance.IsCharacterDefeated(currentTargetId);
                    }
                    else
                    {
                        EnemyStatusManager.Instance.ChangeEnemyStatus(currentTargetId, hpDelta, 0);
                        isTargetDefeated = EnemyStatusManager.Instance.IsEnemyDefeated(currentTargetId);

                        if (isTargetDefeated)
                            EnemyStatusManager.Instance.OnDefeatEnemy(currentTargetId);
                    }

                    // 1. ダメージメッセージ表示と待機
                    _actionProcessor.SetPauseMessage(true); // 💡 メッセージポーズ開始
                    string targetName = _actionProcessor.GetCharacterName(currentTargetId, isTargetFriend);
                    _messageWindowController.GenerateDamageMessage(targetName, damageValue);
                    _battleManager.OnUpdateStatus();
                    while (_actionProcessor.IsPausedMessage) yield return null; // 💡 メッセージ完了まで待機

                    // 2. 撃破メッセージ表示と待機
                    if (isTargetDefeated)
                    {
                        _actionProcessor.SetPauseMessage(true); // 💡 メッセージポーズ開始
                        if (isTargetFriend)
                        {
                            _messageWindowController.GenerateDefeateFriendMessage(targetName);
                        }
                        else
                        {
                            _battleSpriteController.RefreshActiveEnemies();
                            _messageWindowController.GenerateDefeateEnemyMessage(targetName);
                        }
                        while (_actionProcessor.IsPausedMessage) yield return null; // 💡 メッセージ完了まで待機

                        // 勝利/ゲームオーバー判定
                        if (EnemyStatusManager.Instance.IsAllEnemyDefeated())
                            _battleManager.OnEnemyDefeated();
                        if (CharacterStatusManager.Instance.IsAllCharacterDefeated())
                            _battleManager.OnGameover();

                        // 💡 修正: 戦闘が終了したら、即座にコルーチンを終了
                        if (_battleManager.IsBattleFinished)
                        {
                            yield break;
                        }
                    }
                }
                // --- 回復計算と適用 ---
                else if (skillEffect.skillCategory == SkillCategory.Recovery)
                {
                    int healValue = DamageFormula.CalculateHealValue(skillEffect.value);
                    bool isTargetFriend = IsTargetFriend(currentTargetId, action.isActorFriend, skillEffect);

                    // ステータス変更
                    if (isTargetFriend)
                        CharacterStatusManager.Instance.ChangeCharacterStatus(currentTargetId, healValue, 0);
                    else
                        EnemyStatusManager.Instance.ChangeEnemyStatus(currentTargetId, healValue, 0);

                    // 回復メッセージ表示と待機
                    _actionProcessor.SetPauseMessage(true); // 💡 メッセージポーズ開始
                    string targetName = _actionProcessor.GetCharacterName(currentTargetId, isTargetFriend);
                    _messageWindowController.GenerateHpHealMessage(targetName, healValue);
                    _battleManager.OnUpdateStatus();
                    while (_actionProcessor.IsPausedMessage) yield return null; // 💡 メッセージ完了まで待機
                }

                // 修正: ターゲットの処理が終わったら、次のターゲットに進む前にユーザー入力待ちを挟む
                if (!_battleManager.IsBattleFinished)
                {
                    yield return StartCoroutine(WaitForUserInput());
                }
            }
        } // ターゲットループ終了

        // 全てのターゲット処理が完了し、戦闘が終了していない場合にのみプロセスを再開
        if (!_battleManager.IsBattleFinished)
        {
            _actionProcessor.SetPauseProcess(false);
        }
    }

    /// <summary>
    ///💡 新規コルーチン: ターゲット処理の区切りとして、ユーザー入力によるメッセージクリアと待機を行います。
    /// </summary>
    IEnumerator WaitForUserInput()
    {
        // 1. ページャーを表示してキー入力を促す
        _messageWindowController.ShowPager();

        // 2. メッセージの待機フラグをセット
        _actionProcessor.SetPauseMessage(true);

        // 3. ユーザーがキーを押すのを待つ
        while (_messageWindowController._waitKeyInput)
        {
            yield return null;
        }

        // 4. ページャーを非表示にし、メッセージをクリアして次のターゲットに備える
        _messageWindowController.HidePager();
        _messageWindowController.ClearMessage();
    }

    public void SetReferences(BattleManager battleManager, BattleActionProcessor actionProcessor)
    {
        _battleManager = battleManager;
        _actionProcessor = actionProcessor;
        _messageWindowController = _battleManager.GetWindowManager().GetMessageWindowController();
        _battleSpriteController = _battleManager.GetBattleSpriteController();
        statusEffectManager = _battleManager.GetStatusEffectManager();
    }

    public void ProcessAction(BattleAction action)
    {
        var skillData = SkillDataManager.Instance.GetSkillDataById(action.itemId);

        // 消費MP処理
        int hpDelta = 0;
        int mpDelta = skillData.cost * -1;
        if (action.isActorFriend)
            CharacterStatusManager.Instance.ChangeCharacterStatus(action.actorId, hpDelta, mpDelta);
        else
            EnemyStatusManager.Instance.ChangeEnemyStatus(action.actorId, hpDelta, mpDelta);

        _actionProcessor.SetPauseProcess(true);
        StartCoroutine(ProcessSkillActionCoroutine(action, skillData));
    }

    IEnumerator ShowSkillHealMessage(BattleAction action, string skillName, int healValue)
    {
        string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);

        foreach (var targetId in action.targetIds)
        {
            string targetName = _actionProcessor.GetCharacterName(targetId, action.isTargetFriend);
            _actionProcessor.SetPauseMessage(true);
            _messageWindowController.GenerateSkillCastMessage(actorName, skillName);
            while (_actionProcessor.IsPausedMessage) yield return null;

            _actionProcessor.SetPauseMessage(true);
            _messageWindowController.GenerateHpHealMessage(targetName, healValue);
            _battleManager.OnUpdateStatus();
            while (_actionProcessor.IsPausedMessage) yield return null;
        }

        _pauseSkillEffect = false;
    }

    bool IsSkillTargetFriend(SkillEffect skillEffect)
    {
        return skillEffect.EffectTarget == EffectTarget.Own
            || skillEffect.EffectTarget == EffectTarget.FriendSolo
            || skillEffect.EffectTarget == EffectTarget.FriendAll;
    }
}
