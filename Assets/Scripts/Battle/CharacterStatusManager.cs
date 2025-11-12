using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクターのステータスを管理するクラスです。
/// </summary>
public class CharacterStatusManager : DontDestroySingleton<CharacterStatusManager>
{
    /// <summary>
    /// パーティ内にいるキャラクターのIDのリストです。
    /// </summary>
    public List<int> partyCharacter = new();

    /// <summary>
    /// キャラクターのステータスのリストです。
    /// </summary>
    public List<CharacterStatus> characterStatuses = new();

    /// <summary>
    /// プレイヤーの所持金です。
    /// </summary>
    public int partyGold;

    /// <summary>
    /// プレイヤーの所持アイテムのリストです。
    /// </summary>
    public List<PartyItemInfo> partyItemInfoList = new();

    public override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// パーティメンバーの初期化
    /// 最初はゾフィ一人なので、partyCharacterのリストに、ゾフィIDである「１」を入れる。
    /// </summary>
    public void Initialize()
    {
        partyCharacter = new List<int>() { 1 };
        // デバッグ用に適当な値をぶち込んでいます
        characterStatuses = new List<CharacterStatus>()
        {
            SetCharacterStatus(1, 1)
        };
        partyGold = 1000;
        partyItemInfoList = new();
    }

    /// <summary>
    /// CharacterStatusを返す
    /// 新しくキャラクター生成を行った際に使用する
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private CharacterStatus SetCharacterStatus(int id, int level)
    {
        var characterParameterTable = CharacterDataManager.Instance.GetParameterTable(id);  // キャラクターのレベルごとのパラメーターテーブルを取得する
        var characterParameterRecord = characterParameterTable.parameterRecords[level - 1]; // キャラクターのレベルに応じたパラメーターを取得
        CharacterStatus characterStatus = new()
        {
            characterId = id,
            level = characterParameterRecord.Level,
            exp = 0,
            currentHp = characterParameterRecord.HP,
            maxHp = characterParameterRecord.HP,
            currentMp = characterParameterRecord.MP,
            maxMp = characterParameterRecord.MP,
            skillList = new List<int>()
        };

        return characterStatus;
    }

    /// <summary>
    /// 新しくパーティメンバーが加入したときに使用
    /// 加入したいパーティメンバーのキャラクターIDと、レベルを記述する
    /// </summary>
    /// <param name="id"></param>
    /// <param name="level"></param>
    public void SetNewFriend(int id)
    {
        var data = CharacterDataManager.Instance.GetCharacterData(id);
        if (data == null)
        {
            Debug.Log($"[CharacterStatusManager]ID：{id}　はデータに登録されていません。");
            return;
        }
        partyCharacter.Add(id);
        int mainId = 1; // 主人公のID
        var mainCharacterStatus = GetCharacterStatusById(mainId);   // IDから主人公のキャラクターステータスを持ってくる
        int level = mainCharacterStatus.level;  // 新しく加入するメンバーのレベルを、主人公の現在のレベルと同じにする
        characterStatuses.Add(SetCharacterStatus(id, level));   // メンバーを加える
    }

    /// <summary>
    /// パーティ内のキャラクターのステータスをIDで取得します。
    /// </summary>
    /// <param name="characterId">キャラクターのID</param>
    public CharacterStatus GetCharacterStatusById(int characterId)
    {
        return characterStatuses.Find(character => character.characterId == characterId);
    }

    /// <summary>
    /// パーティ内のキャラクターの装備も含めたパラメータをIDで取得します。
    /// </summary>
    /// <param name="characterId">キャラクターのID</param>
    public BattleParameter GetCharacterBattleParameterById(int characterId)
    {
        var characterStatus = GetCharacterStatusById(characterId);
        var parameterTable = CharacterDataManager.Instance.GetParameterTable(characterId);
        var parameterRecord = parameterTable.parameterRecords.Find(p => p.Level == characterStatus.level);
        BattleParameter baseParameter = new()
        {
            Attack = parameterRecord.Attack,
            Defence = parameterRecord.Defence,
            MagicAttack = parameterRecord.MagicAttack,
            MagicDefence = parameterRecord.MagicDefence,
            Speed = parameterRecord.Speed,
            Evasion = parameterRecord.Evasion,
        };

        return baseParameter;
    }

