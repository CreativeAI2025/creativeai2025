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
        StartCoroutine(ProcessItemActionCoroutine(action));
    }

    // 💡 ProcessAction から呼ばれるコルーチンのラッパーを定義
    IEnumerator ProcessItemActionCoroutine(BattleAction action)
    {
        var itemData = ItemDataManager.Instance.GetItemDataById(action.itemId);
        if (itemData == null)
        {
            Logger.Instance.LogWarning($"アイテムデータが見つかりませんでした。 ID: {action.itemId}");
            _actionProcessor.SetPauseProcess(false);
            yield break;
        }

        // 消費アイテムの場合、所持数を減らします。（ループ前に一度だけ実行）
        if (action.isActorFriend && itemData.itemCategory == ItemCategory.ConsumableItem)
        {
            CharacterStatusManager.Instance.UseItem(action.itemId);
        }

        string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);

        // ----------------------------------------------------
        // 💡 アイテム使用メッセージを一度だけ表示
        // ----------------------------------------------------
        _actionProcessor.SetPauseMessage(true);
        _messageWindowController.GenerateUseItemMessage(actorName, itemData.itemName);
        while (_actionProcessor.IsPausedMessage) yield return null;

        // ----------------------------------------------------
        // 💡 複数ターゲットへの効果適用ループ
        // ----------------------------------------------------
        foreach (var targetId in action.targetIds)
        {
            // ... (既存のアイテム効果処理をループ内に配置) ...

            if (itemData.itemEffect.itemEffectCategory == ItemEffectCategory.HPRecovery)
            {
                int hpDelta = DamageFormula.CalculateHealValue(itemData.itemEffect.value);
                int mpDelta = 0;

                // ステータス変更
                if (action.isTargetFriend)
                {
                    CharacterStatusManager.Instance.ChangeCharacterStatus(targetId, hpDelta, mpDelta);
                }
                else
                {
                    EnemyStatusManager.Instance.ChangeEnemyStatus(targetId, hpDelta, mpDelta);
                }

                // メッセージ表示コルーチンを呼び出し
                yield return StartCoroutine(ShowItemHealMessage(targetId, hpDelta, action.isTargetFriend));
            }
            else if (itemData.itemEffect.itemEffectCategory == ItemEffectCategory.MPRecovery)
            {
                int mpDelta = DamageFormula.CalculateHealValue(itemData.itemEffect.value);
                int mpConsume = 0;

                // ステータス変更
                if (action.isTargetFriend)
                {
                    CharacterStatusManager.Instance.ChangeCharacterStatus(targetId, mpDelta, mpConsume);
                }
                else
                {
                    EnemyStatusManager.Instance.ChangeEnemyStatus(targetId, mpDelta, mpConsume);
                }

                // メッセージ表示コルーチンを呼び出し
                yield return StartCoroutine(ShowItemMpHealMessage(targetId, mpDelta, action.isTargetFriend));
            }
            // ... (他の効果も同様に ShowItem...Message を呼ぶ) ...

        } // 💡 foreach (var targetId in action.targetIds) の終了

        // 💡 修正: ループがすべて完了した後、戦闘が終了していない場合にのみプロセスを再開
        if (!_battleManager.IsBattleFinished)
        {
            _actionProcessor.SetPauseProcess(false);
        }
    }

    /// <summary>
    /// HP回復アイテムのメッセージを表示します。（ターゲットごとのメッセージ表示）
    /// </summary>
    IEnumerator ShowItemHealMessage(int targetId, int healValue, bool isTargetFriend)
    {
        string targetName = _actionProcessor.GetCharacterName(targetId, isTargetFriend);

        // 💡 メッセージ表示（UseItemMessageはループ前に移動したため削除）

        _actionProcessor.SetPauseMessage(true);
        _messageWindowController.GenerateHpHealMessage(targetName, healValue);
        _battleManager.OnUpdateStatus();
        while (_actionProcessor.IsPausedMessage)
        {
            yield return null;
        }
    }

    /// <summary>
    /// MP回復アイテムのメッセージを表示します。
    /// </summary>
    IEnumerator ShowItemMpHealMessage(int targetId, int healValue, bool isTargetFriend)
    {
        string targetName = _actionProcessor.GetCharacterName(targetId, isTargetFriend);

        // 💡 メッセージ表示（UseItemMessageはループ前に移動したため削除）

        _actionProcessor.SetPauseMessage(true);
        _messageWindowController.GenerateMpHealMessage(targetName, healValue);
        _battleManager.OnUpdateStatus();
        while (_actionProcessor.IsPausedMessage)
        {
            yield return null;
        }
    }
    /// <summary>
    /// 蘇生アイテムのメッセージを表示します。
    /// </summary>
    IEnumerator ShowItemReviveMessage(BattleAction action, string itemName, int reviveHp)
    {
        string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
        foreach (var targetId in action.targetIds)
        {
            string targetName = _actionProcessor.GetCharacterName(targetId, action.isTargetFriend);

            // アイテム使用メッセージ
            _actionProcessor.SetPauseMessage(true);
            _messageWindowController.GenerateUseItemMessage(actorName, itemName);
            while (_actionProcessor.IsPausedMessage) yield return null;

            // 蘇生メッセージ（＋回復量表示）
            _actionProcessor.SetPauseMessage(true);
            _messageWindowController.GenerateReviveMessage(targetName, reviveHp);
            _battleManager.OnUpdateStatus();
            while (_actionProcessor.IsPausedMessage) yield return null;
        }

        _actionProcessor.SetPauseProcess(false);
    }
}