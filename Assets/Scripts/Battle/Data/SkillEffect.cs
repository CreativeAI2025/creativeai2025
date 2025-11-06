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

    /// <summary>
    /// 発動確率
    /// </summary>
    public int probability = 100;

    /// <summary>
    /// 状態異常のリストです
    /// </summary>
    public List<StatusEffect> StatusEffect = new List<StatusEffect>();
    /// <summary>
    /// バフのリストです
    /// </summary>
    public List<Buff> buff = new List<Buff>();

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
    [Header("追加効果の有無（あればTrue）")] public bool isExtra = false;

    /// <summary>
    /// 追加効果の魔法のカテゴリです。
    /// </summary>
    public SkillCategory extar_skillCategory;

    /// <summary>
    /// 追加効果の効果範囲です。
    /// </summary>
    public EffectTarget extar_EffectTarget;

    /// <summary>
    /// 追加効果の効果量です。
    /// </summary>
    public float extra_value;

    /// <summary>
    /// 追加効果の発動確率
    /// </summary>
    public int extra_probability = 100;

    /// <summary>
    /// 状態異常のリストです
    /// </summary>
    public List<StatusEffect> extra_StatusEffect = new List<StatusEffect>();
    /// <summary>
    /// バフのリストです
    /// </summary>
    public List<Buff> extar_buff = new List<Buff>();

    /// <summary>
    /// 追加効果の対象ステータス
    /// </summary>
    [Header("対象ステータス")] public string extra_status = "対象ステータスはありません";

    /// <summary>
    /// 追加効果の持続ターン数
    /// </summary>
    public int extra_duration = 1;//持続ターン

    public SkillEffect()
    {
    }

    public SkillEffect(SkillCategory skillCategory, EffectTarget EffectTarget, float value = 0, int probability = 100, string status = "対象ステータスはありません", int duration = 1,
    bool isExtra = false, SkillCategory extar_skillCategory = SkillCategory.None, EffectTarget extar_EffectTarget = EffectTarget.Own, float extra_value = 0, int extra_probability = 100,
    string extra_status = "対象ステータスはありません", int extra_duration = 1)//コンストラクター
    {
        this.skillCategory = skillCategory;
        this.EffectTarget = EffectTarget;
        this.value = value;
        this.probability = probability;
        this.status = status;
        this.duration = duration;

        this.isExtra = isExtra;
        this.extar_skillCategory = extar_skillCategory;
        this.extar_EffectTarget = extar_EffectTarget;
        this.extra_value = extra_value;
        this.extra_probability = extra_probability;
        this.extra_status = extra_status;
        this.extra_duration = extra_duration;
    }

    public void SetBuffList(List<Buff> buff)
    {
        this.buff = buff;
    }
}