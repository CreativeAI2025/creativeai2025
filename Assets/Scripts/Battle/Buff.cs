    using UnityEngine;
using System;
using System.Collections.Generic;
[Serializable]
public class Buff
{
    public BuffStatusCategory BuffCategory; 
    public int Duration; // 残りターン数
    public int Power;   // 効果の強さ（倍率）

}
