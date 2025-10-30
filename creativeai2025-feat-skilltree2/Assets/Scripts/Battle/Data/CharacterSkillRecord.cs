using System;
using UnityEngine;

 [Serializable]
    public class CharacterSkillRecord
    {
        /// <summary>
        /// 魔法を覚えるレベルの値です。
        /// </summary>
        public int level;

        /// <summary>
        /// 覚える魔法のIDです。
        /// </summary>
        public int skillId;
    }
