using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// ゲーム内のアイテムを管理するクラスです。
/// </summary>
public class ItemDataManager : DontDestroySingleton<ItemDataManager>
{
    /// <summary>
    /// 読み込んだアイテムデータの一覧です。
    /// </summary>
    private List<ItemData> _itemDataList;
    private Dictionary<int, ItemData> _itemDataDictionary;

    public override void Awake()
    {
        base.Awake();
    }

    public async Task Initialize()
    {
        await Task.WhenAll(
            LoadItemData()
        );
        Debug.Log("[ItemDatamanager]すべてのデータのロードが完了しました。");
    }

    /// <summary>
    /// アイテムデータをロードします。
    /// </summary>
    private async Task LoadItemData()
    {
        AsyncOperationHandle<IList<ItemData>> handle = Addressables.LoadAssetsAsync<ItemData>(AddressablesLabels.Item, null);
        await handle.Task;
        _itemDataList = new List<ItemData>(handle.Result);
        handle.Release();
        _itemDataDictionary = _itemDataList.ToDictionary(item => item.itemId, item => item);
        Debug.Log("[ItemDataManager]LoadItemData Count:" + _itemDataDictionary.Count);
    }

    /// <summary>
    /// IDからアイテムデータを取得します。
    /// </summary>
    public ItemData GetItemDataById(int itemId)
    {
        _itemDataDictionary.TryGetValue(itemId, out ItemData itemData);
        return itemData;
    }

    /// <summary>
    /// 全てのデータを取得します。
    /// </summary>
    public List<ItemData> GetAllData()
    {
        return _itemDataList;
    }
}