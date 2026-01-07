using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Scriptable Objects/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public ItemData[] itemDataList;
    private Dictionary<int, int> _itemDataDict;

    public void Initialize()
    {
        _itemDataDict = new Dictionary<int, int>();
        for (int i = 0; i < itemDataList.Length; i++)
        {
            var itemData = itemDataList[i];
            _itemDataDict.Add(itemData.itemId, i);
        }
    }

    public ItemData GetItemData(int id)
    {
        return itemDataList[_itemDataDict[id]];
    }
}
