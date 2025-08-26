using UnityEngine;
using System.Collections;
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
    /// 攻撃のアクションを処理します。
    /// </summary>
    public void ProcessAction(BattleAction action)
    {
        var actorParam = _actionProcessor.GetCharacterParameter(action.actorId, action.isActorFriend);
        var targetParam = _actionProcessor.GetCharacterParameter(action.targetId, action.isTargetFriend);
        int damage = DamageFormula.CalculateDamage(actorParam.Attack, targetParam.Defence);
        int hpDelta = damage * -1;
        int mpDelta = 0;
        bool isTargetDefeated = false;
        if (action.isTargetFriend)
        {
            CharacterStatusManager.Instance.ChangeCharacterStatus(action.targetId, hpDelta, mpDelta);
            isTargetDefeated = CharacterStatusManager.Instance.IsCharacterDefeated(action.targetId);
        }
        else
        {
            _enemyStatusManager.ChangeEnemyStatus(action.targetId, hpDelta, mpDelta);
            isTargetDefeated = _enemyStatusManager.IsEnemyDefeated(action.targetId);
            if (isTargetDefeated)
            {
                _enemyStatusManager.OnDefeatEnemy(action.targetId);
            }
        }

        _actionProcessor.SetPauseProcess(true);
        StartCoroutine(ShowAttackMessage(action, damage, isTargetDefeated));
    }

    /// <summary>
    /// 攻撃のメッセージを表示します。
    /// </summary>
    IEnumerator ShowAttackMessage(BattleAction action, int damage, bool isTargetDefeated)
    {
        string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
        string targetName = _actionProcessor.GetCharacterName(action.targetId, action.isTargetFriend);

        _actionProcessor.SetPauseMessage(true);
        _messageWindowController.GenerateAttackMessage(actorName);
        while (_actionProcessor.IsPausedMessage)
        {
            yield return null;
        }

        _actionProcessor.SetPauseMessage(true);
        _messageWindowController.GenerateDamageMessage(targetName, damage);
        _battleManager.OnUpdateStatus();
        while (_actionProcessor.IsPausedMessage)
        {
            yield return null;
        }

        if (!isTargetDefeated)
        {
            _actionProcessor.SetPauseProcess(false);
            yield break;
        }

        if (action.isTargetFriend)
        {
            _actionProcessor.SetPauseMessage(true);
            _messageWindowController.GenerateDefeateFriendMessage(targetName);
            while (_actionProcessor.IsPausedMessage)
            {
                yield return null;
            }

            if (CharacterStatusManager.Instance.IsAllCharacterDefeated())
            {
                _battleManager.OnGameover();
            }
        }
        else
        {
            _actionProcessor.SetPauseMessage(true);
            _battleSpriteController.HideEnemy();
            _messageWindowController.GenerateDefeateEnemyMessage(targetName);
            while (_actionProcessor.IsPausedMessage)
            {
                yield return null;
            }

            if (_enemyStatusManager.IsAllEnemyDefeated())
            {
                _battleManager.OnEnemyDefeated();
            }
        }
    }
}

