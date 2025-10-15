using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 敵キャラクターの出現率を定義するクラスです。
    /// </summary>
    [CreateAssetMenu(fileName = "EncounterData", menuName = "Scriptable Objects/SimpleRpg/EncounterData")]
    public class EncounterData : ScriptableObject
    {
        /// <summary>
        /// 敵キャラクターが出現するかどうかのフラグです。
        /// Trueで出現します。
        /// </summary>
        public bool hasEncounter;

        /// <summary>
        /// 敵キャラクターの出現率です。
        /// </summary>
        [Range(0, 100)]
        public float rate;

        /// <summary>
        /// 敵キャラクターの出現率リストです。
        /// </summary>
        public List<EnemyRate> enemyRates;
    }
}