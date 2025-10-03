using UnityEngine;

/// <summary>
/// 戦闘中の敵キャラクターのステータスを保持するクラスです。
/// </summary>
public class EnemyStatus
{
    /// <summary>
    /// 敵キャラクターのIDです。
    /// </summary>
    public int enemyId;

    /// <summary>
    /// 敵キャラクターの戦闘中の通しIDです。
    /// </summary>
    public int enemyBattleId;

    /// <summary>
    /// 敵キャラクターの定義データです。
    /// </summary>
    public EnemyData enemyData;

    /// <summary>
    /// 敵キャラクターの現在のHPです。
    /// </summary>
    public int currentHp;

    /// <summary>
    /// 敵キャラクターの現在のMPです。
    /// </summary>
    public int currentMp;

    /// <summary>
    /// 敵キャラクターが倒されたフラグです。
    /// </summary>
    public bool isDefeated;

    /// <summary>
    /// 敵キャラクターが逃げたフラグです。
    /// </summary>
    public bool isRunaway;

    ///<summary>
    /// 状態異常にかかっているかのフラグです。
    /// </summary>
    public bool isStatusEffect;
    ///<summary>
    /// 状態異常のターン数です。
    /// </summary>
    public int Duration;
    ///<summary>
    /// 状態異常のどれにかかっているかのフラグです。
    /// </summary>
    public bool Poison;
    public bool Paralysis;
    public bool Sleep;
    public bool Confusion;
    ///<summary>
    /// 行動できないかどうかのフラグです。
    /// </summary>
    public bool IsEnemyStop;
}