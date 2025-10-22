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
    /// 攻撃のアクションを処理します。
    /// 修正：複数のターゲットに適応
    /// </summary>
    public void ProcessAction(BattleAction action)
    {
        var actorParam = _actionProcessor.GetCharacterParameter(action.actorId, action.isActorFriend);
        foreach (var targetId in action.targetIds)
        {
            var targetParam = _actionProcessor.GetCharacterParameter(targetId, action.isTargetFriend);
            int damage = 0;
            if (action.isTargetFriend)
            {
                var characterStatus = CharacterStatusManager.Instance.GetCharacterStatusById(targetId);
                var enemyStatus = EnemyStatusManager.Instance.GetEnemyStatusByBattleId(action.actorId);

                if (characterStatus == null)
                {
                    Debug.LogWarning($"キャラクターのステータスが見つかりませんでした。 ID : {targetId}");
                }
                if (enemyStatus == null)
                {
                    Logger.Instance.LogWarning($"敵キャラクターのステータスが見つかりませんでした。 戦闘中ID : {action.actorId}");
                }
                damage = DamageFormula.CalculateDamage(actorParam.Attack, targetParam.Defence, enemyStatus.attackBuffMultiplier, characterStatus.attackBuffMultiplier);
            }

            else
            {
                var characterStatus = CharacterStatusManager.Instance.GetCharacterStatusById(action.actorId);
                var enemyStatus = EnemyStatusManager.Instance.GetEnemyStatusByBattleId(targetId);

                if (characterStatus == null)
                {
                    Debug.LogWarning($"キャラクターのステータスが見つかりませんでした。 ID : {action.actorId}");

                }
                if (enemyStatus == null)
                {
                    Logger.Instance.LogWarning($"敵キャラクターのステータスが見つかりませんでした。 戦闘中ID : {targetId}");
                }

                damage = DamageFormula.CalculateDamage(actorParam.Attack, targetParam.Defence, characterStatus.attackBuffMultiplier, enemyStatus.attackBuffMultiplier);

            }
            int hpDelta = damage * -1;
            int mpDelta = 0;
            bool isTargetDefeated = false;
            // var charaStatus = CharacterStatusManager.Instance.GetCharacterStatusById(action.actorId);
            // if (charaStatus != null && charaStatus.Confusion && Random.value < 0.5f)
            // {
            //     action.targetId = action.actorId;
            // }
            // var enemyStatus = _enemyStatusManager.GetEnemyStatusByBattleId(action.actorId);
            // if (enemyStatus != null && enemyStatus.Confusion && Random.value < 0.5f)
            // {
            //     action.targetId = action.actorId;

            // }
            if (action.isTargetFriend)
            {
                CharacterStatusManager.Instance.ChangeCharacterStatus(targetId, hpDelta, mpDelta);
                isTargetDefeated = CharacterStatusManager.Instance.IsCharacterDefeated(targetId);
            }
            else
            {
                EnemyStatusManager.Instance.ChangeEnemyStatus(targetId, hpDelta, mpDelta);
                isTargetDefeated = EnemyStatusManager.Instance.IsEnemyDefeated(targetId);
                if (isTargetDefeated)
                {
                    EnemyStatusManager.Instance.OnDefeatEnemy(targetId);
                }
            }

            _actionProcessor.SetPauseProcess(true);
            StartCoroutine(ShowAttackMessage(action, damage, isTargetDefeated));
        }

    }

    /// <summary>
    /// 攻撃のメッセージを表示します。
    /// </summary>
    IEnumerator ShowAttackMessage(BattleAction action, int damage, bool isTargetDefeated)
    {
        string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);

        foreach (var targetId in action.targetIds)
        {
            string targetName = _actionProcessor.GetCharacterName(targetId, action.isTargetFriend);
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

                if (EnemyStatusManager.Instance.IsAllEnemyDefeated())
                {
                    _battleManager.OnEnemyDefeated();
                }
            }
        }

    }
}

