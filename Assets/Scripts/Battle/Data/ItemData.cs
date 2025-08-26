using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]

public class ItemData : ScriptableObject
{
  /// <summary>
        /// アイテムのIDです。
        /// </summary>
        public int itemId;

        /// <summary>
        /// アイテムの名前です。
        /// </summary>
        public string itemName;

        /// <summary>
        /// アイテムの説明です。
        /// </summary>
        public string itemDesc;

        /// <summary>
        /// アイテムのカテゴリです。
        /// </summary>
        public ItemCategory itemCategory;

        /// <summary>
        /// アイテムの効果です。
        /// </summary>
        public ItemEffect itemEffect;

        /// <summary>
        /// 使用可能回数です。
        /// </summary>
        public int numberOfUse;

        /// <summary>
        /// アイテムの価格です。
        /// </summary>
        public int price;

}
