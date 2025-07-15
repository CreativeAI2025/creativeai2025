using System;
using System.Collections.Generic;

    /// <summary>
    /// キャラクターの個別のステータス情報を保持するクラスです。
    /// </summary>
    [Serializable]
    public class CharacterStatus
    {
        /// <summary>
        /// キャラクターのIDです。
        /// </summary>
        public int characterId;

        /// <summary>
        /// キャラクターのレベルです。
        /// </summary>
        public int level;

        /// <summary>
        /// キャラクターの経験値です。
        /// </summary>
        public int exp;

        /// <summary>
        /// 現在のHPです。
        /// </summary>
        public int currentHp;

        /// <summary>
        /// 現在のMPです。
        /// </summary>
        public int currentMp;

        /// <summary>
        /// 装備中の武器のIDです。
        /// </summary>
        public int equipWeaponId;

        /// <summary>
        /// 装備中の防具のIDです。
        /// </summary>
        public int equipArmorId;

        /// <summary>
        /// 覚えた魔法のIDのリストです。
        /// </summary>
        public List<int> magicList;

        /// <summary>
        /// キャラクターが倒されたフラグです。
        /// </summary>
        public bool isDefeated;
    }