    /// <summary>
    /// 対象のキャラクターのステータスを増減させます。
    /// </summary>
    /// <param name="characterId">キャラクターのID</param>
    /// <param name="hpDelta">増減させるHP</param>
    /// <param name="mpDelta">増減させるMP</param>
    public void ChangeCharacterStatus(int characterId, int hpDelta, int mpDelta)
    {
        var characterStatus = GetCharacterStatusById(characterId);
        if (characterStatus == null)
        {
            Debug.LogWarning($"キャラクターのステータスが見つかりませんでした。 ID : {characterId}");
            return;
        }
        var parameterTable = CharacterDataManager.Instance.GetParameterTable(characterId);
        var parameterRecord = parameterTable.parameterRecords.Find(p => p.Level == characterStatus.level);
        characterStatus.currentHp += hpDelta;
        if (characterStatus.currentHp > parameterRecord.HP)
        {
            characterStatus.currentHp = parameterRecord.HP;
        }
        else if (characterStatus.currentHp < 0)
        {
            characterStatus.currentHp = 0;
        }

        if (characterStatus.currentHp == 0)
        {
            characterStatus.isDefeated = true;
            return;
        }

        characterStatus.currentMp += mpDelta;
        if (characterStatus.currentMp > parameterRecord.MP)
        {
            characterStatus.currentMp = parameterRecord.MP;
        }
        else if (characterStatus.currentMp < 0)
        {
            characterStatus.currentMp = 0;
        }
    }



    /// <summary>
    /// 対象のキャラクターが倒れたかどうかを取得します。
    /// </summary>
    /// <param name="characterId">キャラクターのID</param>
    public bool IsCharacterDefeated(int characterId)
    {
        var characterStatus = GetCharacterStatusById(characterId);
        return characterStatus.isDefeated;
    }
    /// <summary>
    /// 対象のキャラクターが動けるかどうかを取得します。
    /// </summary>
    /// <param name="characterId">キャラクターのID</param>
    public bool IsCharacterStop(int characterId)
    {
        var characterStatus = GetCharacterStatusById(characterId);
        return characterStatus.IsCharaStop;
    }

    /// <summary>
    /// 全てのキャラクターが倒れたかどうかを取得します。
    /// </summary>
    public bool IsAllCharacterDefeated()
    {
        bool isAllDefeated = true;
        foreach (int characterId in partyCharacter)
        {
            var characterStatus = GetCharacterStatusById(characterId);
            if (!characterStatus.isDefeated)
            {
                isAllDefeated = false;
                break;
            }
        }
        return isAllDefeated;
    }

    /// <summary>
    /// 引数のアイテムを使用します。
    /// </summary>
    /// <param name="itemId">アイテムのID</param>
    public void UseItem(int itemId)
    {
        var partyItemInfo = partyItemInfoList.Find(info => info.itemId == itemId);
        if (partyItemInfo == null)
        {
            Debug.LogWarning($"対象のアイテムを所持していません。 ID : {itemId}");
            return;
        }

        partyItemInfo.usedNum++;
        var itemData = ItemDataManager.Instance.GetItemDataById(itemId);
        if (partyItemInfo.usedNum >= itemData.numberOfUse && itemData.numberOfUse > 0)
        {
            partyItemInfo.itemNum--;
        }

        if (partyItemInfo.itemNum <= 0)
        {
            partyItemInfoList.Remove(partyItemInfo);
        }
    }

    /// <summary>
    /// HPが0でないパーティキャラクターの経験値を増加させます。
    /// </summary>
    /// <param name="exp">増加させる経験値</param>
    public void IncreaseExp(int exp)
    {
        foreach (var characterId in partyCharacter)
        {
            var characterStatus = GetCharacterStatusById(characterId);
            if (!characterStatus.isDefeated)
            {
                characterStatus.exp += exp;
            }
        }
    }

    /// <summary>
    /// パーティの所持金を増加させます。
    /// </summary>
    /// <param name="gold">増加させる金額</param>
    public void IncreaseGold(int gold)
    {
        partyGold += gold;
    }
    /// <summary>
    /// 指定したキャラクターがレベルアップしたかどうかを返します。
    /// Trueでレベルアップしています。
    /// </summary>
    public bool CheckLevelUp(int characterId)
    {
        var characterStatus = GetCharacterStatusById(characterId);
        var expTable = CharacterDataManager.Instance.GetExpTable();
        int targetLevel = 1;
        for (int i = 0; i < expTable.expRecords.Count; i++)
        {
            var expRecord = expTable.expRecords[i];
            if (characterStatus.exp >= expRecord.Exp)
            {
                targetLevel = expRecord.Level;
            }
            else
            {
                break;
            }
        }

        if (targetLevel > characterStatus.level)
        {
            characterStatus.level = targetLevel;
            return true;
        }

        return false;
    }
}