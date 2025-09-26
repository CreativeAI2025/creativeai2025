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
        /// 攻撃力の補正値です。
        /// </summary>
        public int strength;

        /// <summary>
        /// 防御力の補正値です。
        /// </summary>
        public int guard;

        /// <summary>
        /// 素早さの補正値です。
        /// </summary>
        public int speed;

        /// <summary>
        /// アイテムの価格です。
        /// </summary>
        public int price;

}
