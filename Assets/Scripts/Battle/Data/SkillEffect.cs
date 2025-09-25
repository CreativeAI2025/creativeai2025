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
        public int value;
    }