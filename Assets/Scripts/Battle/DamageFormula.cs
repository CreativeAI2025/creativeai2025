using System;

class DamageFormula //ダメージ計算式を書く
{
    int Damage = Parameter.Attack - Parameter.Defence;
      public int GetDamage()
    {
        return Damage;
    }
}