using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


/// <summary>
/// ゲーム内のアイテムを管理するクラスです。
/// </summary>
public class ItemDataManager : DontDestroySingleton<ItemDataManager>
{
    /// <summary>
    /// 読み込んだアイテムデータの一覧です。
    /// </summary>
    private List<ItemData> _itemDataList;

    public override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// アイテムデータをロードします。
    /// </summary>
    public async void LoadItemData()
    {
        AsyncOperationHandle<IList<ItemData>> handle = Addressables.LoadAssetsAsync<ItemData>(AddressablesLabels.Item, null);
        await handle.Task;
        _itemDataList = new List<ItemData>(handle.Result);
        handle.Release();
    }

    /// <summary>
    /// IDからアイテムデータを取得します。
    /// </summary>
    public ItemData GetItemDataById(int itemId)
    {
        return _itemDataList.Find(item => item.itemId == itemId);
    }

    /// <summary>
    /// 全てのデータを取得します。
    /// </summary>
    public List<ItemData> GetAllData()
    {
        return _itemDataList;
    }
}