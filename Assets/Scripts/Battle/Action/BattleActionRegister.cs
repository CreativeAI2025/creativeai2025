using UnityEngine;

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
    public void SetFriendAttackAction(int actorId, int targetId)
    {
        var characterParam = GetCharacterParameterRecord(actorId);
        BattleAction action = new()
        {
            actorId = actorId,
            isActorFriend = true,
            targetId = targetId,
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
    public void SetEnemyAttackAction(int actorId, int targetId, EnemyData enemyData)
    {
        BattleAction action = new()
        {
            actorId = actorId,
            isActorFriend = false,
            targetId = targetId,
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
    /// <param name="magicId">魔法のID</param>
    public void SetFriendSkillAction(int actorId, int targetId, int magicId)
    {
        var characterParam = GetCharacterParameterRecord(actorId);
        BattleAction action = new()
        {
            actorId = actorId,
            isActorFriend = true,
            targetId = targetId,
            battleCommand = BattleCommand.Skill,
            itemId = magicId,
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
    public void SetEnemySkillAction(int actorId, int targetId, int magicId, EnemyData enemyData)
    {
        BattleAction action = new()
        {
            actorId = actorId,
            isActorFriend = false,
            targetId = targetId,
            battleCommand = BattleCommand.Skill,
            itemId = magicId,
            actorSpeed = enemyData.Speed,
        };

        _actionProcessor.RegisterAction(action);
    }

    /// <summary>
    /// アイテムコマンドのアクションをセットします。
    /// </summary>
    /// <param name="actorId">アクションを行うキャラクターのID</param>
    /// <param name="enemyBattleId">攻撃対象のキャラクターの戦闘中ID</param>
    /// <param name="itemId">アイテムのID</param>
    public void SetFriendItemAction(int actorId, int enemyBattleId, int itemId)
    {
        var characterParam = GetCharacterParameterRecord(actorId);

        var itemData = ItemDataManager.Instance.GetItemDataById(itemId);
        if (itemData == null)
        {
           Logger.Instance.LogError($"選択されたIDのアイテムは見つかりませんでした。ID : {itemId}");
            return;
        }

        int targetId = enemyBattleId;
        bool isTargetFriend = false;
        if (itemData.itemEffect.effectTarget == EffectTarget.Own
            || itemData.itemEffect.effectTarget == EffectTarget.FriendAll
            || itemData.itemEffect.effectTarget == EffectTarget.FriendSolo)
        {
            isTargetFriend = true;
            targetId = actorId;
        }

        BattleAction action = new()
        {
            actorId = actorId,
            isActorFriend = true,
            targetId = targetId,
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