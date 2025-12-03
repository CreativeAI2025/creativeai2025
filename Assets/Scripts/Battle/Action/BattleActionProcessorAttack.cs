using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class BattleActionProcessorAttack : MonoBehaviour
{
    /// <summary>
    /// 戦闘中のアクションを処理するクラスへの参照です。
    /// </summary>
    BattleActionProcessor _actionProcessor;

    /// <summary>
    /// 戦闘に関する機能を管理するクラスへの参照です。
    /// </summary>
    BattleManager _battleManager;
   

    /// <summary>
    /// メッセージウィンドウを制御するクラスへの参照です。
    /// </summary>
    MessageWindowController _messageWindowController;

    /// <summary>
    /// 戦闘関連のスプライトを制御するクラスへの参照です。
    /// </summary>
    BattleSpriteController _battleSpriteController;
    SkillAnimationManager _skillAnimationManager;

    /// <summary>
    /// 参照をセットします。
    /// </summary>
    public void SetReferences(BattleManager battleManager, BattleActionProcessor actionProcessor)
    {
        _battleManager = battleManager;
        _actionProcessor = actionProcessor;
        _messageWindowController = _battleManager.GetWindowManager().GetMessageWindowController();
        _battleSpriteController = _battleManager.GetBattleSpriteController();
        _skillAnimationManager = _battleManager.GetSkillAnimationManager();
    }

    /// <summary>
    /// 攻撃のアクションを処理します。
    /// 修正：複数のターゲットに適応
    /// </summary>
    public void ProcessAction(BattleAction action)
    {
        _actionProcessor.SetPauseProcess(true);
        StartCoroutine(ProcessAttackActionCoroutine(action));
    }

    /// <summary>
    /// 攻撃のアクションをコルーチンで処理します。
    /// </summary>
    IEnumerator ProcessAttackActionCoroutine(BattleAction action)
    {
        var actorParam = _actionProcessor.GetCharacterParameter(action.actorId, action.isActorFriend);
        List<int> validTargetIds = _actionProcessor.GetValidTargets(action.targetIds, action.isTargetFriend);

        if (!validTargetIds.Any())
        {
            Logger.Instance.Log($"アクションの実行前にターゲットがいなくなったため、{action.battleCommand}をキャンセルします。");
            _actionProcessor.SetPauseProcess(false);
            yield break; // ターゲットがいなければキャンセルし、次の行動へ
        }

        // 💡 攻撃メッセージをターゲットごとに表示するため、ループの外側でメッセージポーズを設定
        _actionProcessor.SetPauseMessage(true);
        string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
        _messageWindowController.GenerateAttackMessage(actorName);
        while (_actionProcessor.IsPausedMessage) { yield return null; }

        foreach (var targetId in validTargetIds)
        {
            var targetParam = _actionProcessor.GetCharacterParameter(targetId, action.isTargetFriend);

            // ... (ダメージ計算ロジック。ここでは簡略化) ...

            // ターゲットのバフ・デバフ倍率を取得
            float actorAttackBuff = action.isActorFriend
                ? CharacterStatusManager.Instance.GetCharacterStatusById(action.actorId)?.attackBuffMultiplier ?? 1.0f
                : EnemyStatusManager.Instance.GetEnemyStatusByBattleId(action.actorId)?.attackBuffMultiplier ?? 1.0f;
            float targetDefenceBuff = action.isTargetFriend
                ? CharacterStatusManager.Instance.GetCharacterStatusById(targetId)?.defenceBuffMultiplier ?? 1.0f
                : EnemyStatusManager.Instance.GetEnemyStatusByBattleId(targetId)?.defenceBuffMultiplier ?? 1.0f;

            int damage = DamageFormula.CalculateDamage(actorParam.Attack, targetParam.Defence, actorAttackBuff, targetDefenceBuff);

            int hpDelta = damage * -1;
            int mpDelta = 0;
            bool isCurrentTargetDefeated = false;

            // ステータス変更
            if (action.isTargetFriend)
            {
                CharacterStatusManager.Instance.ChangeCharacterStatus(targetId, hpDelta, mpDelta);
                isCurrentTargetDefeated = CharacterStatusManager.Instance.IsCharacterDefeated(targetId);
            }
            else
            {
                EnemyStatusManager.Instance.ChangeEnemyStatus(targetId, hpDelta, mpDelta);
                isCurrentTargetDefeated = EnemyStatusManager.Instance.IsEnemyDefeated(targetId);
                if (isCurrentTargetDefeated)
                {
                    EnemyStatusManager.Instance.OnDefeatEnemy(targetId);
                }
            }
        //   var skillData = SkillDataManager.Instance.GetSkillDataById(999);
           _skillAnimationManager.PlayEffectAtEnemy(targetId, 102, action.isTargetFriend);

            // SkillEfectChange.Instance.PlaySkillAnimation(105);
            SoundManager.Instance.PlaySE(21, 0.5f);
            // ダメージメッセージ表示
            _actionProcessor.SetPauseMessage(true);
            string targetName = _actionProcessor.GetCharacterName(targetId, action.isTargetFriend);
            _messageWindowController.GenerateDamageMessage(targetName, damage);
            _battleManager.OnUpdateStatus();
            while (_actionProcessor.IsPausedMessage) { yield return null; }

            // 撃破メッセージ表示
            if (isCurrentTargetDefeated)
            {
                _actionProcessor.SetPauseMessage(true);
                if (action.isTargetFriend)
                {
                    _messageWindowController.GenerateDefeateFriendMessage(targetName);
                }
                else
                {
                    // 💡 修正: 倒れた敵のスプライトを更新（HideEnemyを置き換え）
                    _battleSpriteController.RefreshActiveEnemies();
                    _messageWindowController.GenerateDefeateEnemyMessage(targetName);
                }
                while (_actionProcessor.IsPausedMessage) { yield return null; }

                // 勝利/ゲームオーバー判定
                if (EnemyStatusManager.Instance.IsAllEnemyDefeated())
                {
                    _battleManager.OnEnemyDefeated();
                    yield break; // 戦闘終了
                }
                if (CharacterStatusManager.Instance.IsAllCharacterDefeated())
                {
                    _battleManager.OnGameover();
                    yield break; // ゲームオーバー
                }
            }

            // アクション処理の一時停止は、ループ完了後に行うため、ここでは何もしない。
        }

        // 💡 追記: 複数のターゲット処理がすべて完了したため、待機フラグを解除して次のアクションへ移行
        _actionProcessor.SetPauseProcess(false);
    }
}

