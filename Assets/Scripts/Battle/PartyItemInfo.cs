using UnityEngine;
using System;
    [Serializable]
public class PartyItemInfo
{
/// <summary>
        /// アイテムのIDです。
        /// </summary>
        public int itemId;

        /// <summary>
        /// 所有している個数です。
        /// </summary>
        public int itemNum;

        /// <summary>
        /// このアイテムを使用した回数です。
        /// </summary>
        public int usedNum;
}
