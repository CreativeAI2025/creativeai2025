using UnityEngine;
using System.Collections;
public class BattleActionProcessorItem : MonoBehaviour
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
    /// 参照をセットします。
    /// </summary>
    public void SetReferences(BattleManager battleManager, BattleActionProcessor actionProcessor)
    {
        _battleManager = battleManager;
        _actionProcessor = actionProcessor;
        _messageWindowController = _battleManager.GetWindowManager().GetMessageWindowController();
    }

    /// <summary>
    /// アイテムのアクションを処理します。
    /// </summary>
    public void ProcessAction(BattleAction action)
    {
        var itemData = ItemDataManager.Instance.GetItemDataById(action.itemId);
        if (itemData == null)
        {
            Logger.Instance.LogWarning($"アイテムデータが見つかりませんでした。 ID: {action.itemId}");
            return;
        }

        // 消費アイテムの場合、所持数を減らします。
        if (action.isActorFriend && itemData.itemCategory == ItemCategory.ConsumableItem)
        {
            CharacterStatusManager.Instance.UseItem(action.itemId);
        }

        _actionProcessor.SetPauseProcess(true);
        if (itemData.itemEffect.itemEffectCategory == ItemEffectCategory.HPRecovery)
        {
            int hpDelta = DamageFormula.CalculateHealValue(itemData.itemEffect.value);
            int mpDelta = 0;

            if (action.isActorFriend)
            {
                CharacterStatusManager.Instance.ChangeCharacterStatus(action.targetId, hpDelta, mpDelta);
            }
            else
            {
                EnemyStatusManager.Instance.ChangeEnemyStatus(action.targetId, hpDelta, mpDelta);
            }

            StartCoroutine(ShowItemHealMessage(action, itemData.itemName, hpDelta));
        }
        else if (itemData.itemEffect.itemEffectCategory == ItemEffectCategory.MPRecovery)
        {
            int hpDelta = 0;
            int mpDelta = DamageFormula.CalculateHealValue(itemData.itemEffect.value); // MP回復用の計算

            if (action.isActorFriend)
            {
                CharacterStatusManager.Instance.ChangeCharacterStatus(action.targetId, hpDelta, mpDelta);
            }
            else
            {
                EnemyStatusManager.Instance.ChangeEnemyStatus(action.targetId, hpDelta, mpDelta);
            }

            StartCoroutine(ShowItemMpHealMessage(action, itemData.itemName, mpDelta));
        }
        else if (itemData.itemEffect.itemEffectCategory == ItemEffectCategory.Revive)
        {
            // 蘇生時の回復HP（例: アイテム効果値の50%回復など）
            int reviveHp = DamageFormula.CalculateHealValue(itemData.itemEffect.value);
            int mpDelta = 0;

            if (action.isActorFriend)
            {
                var status = CharacterStatusManager.Instance.GetCharacterStatusById(action.targetId);
                if (status != null && status.isDefeated)
                {
                    status.isDefeated = false; // 蘇生！
                    CharacterStatusManager.Instance.ChangeCharacterStatus(action.targetId, reviveHp, mpDelta);
                }
            }
            else
            {
                var status = EnemyStatusManager.Instance.GetEnemyStatusByBattleId(action.targetId);
                if (status != null && status.isDefeated)
                {
                    status.isDefeated = false; // 蘇生！
                    EnemyStatusManager.Instance.ChangeEnemyStatus(action.targetId, reviveHp, mpDelta);
                }
            }

            StartCoroutine(ShowItemReviveMessage(action, itemData.itemName, reviveHp));

        }
        {
            Debug.LogWarning($"未定義のアイテム効果です。 ID: {itemData.itemId}");
        }
    }

    /// <summary>
    /// HP回復アイテムのメッセージを表示します。
    /// </summary>
    IEnumerator ShowItemHealMessage(BattleAction action, string itemName, int healValue)
    {
        string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
        string targetName = _actionProcessor.GetCharacterName(action.targetId, action.isTargetFriend);

        _actionProcessor.SetPauseMessage(true);
        _messageWindowController.GenerateUseItemMessage(actorName, itemName);
        while (_actionProcessor.IsPausedMessage)
        {
            yield return null;
        }

        _actionProcessor.SetPauseMessage(true);
        _messageWindowController.GenerateHpHealMessage(targetName, healValue);
        _battleManager.OnUpdateStatus();
        while (_actionProcessor.IsPausedMessage)
        {
            yield return null;
        }

        _actionProcessor.SetPauseProcess(false);
    }
    /// <summary>
    /// MP回復アイテムのメッセージを表示します。
    /// </summary>
    IEnumerator ShowItemMpHealMessage(BattleAction action, string itemName, int healValue)
    {
        string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
        string targetName = _actionProcessor.GetCharacterName(action.targetId, action.isTargetFriend);

        _actionProcessor.SetPauseMessage(true);
        _messageWindowController.GenerateUseItemMessage(actorName, itemName);
        while (_actionProcessor.IsPausedMessage)
        {
            yield return null;
        }

        _actionProcessor.SetPauseMessage(true);
        _messageWindowController.GenerateMpHealMessage(targetName, healValue);
        _battleManager.OnUpdateStatus();
        while (_actionProcessor.IsPausedMessage)
        {
            yield return null;
        }

        _actionProcessor.SetPauseProcess(false);
    }
    /// <summary>
    /// 蘇生アイテムのメッセージを表示します。
    /// </summary>
    IEnumerator ShowItemReviveMessage(BattleAction action, string itemName, int reviveHp)
    {
        string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
        string targetName = _actionProcessor.GetCharacterName(action.targetId, action.isTargetFriend);

        // アイテム使用メッセージ
        _actionProcessor.SetPauseMessage(true);
        _messageWindowController.GenerateUseItemMessage(actorName, itemName);
        while (_actionProcessor.IsPausedMessage) yield return null;

        // 蘇生メッセージ（＋回復量表示）
        _actionProcessor.SetPauseMessage(true);
        _messageWindowController.GenerateReviveMessage(targetName, reviveHp);
        _battleManager.OnUpdateStatus();
        while (_actionProcessor.IsPausedMessage) yield return null;

        _actionProcessor.SetPauseProcess(false);
    }
}