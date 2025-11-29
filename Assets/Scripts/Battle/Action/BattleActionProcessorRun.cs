using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BattleActionProcessorRun : MonoBehaviour, IBattleActionProcessor
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

    /// <summary>
    /// 参照をセットします。
    /// </summary>
    public void SetReferences(BattleManager battleManager, BattleActionProcessor actionProcessor)
    {
        _battleManager = battleManager;
        _actionProcessor = actionProcessor;
        _messageWindowController = _battleManager.GetWindowManager().GetMessageWindowController();
        _battleSpriteController = _battleManager.GetBattleSpriteController();
    }

    /// <summary>
    /// 逃走のアクションを処理します。
    /// </summary>
    public void ProcessAction(BattleAction action)
    {
        var actorStatus = _actionProcessor.GetCharacterParameter(action.actorId, action.isActorFriend);

        _actionProcessor.SetPauseProcess(true);

        // 逃走が成功したかどうかを判定します。
        bool isRunSuccess = true;
        foreach (var targetId in action.targetIds)
        {
            var targetStatus = _actionProcessor.GetCharacterParameter(targetId, action.isTargetFriend);
            // 敵それぞれに逃げる判定を行い、１体でも逃げられなかったら闘争失敗
            if (!DamageFormula.CalculateCanRun(actorStatus.Speed, targetStatus.Speed))
            {
                isRunSuccess = false;
                break;
            }
        }
        StartCoroutine(ShowRunMessage(action, isRunSuccess));
    }

    /// <summary>
    /// 逃走のメッセージを表示します。
    /// </summary>
    IEnumerator ShowRunMessage(BattleAction action, bool isSuccess)
    {
        string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);

        _actionProcessor.SetPauseMessage(true);
        _messageWindowController.GenerateRunMessage(actorName);
        while (_actionProcessor.IsPausedMessage)
        {
            yield return null;
        }

        Logger.Instance.Log($"キャラクターの逃走判定 : {isSuccess}");

        if (isSuccess)
        {
            if (action.isActorFriend)
            {
                _battleManager.OnRunaway();
            }
            else
            {
                _battleSpriteController.HideEnemy();
                EnemyStatusManager.Instance.OnRunEnemy(action.actorId);
                _battleManager.OnEnemyRunaway();
            }
        }
        else
        {
            _actionProcessor.SetPauseMessage(true);
            _messageWindowController.GenerateRunFailedMessage();
            while (_actionProcessor.IsPausedMessage)
            {
                yield return null;
            }
            _actionProcessor.SetPauseProcess(false);
        }
    }
}
