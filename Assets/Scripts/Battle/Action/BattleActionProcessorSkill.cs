using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleActionProcessorSkill : MonoBehaviour
{
    BattleActionProcessor _actionProcessor;
    BattleManager _battleManager;
    MessageWindowController _messageWindowController;
    EnemyStatusManager _enemyStatusManager;
    BattleSpriteController _battleSpriteController;

    // 追加: 状態異常マネージャー
    [SerializeField] private StatusEffectManager statusEffectManager;

    bool _pauseSkillEffect;

    // 追記: ターゲットIDリストを生成するヘルパーメソッド
    private List<int> GetEffectiveTargetIds(BattleAction action, SkillData skillData)
    {
        var skillEffect = skillData.skillEffect; // 最初の効果の範囲を使うと仮定
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


    /// <summary>
    /// スキルのアクションを記述していく
    /// エフェクトテゴリーによる条件分岐で、適切な効果を付与する
    /// </summary>
    /// <param name="action"></param>
    /// <param name="skillData"></param>
    /// <returns></returns>
    IEnumerator ProcessSkillActionCoroutine(BattleAction action, SkillData skillData)
    {
        // ... (MP消費処理) ...



        // 消費MP処理
        int hpDelta = 0;
        int mpDelta = skillData.cost * -1;
        if (action.isActorFriend)
            CharacterStatusManager.Instance.ChangeCharacterStatus(action.actorId, hpDelta, mpDelta);
        else
            _enemyStatusManager.ChangeEnemyStatus(action.actorId, hpDelta, mpDelta);

        _actionProcessor.SetPauseProcess(true);



        // 修正: 有効なターゲットのみでリストを再構築（他のアクションで倒された敵を除外）
        List<int> effectiveTargetIds = _actionProcessor.GetValidTargets(action.targetIds, action.isTargetFriend);

        // 魔法詠唱メッセージを一度だけ表示
        string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
        _actionProcessor.SetPauseMessage(true);
        _messageWindowController.GenerateSkillCastMessage(actorName, skillData.skillName);
        while (_actionProcessor.IsPausedMessage) yield return null;

        // 追記: 詠唱メッセージの後に、メッセージウィンドウをクリアして次のメッセージに備えます
        // これにより、詠唱メッセージとダメージメッセージが混ざるのを防ぎます
        //_messageWindowController.GetMessageUIController().ClearMessage();
        var actorParam = _actionProcessor.GetCharacterParameter(action.actorId, action.isActorFriend);
        // var targetParam = _actionProcessor.GetCharacterParameter(action.targetIds, action.isTargetFriend);

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
            var skillEffect = skillData.skillEffect;

            // --- ダメージ計算と適用 ---
            if (skillEffect.skillCategory == SkillCategory.PhysicalDamage)
            {
                bool isTargetFriend = IsTargetFriend(currentTargetId, action.isActorFriend, skillEffect);
                float buffValue = 0.0f;
                // 基本パラメータの取得
                actorParam = _actionProcessor.GetCharacterParameter(action.actorId, action.isActorFriend);
                var targetParam = _actionProcessor.GetCharacterParameter(currentTargetId, isTargetFriend);
                StatusEffectCategory? appliedEffectCategory = null;
                BuffStatusCategory? appliedBuffCategory = null;
                // バフ/デバフ倍率の取得
                int damageValue = 0;
                if (isTargetFriend)
                {
                    var characterStatus = CharacterStatusManager.Instance.GetCharacterStatusById(currentTargetId);
                    var enemyStatus = EnemyStatusManager.Instance.GetEnemyStatusByBattleId(action.actorId);
                    damageValue = DamageFormula.CalculateSkillDamage(actorParam.Attack, targetParam.Defence, enemyStatus.attackBuffMultiplier, characterStatus.defenceBuffMultiplier, skillEffect.value);

                }
                else
                {
                    var characterStatus = CharacterStatusManager.Instance.GetCharacterStatusById(action.actorId);
                    var enemyStatus = EnemyStatusManager.Instance.GetEnemyStatusByBattleId(currentTargetId);
                    damageValue = DamageFormula.CalculateSkillDamage(actorParam.Attack, targetParam.Defence, characterStatus.attackBuffMultiplier, enemyStatus.defenceBuffMultiplier, skillEffect.value);

                }
                // {
                //     var charaStatus = CharacterStatusManager.Instance.GetCharacterStatusById(action.actorId);
                //     var targetStatus = isTargetFriend
                //         ? (object)charaStatus
                //         : EnemyStatusManager.Instance.GetEnemyStatusByBattleId(currentTargetId);
                // }
                // else
                // {

                // }

                // ... (ダメージ計算とステータス変更のロジックはそのまま) ...
                hpDelta = -damageValue;
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
                foreach (var statusEffect in skillEffect.StatusEffect)
                {
                    appliedEffectCategory = statusEffect.EffectCategory;
                    Logger.Instance.Log("状態異常付与");
                    if (isTargetFriend)
                    {
                        statusEffectManager.ApplyStatusEffectToPlayer(currentTargetId, statusEffect);
                    }
                    else
                    {
                        statusEffectManager.ApplyStatusEffectToEnemy(currentTargetId, statusEffect);
                    }
                    string statusMessage = "";
                    if (appliedEffectCategory != null)
                    {
                        appliedEffectCategory = statusEffect.EffectCategory;
                        Logger.Instance.Log("状態異常付与");
                        switch (appliedEffectCategory)
                        {
                            case StatusEffectCategory.Poison:
                                statusMessage = BattleMessage.PoisonSuffix;
                                break;
                            case StatusEffectCategory.Paralysis:
                                statusMessage = BattleMessage.ParalysisSuffix;
                                break;
                            case StatusEffectCategory.Sleep:
                                statusMessage = BattleMessage.SleepSuffix;
                                break;
                            case StatusEffectCategory.Confusion:
                                statusMessage = BattleMessage.ConfusionSuffix;
                                break;
                        }
                        _actionProcessor.SetPauseMessage(true); // 💡 メッセージポーズ開始
                        _messageWindowController.GenerateStatusAilmentMessage(targetName, statusMessage);
                        _battleManager.OnUpdateStatus();
                        while (_actionProcessor.IsPausedMessage) yield return null; // 💡 メッセージ完了まで待機
                    }
                }
                foreach (var buff in skillEffect.buff)
                {
                    appliedBuffCategory = buff.BuffCategory;
                    buffValue = buff.Power;
                    bool buffTarget = IsBuffTargetFriend(buff);
                    Logger.Instance.Log("バフデバフ付与");
                    string buffMessage = "";
                    if (appliedBuffCategory != null)
                    {
                        if (buffTarget)
                        {
                            statusEffectManager.PlayerApplyBuff(currentTargetId, buff);
                        }
                        else
                        {
                            statusEffectManager.EnemyApplyBuff(currentTargetId, buff);
                        }
                        switch (appliedBuffCategory)
                        {
                            case BuffStatusCategory.Attack:
                                buffMessage = BattleMessage.AttackStatusSuffix;
                                break;
                            case BuffStatusCategory.Defence:
                                buffMessage = BattleMessage.DefenceStatusSuffix;
                                break;
                            case BuffStatusCategory.MagicAttack:
                                buffMessage = BattleMessage.MagicAttackStatusSuffix;
                                break;
                            case BuffStatusCategory.MagicDefence:
                                buffMessage = BattleMessage.MagicDefenceStatusSuffix;
                                break;
                            case BuffStatusCategory.Speed:
                                buffMessage = BattleMessage.SpeedStatusSuffix;
                                break;
                            case BuffStatusCategory.Evasion:
                                buffMessage = BattleMessage.EvasionStatusSuffix;
                                break;
                        }
                        string buffTargetName = _actionProcessor.GetCharacterName(currentTargetId, isTargetFriend);
                        _actionProcessor.SetPauseMessage(true); // 💡 メッセージポーズ開始
                        _messageWindowController.GenerateRecoverStatusMessage(buffTargetName, buffMessage, buffValue);
                        _battleManager.OnUpdateStatus();
                        while (_actionProcessor.IsPausedMessage) yield return null; // 💡 メッセージ完了まで待機
                    }
                }
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
            else if (skillEffect.skillCategory == SkillCategory.MagicDamage)
            {
                bool isTargetFriend = IsTargetFriend(currentTargetId, action.isActorFriend, skillEffect);
                float buffValue = 0.0f;
                // 基本パラメータの取得
                actorParam = _actionProcessor.GetCharacterParameter(action.actorId, action.isActorFriend);
                var targetParam = _actionProcessor.GetCharacterParameter(currentTargetId, isTargetFriend);
                StatusEffectCategory? appliedEffectCategory = null;
                BuffStatusCategory? appliedBuffCategory = null;
                // バフ/デバフ倍率の取得
                float actorAttackBuff = 1.0f;
                float targetDefenceBuff = 1.0f;
                int damageValue = 0;
                if (isTargetFriend)
                {
                    var characterStatus = CharacterStatusManager.Instance.GetCharacterStatusById(currentTargetId);
                    var enemyStatus = EnemyStatusManager.Instance.GetEnemyStatusByBattleId(action.actorId);
                    damageValue = DamageFormula.CalculateSkillDamage(actorParam.MagicAttack, targetParam.MagicDefence, enemyStatus.magicAttackBuffMultiplier, characterStatus.magicDefenceBuffMultiplier, skillEffect.value);

                }
                else
                {
                    var characterStatus = CharacterStatusManager.Instance.GetCharacterStatusById(action.actorId);
                    var enemyStatus = EnemyStatusManager.Instance.GetEnemyStatusByBattleId(currentTargetId);
                    damageValue = DamageFormula.CalculateSkillDamage(actorParam.MagicAttack, targetParam.MagicDefence, characterStatus.magicAttackBuffMultiplier, enemyStatus.magicDefenceBuffMultiplier, skillEffect.value);

                }
                // {
                //     var charaStatus = CharacterStatusManager.Instance.GetCharacterStatusById(action.actorId);
                //     var targetStatus = isTargetFriend
                //         ? (object)charaStatus
                //         : EnemyStatusManager.Instance.GetEnemyStatusByBattleId(currentTargetId);
                // }
                // else
                // {

                // }

                // ... (ダメージ計算とステータス変更のロジックはそのまま) ...
                hpDelta = -damageValue;
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
                foreach (var statusEffect in skillEffect.StatusEffect)
                {
                    appliedEffectCategory = statusEffect.EffectCategory;
                    Logger.Instance.Log("状態異常付与");
                    if (isTargetFriend)
                    {
                        statusEffectManager.ApplyStatusEffectToPlayer(currentTargetId, statusEffect);
                    }
                    else
                    {
                        statusEffectManager.ApplyStatusEffectToEnemy(currentTargetId, statusEffect);
                    }
                    string statusMessage = "";
                    if (appliedEffectCategory != null)
                    {
                        appliedEffectCategory = statusEffect.EffectCategory;
                        Logger.Instance.Log("状態異常付与");
                        switch (appliedEffectCategory)
                        {
                            case StatusEffectCategory.Poison:
                                statusMessage = BattleMessage.PoisonSuffix;
                                break;
                            case StatusEffectCategory.Paralysis:
                                statusMessage = BattleMessage.ParalysisSuffix;
                                break;
                            case StatusEffectCategory.Sleep:
                                statusMessage = BattleMessage.SleepSuffix;
                                break;
                            case StatusEffectCategory.Confusion:
                                statusMessage = BattleMessage.ConfusionSuffix;
                                break;
                        }

                        _actionProcessor.SetPauseMessage(true); // 💡 メッセージポーズ開始
                        _messageWindowController.GenerateStatusAilmentMessage(targetName, statusMessage);
                        _battleManager.OnUpdateStatus();
                        while (_actionProcessor.IsPausedMessage) yield return null; // 💡 メッセージ完了まで待機
                    }
                }
                foreach (var buff in skillEffect.buff)
                {
                    appliedBuffCategory = buff.BuffCategory;
                    buffValue = buff.Power;
                    bool buffTarget = IsBuffTargetFriend(buff);
                    Logger.Instance.Log("バフデバフ付与");
                    string buffMessage = "";
                    if (appliedBuffCategory != null)
                    {
                        if (buffTarget)
                        {
                            statusEffectManager.PlayerApplyBuff(currentTargetId, buff);
                        }
                        else
                        {
                            statusEffectManager.EnemyApplyBuff(currentTargetId, buff);
                        }
                        switch (appliedBuffCategory)
                        {
                            case BuffStatusCategory.Attack:
                                buffMessage = BattleMessage.AttackStatusSuffix;
                                break;
                            case BuffStatusCategory.Defence:
                                buffMessage = BattleMessage.DefenceStatusSuffix;
                                break;
                            case BuffStatusCategory.MagicAttack:
                                buffMessage = BattleMessage.MagicAttackStatusSuffix;
                                break;
                            case BuffStatusCategory.MagicDefence:
                                buffMessage = BattleMessage.MagicDefenceStatusSuffix;
                                break;
                            case BuffStatusCategory.Speed:
                                buffMessage = BattleMessage.SpeedStatusSuffix;
                                break;
                            case BuffStatusCategory.Evasion:
                                buffMessage = BattleMessage.EvasionStatusSuffix;
                                break;
                        }
                        string buffTargetName = _actionProcessor.GetCharacterName(currentTargetId, isTargetFriend);
                        _actionProcessor.SetPauseMessage(true); // 💡 メッセージポーズ開始
                        _messageWindowController.GenerateRecoverStatusMessage(buffTargetName, buffMessage, buffValue);
                        _battleManager.OnUpdateStatus();
                        while (_actionProcessor.IsPausedMessage) yield return null; // 💡 メッセージ完了まで待機
                    }
                }


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
            // 状態異常の回復     
            else if (skillEffect.skillCategory == SkillCategory.EffectRecovery)
            {
                var characterStatus = CharacterStatusManager.Instance.GetCharacterStatusById(currentTargetId);
                bool isTargetFriend = IsTargetFriend(currentTargetId, action.isActorFriend, skillEffect);
                characterStatus.Duration = 0;
                characterStatus.Poison = false;
                characterStatus.Sleep = false;
                characterStatus.Paralysis = false;
                characterStatus.IsCharaStop = false;
                _actionProcessor.SetPauseMessage(true); // 💡 メッセージポーズ開始
                string targetName = _actionProcessor.GetCharacterName(currentTargetId, isTargetFriend);
                _messageWindowController.GenerateRecoverStatusMessage(targetName);
                _battleManager.OnUpdateStatus();
                while (_actionProcessor.IsPausedMessage) yield return null; // 💡 メッセージ完了まで待機
            }
            else if (skillEffect.skillCategory == SkillCategory.Revive)
            {
                int healValue = DamageFormula.CalculateHealValue(skillEffect.value);
                var characterStatus = CharacterStatusManager.Instance.GetCharacterStatusById(currentTargetId);
                bool isTargetFriend = IsTargetFriend(currentTargetId, action.isActorFriend, skillEffect);
                characterStatus.isDefeated = false;
                CharacterStatusManager.Instance.ChangeCharacterStatus(currentTargetId, healValue, 0);
                _actionProcessor.SetPauseMessage(true); // 💡 メッセージポーズ開始
                string targetName = _actionProcessor.GetCharacterName(currentTargetId, isTargetFriend);
                _messageWindowController.GenerateReviveMessage(targetName, healValue);
                _battleManager.OnUpdateStatus();
                while (_actionProcessor.IsPausedMessage) yield return null; // 💡 メッセージ完了まで待機
            }

            else if (skillEffect.skillCategory == SkillCategory.Buff) //バフデバフ込み
            {
                string buffMessage = "";
                bool isTargetFriend = IsTargetFriend(currentTargetId, action.isActorFriend, skillEffect);
                BuffStatusCategory? appliedBuffCategory = null;
                float buffValue = 0.0f;                            //バフデバフ付与

                foreach (var buff in skillEffect.buff)
                {
                    appliedBuffCategory = buff.BuffCategory;
                    buffValue = buff.Power;
                    Logger.Instance.Log("バフデバフ付与");
                    if (isTargetFriend)
                    {
                        statusEffectManager.PlayerApplyBuff(currentTargetId, buff);
                    }
                    else
                    {
                        statusEffectManager.EnemyApplyBuff(currentTargetId, buff);
                    }
                    if (appliedBuffCategory != null)
                    {
                        switch (appliedBuffCategory)
                        {
                            case BuffStatusCategory.Attack:
                                buffMessage = BattleMessage.AttackStatusSuffix;
                                break;
                            case BuffStatusCategory.Defence:
                                buffMessage = BattleMessage.DefenceStatusSuffix;
                                break;
                            case BuffStatusCategory.MagicAttack:
                                buffMessage = BattleMessage.MagicAttackStatusSuffix;
                                break;
                            case BuffStatusCategory.MagicDefence:
                                buffMessage = BattleMessage.MagicDefenceStatusSuffix;
                                break;
                            case BuffStatusCategory.Speed:
                                buffMessage = BattleMessage.SpeedStatusSuffix;
                                break;
                            case BuffStatusCategory.Evasion:
                                buffMessage = BattleMessage.EvasionStatusSuffix;
                                break;
                        }
                    }
                    string targetName = _actionProcessor.GetCharacterName(currentTargetId, isTargetFriend);
                    _actionProcessor.SetPauseMessage(true); // 💡 メッセージポーズ開始
                    _messageWindowController.GenerateRecoverStatusMessage(targetName, buffMessage, buffValue);
                    _battleManager.OnUpdateStatus();
                    while (_actionProcessor.IsPausedMessage) yield return null; // 💡 メッセージ完了まで待機
                }


            }
            else if (skillEffect.skillCategory == SkillCategory.DeBuff) //StatusEffect用
            {
                // 状態異常付与
                string statusMessage = "";
                bool isTargetFriend = IsTargetFriend(currentTargetId, action.isActorFriend, skillEffect);
                StatusEffectCategory? appliedEffectCategory = null;
                foreach (var statusEffect in skillEffect.StatusEffect)
                {
                    appliedEffectCategory = statusEffect.EffectCategory;
                    Logger.Instance.Log("状態異常付与");
                    if (isTargetFriend)
                    {
                        statusEffectManager.ApplyStatusEffectToPlayer(currentTargetId, statusEffect);
                    }
                    else
                    {
                        statusEffectManager.ApplyStatusEffectToEnemy(currentTargetId, statusEffect);
                    }

                    if (appliedEffectCategory != null)
                    {
                        switch (appliedEffectCategory)
                        {
                            case StatusEffectCategory.Poison:
                                statusMessage = BattleMessage.PoisonSuffix;
                                break;
                            case StatusEffectCategory.Paralysis:
                                statusMessage = BattleMessage.ParalysisSuffix;
                                break;
                            case StatusEffectCategory.Sleep:
                                statusMessage = BattleMessage.SleepSuffix;
                                break;
                            case StatusEffectCategory.Confusion:
                                statusMessage = BattleMessage.ConfusionSuffix;
                                break;
                        }
                    }
                }

                string targetName = _actionProcessor.GetCharacterName(currentTargetId, isTargetFriend);
                _actionProcessor.SetPauseMessage(true); // 💡 メッセージポーズ開始
                _messageWindowController.GenerateStatusAilmentMessage(targetName, statusMessage);
                _battleManager.OnUpdateStatus();
                while (_actionProcessor.IsPausedMessage) yield return null; // 💡 メッセージ完了まで待機
            }
            _battleSpriteController.PlayEffectAtEnemy(currentTargetId, skillData.effectSprite);
            switch (skillData.skillId)
            {
                case 0:
                    SoundManager.Instance.PlaySE(6, 0.5f);
                    break;
                case 1:
                    SoundManager.Instance.PlaySE(7, 0.5f);
                    break;
                case 2:
                    SoundManager.Instance.PlaySE(8, 0.5f);
                    break;
                case 3:
                    SoundManager.Instance.PlaySE(9, 0.5f);
                    break;
                case 4:
                    SoundManager.Instance.PlaySE(10, 0.5f);
                    break;
                case 5:
                    SoundManager.Instance.PlaySE(11, 0.5f);
                    break;
                case 6:
                    SoundManager.Instance.PlaySE(12, 0.5f);
                    break;
                case 7:
                    SoundManager.Instance.PlaySE(13, 0.5f);
                    break;
                case 8:
                    SoundManager.Instance.PlaySE(22, 0.5f);
                    break;
                case 9:
                    SoundManager.Instance.PlaySE(22, 0.5f);
                    break;
                case 10:
                    SoundManager.Instance.PlaySE(22, 0.5f);
                    break;
                case 11:
                    SoundManager.Instance.PlaySE(22, 0.5f);
                    break;
                case 12:
                    SoundManager.Instance.PlaySE(22, 0.5f);
                    break;
                case 13:
                    SoundManager.Instance.PlaySE(27, 0.5f);
                    break;
                case 14:
                    SoundManager.Instance.PlaySE(28, 0.5f);
                    break;
                case 15:
                    SoundManager.Instance.PlaySE(29, 0.5f);
                    break;
                case 16:
                    SoundManager.Instance.PlaySE(47, 0.5f);
                    break;
                case 17:
                    SoundManager.Instance.PlaySE(46, 0.5f);
                    break;
                case 18:
                    SoundManager.Instance.PlaySE(48, 0.5f);
                    break;
                case 19:
                    SoundManager.Instance.PlaySE(25, 0.5f);
                    break;
                case 20:
                    SoundManager.Instance.PlaySE(25, 0.5f);
                    break;
                case 21:
                    SoundManager.Instance.PlaySE(48, 0.5f);
                    break;
                case 22:
                    SoundManager.Instance.PlaySE(48, 0.5f);
                    break;
                case 23:
                    SoundManager.Instance.PlaySE(48, 0.5f);
                    break;
                case 24:
                    SoundManager.Instance.PlaySE(48, 0.5f);
                    break;
                case 25:
                    SoundManager.Instance.PlaySE(48, 0.5f);
                    break;
                case 26:
                    SoundManager.Instance.PlaySE(48, 0.5f);
                    break;
                case 27:
                    SoundManager.Instance.PlaySE(27, 0.5f);
                    break;
                case 28:
                    SoundManager.Instance.PlaySE(44, 0.5f);
                    break;
                case 29:
                    SoundManager.Instance.PlaySE(45, 0.5f);
                    break;
                case 30:
                    SoundManager.Instance.PlaySE(29, 0.5f);
                    break;
                case 31:
                    SoundManager.Instance.PlaySE(28, 0.5f);
                    break;
                case 32:
                    SoundManager.Instance.PlaySE(46, 0.5f);
                    break;
                case 33:
                    SoundManager.Instance.PlaySE(45, 0.5f);
                    break;
                case 34:
                    SoundManager.Instance.PlaySE(47, 0.5f);
                    break;
                case 35:
                    SoundManager.Instance.PlaySE(45, 0.5f);
                    break;
                case 36:
                    SoundManager.Instance.PlaySE(47, 0.5f);
                    break;
                case 37:
                    SoundManager.Instance.PlaySE(48, 0.5f);
                    break;
                case 38:
                    SoundManager.Instance.PlaySE(48, 0.5f);
                    break;
                case 39:
                    SoundManager.Instance.PlaySE(48, 0.5f);
                    break;
                case 40:
                    SoundManager.Instance.PlaySE(14, 0.5f);
                    break;
                case 41:
                    SoundManager.Instance.PlaySE(15, 0.5f);
                    break;
                case 42:
                    SoundManager.Instance.PlaySE(16, 0.5f);
                    break;
                case 43:
                    SoundManager.Instance.PlaySE(17, 0.5f);
                    break;
                case 44:
                    SoundManager.Instance.PlaySE(18, 0.5f);
                    break;
                case 45:
                    SoundManager.Instance.PlaySE(19, 0.5f);
                    break;
                case 46:
                    SoundManager.Instance.PlaySE(20, 0.5f);
                    break;
                case 47:
                    SoundManager.Instance.PlaySE(45, 0.5f);
                    break;
                case 48:
                    SoundManager.Instance.PlaySE(45, 0.5f);
                    break;
                case 49:
                    SoundManager.Instance.PlaySE(45, 0.5f);
                    break;
                case 50:
                    SoundManager.Instance.PlaySE(47, 0.5f);
                    break;
                case 51:
                    SoundManager.Instance.PlaySE(47, 0.5f);
                    break;
                case 52:
                    SoundManager.Instance.PlaySE(46, 0.5f);
                    break;
                case 53:
                    SoundManager.Instance.PlaySE(48, 0.5f);
                    break;
                case 54:
                    SoundManager.Instance.PlaySE(22, 0.5f);
                    break;
                case 901:
                    SoundManager.Instance.PlaySE(21, 0.5f);
                    break;
                case 902:
                    SoundManager.Instance.PlaySE(27, 0.5f);
                    break;
                case 903:
                    SoundManager.Instance.PlaySE(21, 0.5f);
                    break;
                case 904:
                    SoundManager.Instance.PlaySE(46, 0.5f);
                    break;
                case 905:
                    SoundManager.Instance.PlaySE(48, 0.5f);
                    break;
                case 906:
                    SoundManager.Instance.PlaySE(22, 0.5f);
                    break;
                case 907:
                    SoundManager.Instance.PlaySE(22, 0.5f);
                    break;
                case 908:
                    SoundManager.Instance.PlaySE(21, 0.5f);
                    break;
                case 909:
                    SoundManager.Instance.PlaySE(21, 0.5f);
                    break;
                case 910:
                    SoundManager.Instance.PlaySE(21, 0.5f);
                    break;
                case 911:
                    SoundManager.Instance.PlaySE(21, 0.5f);
                    break;
                case 912:
                    SoundManager.Instance.PlaySE(22, 0.5f);
                    break;
            }
            // 修正: ターゲットの処理が終わったら、次のターゲットに進む前にユーザー入力待ちを挟む
            if (!_battleManager.IsBattleFinished)
            {
                yield return StartCoroutine(WaitForUserInput());
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
        // _enemyStatusManager = _battleManager.GetEnemyStatusManager();
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
    bool IsBuffTargetFriend(Buff buff)
    {
        return buff.effectTarget == EffectTarget.Own
            || buff.effectTarget == EffectTarget.FriendSolo
            || buff.effectTarget == EffectTarget.FriendAll;
    }
}
