    using UnityEngine;
using System;
using System.Collections.Generic;
[Serializable]
public class Buff
{
    public BuffStatusCategory BuffCategory; 
        /// <summary>
    /// 魔法の効果範囲です。
    /// </summary>
    public EffectTarget EffectTarget;
    public int Duration; // 残りターン数
    public float Power;   // 効果の強さ（倍率）

}
