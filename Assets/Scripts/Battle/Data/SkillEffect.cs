using UnityEngine;
using System;
using System.Collections.Generic;
[Serializable]
public class SkillEffect
{
    /// <summary>
    /// 魔法のカテゴリです。
    /// </summary>
    public SkillCategory skillCategory;

    /// <summary>
    /// 魔法の効果範囲です。
    /// </summary>
    public EffectTarget EffectTarget;

    /// <summary>
    /// 効果量です。
    /// </summary>
    public float value;

    public bool StatusEffectEnable;
    /// <summary>
    /// 状態異常のリストです
    /// </summary>
    public List<StatusEffect> StatusEffect;
    /// <summary>
    /// バフのリストです
    /// </summary>
    public List<Buff> Buff;
    }
    /// <summary>
    /// 発動確率
    /// </summary>
    public int probability = 100;

    /// <summary>
    /// 対象ステータス
    /// </summary>
    [Header("対象ステータス")] public string status = "対象ステータスはありません";

    /// <summary>
    /// 持続ターン数
    /// </summary>
    public int duration = 1;//持続ターン


    /// <summary>
    /// 追加効果の有無
    /// </summary>
    public bool isExtra = false;

    /// <summary>
    /// 追加効果の効果量です。
    /// </summary>
    public float extra_value;

    /// <summary>
    /// 追加効果の発動確率
    /// </summary>
    public int extra_probability = 100;

    /// <summary>
    /// 追加効果の対象ステータス
    /// </summary>
    [Header("対象ステータス")] public string extra_status = "対象ステータスはありません";

    /// <summary>
    /// 追加効果の持続ターン数
    /// </summary>
    public int extra_duration = 1;//持続ターン

    public SkillEffect(SkillCategory skillCategory, EffectTarget EffectTarget, float value = 0, int probability = 100, string status = "対象ステータスはありません", int duration = 1,
    bool isExtra = false, float extra_value = 0, int extra_probability = 100, string extra_status = "対象ステータスはありません", int extra_duration = 1)//コンストラクター
    {
        this.skillCategory = skillCategory;
        this.EffectTarget = EffectTarget;
        this.value = value;
        this.probability = probability;
        this.status = status;
        this.duration = duration;

        this.isExtra = isExtra;
        this.extra_value = extra_value;
        this.extra_probability = extra_probability;
        this.extra_status = extra_status;
        this.extra_duration = extra_duration;
    }
}
