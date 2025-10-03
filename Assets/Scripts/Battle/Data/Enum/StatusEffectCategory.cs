using UnityEngine;

public enum StatusEffectCategory
{
    None,
    Poison,     // 毒：毎ターンダメージ
    Paralysis,  // 麻痺：一定確率で行動不能
    Sleep,      // 睡眠：数ターン行動不能
    Confusion,       //混乱：確率で味方を攻撃
}