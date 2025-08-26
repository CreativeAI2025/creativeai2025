using UnityEngine;
using System.Collections;
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
    /// 戦闘中の敵キャラクターの管理を行うクラスへの参照です。
    /// </summary>
    EnemyStatusManager _enemyStatusManager;

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
        _enemyStatusManager = _battleManager.GetEnemyStatusManager();
        _battleSpriteController = _battleManager.GetBattleSpriteController();
    }

    /// <summary>
    /// 逃走のアクションを処理します。
    /// </summary>
    public void ProcessAction(BattleAction action)
    {
        var actorStatus = _actionProcessor.GetCharacterParameter(action.actorId, action.isActorFriend);
        var targetStatus = _actionProcessor.GetCharacterParameter(action.targetId, action.isTargetFriend);

        _actionProcessor.SetPauseProcess(true);

        // 逃走が成功したかどうかを判定します。
        bool isRunSuccess = DamageFormula.CalculateCanRun(actorStatus.Speed, targetStatus.Speed);
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
                _enemyStatusManager.OnRunEnemy(action.actorId);
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
