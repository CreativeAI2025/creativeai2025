using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 戦闘中のアクションを登録するクラスです。
/// </summary>
public class BattleActionRegister : MonoBehaviour
{
    /// <summary>
    /// 戦闘中のアクションを処理するクラスへの参照です。
    /// </summary>
    BattleActionProcessor _actionProcessor;

    /// <summary>
    /// このクラスを初期化します。
    /// </summary>
    /// <param name="actionProcessor">戦闘中のアクションを処理するクラスへの参照</param>
    public void InitializeRegister(BattleActionProcessor actionProcessor)
    {
        _actionProcessor = actionProcessor;
    }

    /// <summary>
    /// キャラクターのパラメータレコードを取得します。
    /// </summary>
    /// <param name="characterId">キャラクターのID</param>
    public ParameterRecord GetCharacterParameterRecord(int characterId)
    {
        var characterStatus = CharacterStatusManager.Instance.GetCharacterStatusById(characterId);
        var parameterTable = CharacterDataManager.Instance.GetParameterTable(characterId);
        var parameterRecord = parameterTable.parameterRecords.Find(p => p.Level == characterStatus.level);
        return parameterRecord;
    }

    /// <summary>
    /// 攻撃コマンドのアクションをセットします。
    /// </summary>
    /// <param name="actorId">アクションを行うキャラクターのID</param>
    /// <param name="targetId">攻撃対象のキャラクターのID</param>
    public void SetFriendAttackAction(int actorId, List<int> targetIds)
    {
        var characterParam = GetCharacterParameterRecord(actorId);
        BattleAction action = new()
        {
            actorId = actorId,
            isActorFriend = true,
            targetIds = targetIds,
            isTargetFriend = false,
            battleCommand = BattleCommand.Attack,
            actorSpeed = characterParam.Speed,
        };

        _actionProcessor.RegisterAction(action);
    }

    /// <summary>
    /// 敵キャラクターの攻撃コマンドのアクションをセットします。
    /// </summary>
    /// <param name="actorId">アクションを行う敵キャラクターの戦闘中ID</param>
    /// <param name="targetId">攻撃対象のキャラクターの戦闘中ID</param>
    /// <param name="enemyData">敵キャラクターのデータ</param>
    public void SetEnemyAttackAction(int actorId, List<int> targetIds, EnemyData enemyData)
    {
        BattleAction action = new()
        {
            actorId = actorId,
            isActorFriend = false,
            targetIds = targetIds,
            isTargetFriend = true,
            battleCommand = BattleCommand.Attack,
            actorSpeed = enemyData.Speed,
        };

        _actionProcessor.RegisterAction(action);
    }

    /// <summary>
    /// 魔法コマンドのアクションをセットします。
    /// </summary>
    /// <param name="actorId">アクションを行うキャラクターのID</param>
    /// <param name="targetId">攻撃対象のキャラクターのID</param>
    /// <param name="skillId">魔法のID</param>
    public void SetFriendSkillAction(int actorId, List<int> targetIds, bool isTargetFriend, int skillId)
    {
        var characterParam = GetCharacterParameterRecord(actorId);
        BattleAction action = new()
        {
            actorId = actorId,
            isActorFriend = true,
            targetIds = targetIds,
            isTargetFriend = isTargetFriend,
            battleCommand = BattleCommand.Skill,
            itemId = skillId,
            actorSpeed = characterParam.Speed,
        };
        _actionProcessor.RegisterAction(action);
    }

    /// <summary>
    /// 敵キャラクターの魔法コマンドのアクションをセットします。
    /// </summary>
    /// <param name="actorId">アクションを行う敵キャラクターの戦闘中ID</param>
    /// <param name="targetId">攻撃対象のキャラクターの戦闘中ID</param>
    /// <param name="magicId">魔法のID</param>
    /// <param name="enemyData">敵キャラクターのデータ</param>
    public void SetEnemySkillAction(int actorId, List<int> targetIds, int magicId, EnemyData enemyData)
    {
        BattleAction action = new()
        {
            actorId = actorId,
            isActorFriend = false,
            targetIds = targetIds,
            battleCommand = BattleCommand.Skill,
            itemId = magicId,
            actorSpeed = enemyData.Speed,
        };

        _actionProcessor.RegisterAction(action);
    }

    /// <summary>
    /// アイテムコマンドのアクションをセットします。
    /// <summary>
    /// アイテムコマンドのアクションをセットします。
    /// 💡 修正: 引数を List<int> targetIds と bool isTargetFriend に変更
    /// </summary>
    /// <param name="actorId">アクションを行うキャラクターのID</param>
    /// <param name="targetIds">攻撃対象のキャラクターIDまたは戦闘中IDのリスト</param>
    /// <param name="isTargetFriend">ターゲットが味方かどうかのフラグ</param>
    /// <param name="itemId">アイテムのID</param>
    public void SetFriendItemAction(int actorId, List<int> targetIds, bool isTargetFriend, int itemId)
    {
        var characterParam = GetCharacterParameterRecord(actorId);

        var itemData = ItemDataManager.Instance.GetItemDataById(itemId);
        if (itemData == null)
        {
            Logger.Instance.LogError($"選択されたIDのアイテムは見つかりませんでした。ID : {itemId}");
            return;
        }

        // 💡 修正点: UI（BattleManager）から渡されたターゲットリストと属性をそのままアクションに登録します。
        // 以前のロジック（EffectTargetをチェックしてtargetId/isTargetFriendを計算する部分）は、
        // ターゲット選択ウィンドウ側で処理されるため、ここで削除されます。

        BattleAction action = new()
        {
            actorId = actorId,
            isActorFriend = true,
            targetIds = targetIds, // 💡 List<int>を代入
            isTargetFriend = isTargetFriend,
            battleCommand = BattleCommand.Item,
            itemId = itemId,
            actorSpeed = characterParam.Speed,
        };

        _actionProcessor.RegisterAction(action);
    }

    /// <summary>
    /// 逃げるコマンドのアクションをセットします。
    /// </summary>
    /// <param name="actorId">アクションを行うキャラクターのID</param>
    public void SetFriendRunAction(int actorId)
    {
        var characterParam = GetCharacterParameterRecord(actorId);
        BattleAction action = new()
        {
            actorId = actorId,
            isActorFriend = true,
            battleCommand = BattleCommand.Run,
            actorSpeed = characterParam.Speed,
        };

        _actionProcessor.RegisterAction(action);
    }
}