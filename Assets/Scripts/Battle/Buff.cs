using UnityEngine;
using System;
using System.Collections.Generic;
[Serializable]
public class Buff
{
    public BuffStatusCategory BuffCategory;
    public EffectTarget effectTarget;//対象の範囲
    public int Duration; // 残りターン数
    public float Power;   // 効果の強さ（倍率）

    public Buff(BuffStatusCategory buffCategory = BuffStatusCategory.Attack, int duration = 0, float power = 0, EffectTarget effectTarget = EffectTarget.Own)
    {
        this.BuffCategory = buffCategory;
        this.Duration = duration;
        this.Power = power;
        this.effectTarget = effectTarget;
    }
}
