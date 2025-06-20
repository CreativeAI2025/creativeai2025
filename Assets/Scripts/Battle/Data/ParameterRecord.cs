using UnityEngine;
using System;

namespace SimpleRpg
{
   /// <summary>
   /// レベルに対応するパラメータを保持するクラスです。
   /// </summary>
   [Serializable]
   public class ParameterRecord
   {
      //キャラクターのパラメータ
      public int Level;
      public int HP;
      public int MP;
      public int Attack;
      public int Defence;
      public int MagicAttack;
      public int MagicDefence;
      public int Speed;
      public int Evasion;


   }
}