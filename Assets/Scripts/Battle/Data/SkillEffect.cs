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
    public bool buturi;
    /// <summary>
    /// 状態異常のリストです
    /// </summary>
    public List<StatusEffect> StatusEffect;
    /// <summary>
    /// バフのリストです
    /// </summary>
    public List<Buff> Buff;
    }