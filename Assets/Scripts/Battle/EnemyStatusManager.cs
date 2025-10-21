using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 戦闘中の敵キャラクターのデータを管理するクラスです。
/// </summary>
public class EnemyStatusManager : DontDestroySingleton<EnemyStatusManager>
{
    /// <summary>
    /// 戦闘中の敵キャラクターのステータス一覧です。
    /// </summary>
    List<EnemyStatus> _enemyStatuses = new();

    /// <summary>
    /// 敵キャラクターのステータス一覧を初期化します。
    /// </summary>
    public void InitializeEnemyStatusList()
    {
        _enemyStatuses.Clear();
    }

    /// <summary>
    /// 敵キャラクターのステータス一覧を取得します。
    /// </summary>
    public List<EnemyStatus> GetEnemyStatusList()
    {
        return _enemyStatuses;
    }

    /// <summary>
    /// 敵キャラクターのステータスを戦闘中のIDで取得します。
    /// </summary>
    /// <param name="battleId">戦闘中のID</param>
    public EnemyStatus GetEnemyStatusByBattleId(int battleId)
    {
        return _enemyStatuses.Find(status => status.enemyBattleId == battleId);
    }

    /// <summary>
    /// 戦闘中の敵キャラクターのうち、最大の戦闘中IDを取得します。
    /// </summary>
    public int GetMaxBattleId()
    {
        return _enemyStatuses.Count > 0 ? _enemyStatuses.Max(status => status.enemyBattleId) : -1;
    }

    /// <summary>
    /// 敵キャラクターのステータスをセットします。
    /// </summary>
    /// <param name="ids">敵キャラクターの定義データのIDのリスト</param>
    public void SetUpEnemyStatus(List<int> ids)
    {
        _enemyStatuses = new List<EnemyStatus>();
        foreach (var enemyId in ids)
        {
            int battleId = GetMaxBattleId() + 1;
            var enemyData = EnemyDataManager.Instance.GetEnemyDataById(enemyId);
            EnemyStatus enemyStatus = new EnemyStatus
            {
                enemyId = enemyId,
                enemyBattleId = battleId,
                enemyData = enemyData,
                currentHp = enemyData.HP,
                currentMp = enemyData.MP
            };
            _enemyStatuses.Add(enemyStatus);
        }
    }

    /// <summary>
    /// 敵キャラクターのステータスを変更します。
    /// </summary>
    /// <param name="battleId">戦闘中のID</param>
    /// <param name="HPDelta">HPの変化量</param>
    /// <param name="mpDelta">MPの変化量</param>
    public void ChangeEnemyStatus(int battleId, int HPDelta, int mpDelta)
    {
        var enemyStatus = GetEnemyStatusByBattleId(battleId);
        if (enemyStatus == null)
        {
            Logger.Instance.LogWarning($"敵キャラクターのステータスが見つかりませんでした。 戦闘中ID : {battleId}");
            return;
        }

        enemyStatus.currentHp += HPDelta;
        if (enemyStatus.currentHp > enemyStatus.enemyData.HP)
        {
            enemyStatus.currentHp = enemyStatus.enemyData.HP;
        }
        else if (enemyStatus.currentHp < 0)
        {
            enemyStatus.currentHp = 0;
        }

        enemyStatus.currentMp += mpDelta;
        if (enemyStatus.currentMp > enemyStatus.enemyData.MP)
        {
            enemyStatus.currentMp = enemyStatus.enemyData.MP;
        }
        else if (enemyStatus.currentMp < 0)
        {
            enemyStatus.currentMp = 0;
        }
    }



    /// <summary>
    /// 敵キャラクターが倒れたかどうかを取得します。
    /// </summary>
    /// <param name="battleId">戦闘中のID</param>
    public bool IsEnemyDefeated(int battleId)
    {
        var enemyStatus = GetEnemyStatusByBattleId(battleId);
        return enemyStatus.currentHp <= 0;
    }

    /// <summary>
    /// 引数の敵キャラクターが倒れたフラグをセットします。
    /// </summary>
    /// <param name="battleId">戦闘中のID</param>
    public void OnDefeatEnemy(int battleId)
    {
        var enemyStatus = GetEnemyStatusByBattleId(battleId);
        enemyStatus.isDefeated = true;
    }

    /// <summary>
    /// 引数の敵キャラクターが動けるかどうかを取得します。
    /// </summary>
    /// <param name="characterId">キャラクターのID</param>
    public bool IsEnemyStop(int battleId)
    {
        var enemyStatus = GetEnemyStatusByBattleId(battleId);
        return enemyStatus.IsEnemyStop;
    }

    /// <summary>
    /// 引数の敵キャラクターが逃げたフラグをセットします。
    /// </summary>
    /// <param name="battleId">戦闘中のID</param>
    public void OnRunEnemy(int battleId)
    {
        var enemyStatus = GetEnemyStatusByBattleId(battleId);
        enemyStatus.isRunaway = true;
    }

    /// <summary>
    /// 全ての敵キャラクターが倒れた、または逃げたかどうかを取得します。
    /// </summary>
    public bool IsAllEnemyDefeated()
    {
        bool isAllDefeated = true;
        foreach (var enemyStatus in _enemyStatuses)
        {
            if (!enemyStatus.isDefeated && !enemyStatus.isRunaway)
            {
                isAllDefeated = false;
                break;
            }
        }
        return isAllDefeated;
    }
}