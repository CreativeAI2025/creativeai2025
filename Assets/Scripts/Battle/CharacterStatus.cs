using System;
using System.Collections.Generic;

/// <summary>
/// キャラクターの個別のステータス情報を保持するクラスです。
/// </summary>
[Serializable]
public class CharacterStatus
{
    /// <summary>
    /// キャラクターのIDです。
    /// </summary>
    public int characterId;

    /// <summary>
    /// キャラクターのレベルです。
    /// </summary>
    public int level;

    /// <summary>
    /// キャラクターの経験値です。
    /// </summary>
    public int exp;

    /// <summary>
    /// 現在のHPです。
    /// </summary>
    public int currentHp;

    /// <summary>
    /// 現在の最大HPです。
    /// </summary>
    public int maxHp;

    /// <summary>
    /// 現在のMPです。
    /// </summary>
    public int currentMp;

    /// <summary>
    /// 現在の最大MPです。
    /// </summary>
    public int maxMp;

    public int currentAttack;
    public int currentDefence;
    public int currentMagicAttack;
    public int currentMagicDefence;
    public int currentSpeed;
   public int currentEvasion;


    /// <summary>
    /// 装備中の武器のIDです。
    /// </summary>
    public int equipWeaponId;

    /// <summary>
    /// 装備中の防具のIDです。
    /// </summary>
    public int equipArmorId;
    /// <summary>
    /// 覚えた魔法のIDのリストです。
    /// </summary>
    public List<int> skillList = new();

    /// <summary>
    /// キャラクターが倒されたフラグです。
    /// </summary>
    public bool isDefeated = false;

    ///<summary>
    /// 状態異常にかかっているかのフラグです。
    /// </summary>
    public bool isStatusEffect = false;
    ///<summary>
    /// 状態異常のターン数です。
    /// </summary>
    public int Duration;
    ///<summary>
    /// 状態異常のどれにかかっているかのフラグです。
    /// </summary>
    public bool Poison = false;
    public bool Paralysis = false;
    public bool Sleep = false;
    public bool Confusion = false;
    ///<summary>
    /// 動けるかどうかのフラグです。
    /// </summary>
    public bool IsCharaStop = false;
    // --- バフ・デバフ倍率 + 残りターン ---
    public float attackBuffMultiplier = 1.0f;
    public int attackBuffDuration = 0;

    public float defenceBuffMultiplier = 1.0f;
    public int defenceBuffDuration = 0;

    public float magicAttackBuffMultiplier = 1.0f;
    public int magicAttackBuffDuration = 0;

    public float magicDefenceBuffMultiplier = 1.0f;
    public int magicDefenceBuffDuration = 0;

    public float speedBuffMultiplier = 1.0f;
    public int speedBuffDuration = 0;

    public float evasionBuffMultiplier = 1.0f;
    public int evasionBuffDuration = 0;


}

