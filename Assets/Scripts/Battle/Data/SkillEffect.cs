using UnityEngine;
using System;
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
    /// 対象ステータス
    /// </summary>
    [Header("対象ステータス")] public string status = "対象ステータスはありません";

    /// <summary>
    /// 持続ターン数
    /// </summary>
    public int duration = 1;//持続ターン

    public SkillEffect(SkillCategory skillCategory, EffectTarget EffectTarget, float value = 0, int probability = 100, string status = "対象ステータスはありません", int duration = 1)//コンストラクター
    {
        this.skillCategory = skillCategory;
        this.EffectTarget = EffectTarget;
        this.value = value;
        this.probability = probability;
        this.status = status;
        this.duration = duration;
    }
}