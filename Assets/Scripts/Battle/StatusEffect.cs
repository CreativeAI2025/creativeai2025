using UnityEngine;
using System;
using System.Collections.Generic;
[Serializable]
public class StatusEffect
{
    public StatusEffectCategory EffectCategory; //敵味方に状態異常のフラグを付ける、四つの処理をマネージャーに書く、onoffを制御、ここにはフラグを制御するだけ
    public int Duration; // 残りターン数
    public int Power;   // 効果の強さ（ダメージ量など）

    public StatusEffect(StatusEffectCategory EffectCategory = StatusEffectCategory.None, int Duration = 0, int Power = 0)
    {
        this.EffectCategory = EffectCategory;
        this.Duration = Duration;
        this.Power = Power;
    }
}