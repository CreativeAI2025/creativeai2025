using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 戦闘機能のテストを行うためのクラスです。
/// </summary>
public class BattleTester : MonoBehaviour
{
    /// <summary>
    /// 戦闘機能を管理するクラスへの参照です。
    /// </summary>
    [SerializeField]
    BattleManager _battleManager;

    [Header("テスト用の設定")]
    /// <summary>
    /// 戦う対象の敵キャラクターのIDです。
    /// </summary>
    [SerializeField]
    int _enemyId1;
    //  [SerializeField]
    // int _enemyId2;
    //      [SerializeField]
    // int _enemyId3;
    //      [SerializeField]
    // int _enemyId4;
    //      [SerializeField]
    // int _enemyId5;


    /// <summary>
    /// 味方キャラクターのレベルです。
    /// </summary>
    [SerializeField]
    int _playerLevel;



    /// <summary>
    /// アイテム所持数の設定です。
    /// </summary>
    [SerializeField]
    List<PartyItemInfo> _partyItemInfoList = new();

    [Header("戦闘機能をテストする")]
    /// <summary>
    /// 戦闘機能をテストするフラグです。
    /// Inspectorウィンドウからチェックを入れると、戦闘機能をテストします。
    /// </summary>
    [SerializeField]
    bool _executeBattle;

    void Update()
    {
        CheckStartFlag();
    }

    /// <summary>
    /// 戦闘機能をテストするフラグの状態をチェックします。
    /// </summary>
    void CheckStartFlag()
    {
        // 定義データのロードを待つため、最初の5フレームは処理を抜けます。
        if (Time.frameCount < 5)
        {
            return;
        }

        if (!_executeBattle)
        {
            return;
        }

        _executeBattle = false;

        // 戦闘中でない場合のみ、戦闘を開始します。
        if (_battleManager.BattlePhase != BattlePhase.NotInBattle)
        {
            Logger.Instance.Log("戦闘中のためBattleTesterの処理を抜けます。");
            return;
        }

        ReadyForBattle();
    }

    /// <summary>
    /// 戦闘を開始する準備を行います。
    /// </summary>
    void ReadyForBattle()
    {
        SetPlayerStatus();
        SetEnemyId();
        StartBattle();
    }

    /// <summary>
    /// 味方キャラクターのステータスをセットします。
    /// </summary>
    private void SetPlayerStatus()
    {
        // 経験値表を使って、レベルから経験値を取得します。
        var expTable = CharacterDataManager.Instance.GetExpTable();
        var expRecord = expTable.expRecords.Find(record => record.Level == _playerLevel);
        var exp = expRecord.Exp;

        // レベルに対応するパラメータデータを取得します。
        int charcterId = 1;
        var parameterTable = CharacterDataManager.Instance.GetParameterTable(charcterId);
        var parameterRecord = parameterTable.parameterRecords.Find(record => record.Level == _playerLevel);

        // 指定したレベルまでに覚えている魔法のIDをリスト化します。
        var skillList = GetSkillIdList(charcterId, _playerLevel);

        // キャラクターのステータスを設定します。
        CharacterStatus status = new()
        {
            characterId = charcterId,
            level = _playerLevel,
            exp = exp,
            currentHp = parameterRecord.HP,
            currentMp = parameterRecord.MP,

            skillList = skillList,
        };

        CharacterStatusManager.Instance.characterStatuses = new()
            {
                status
            };

        // パーティにいるキャラクターのIDをセットします。
        CharacterStatusManager.Instance.partyCharacter = new()
            {
                charcterId
            };

        // 所持アイテムをセットします。
        CharacterStatusManager.Instance.partyItemInfoList = _partyItemInfoList;
    }

    /// <summary>
    /// キャラクターが覚えている魔法のIDリストを返します。
    /// </summary>
    /// <param name="characterId">キャラクターID</param>
    /// <param name="level">キャラクターのレベル</param>
    List<int> GetSkillIdList(int characterId, int level)
    {
        var learnableSkill = CharacterDataManager.Instance.GetLearnableSkill(characterId, level);
        List<int> skillList = new();
        foreach (var record in learnableSkill)
        {
            skillList.Add(record.skillId);
        }
        return skillList;
    }

    /// <summary>
    /// 敵キャラクターのIDをセットします。
    /// </summary>
    void SetEnemyId()
    {
        _battleManager.SetUpEnemyStatus(_enemyId1);
        // _battleManager.SetUpEnemyStatus(_enemyId2);
        // _battleManager.SetUpEnemyStatus(_enemyId3);
        // _battleManager.SetUpEnemyStatus(_enemyId4);
        // _battleManager.SetUpEnemyStatus(_enemyId5);

    }

    /// <summary>
    /// 戦闘を開始します。
    /// </summary>
    void StartBattle()
    {
        //_battleManager.StartBattle();
    }
}