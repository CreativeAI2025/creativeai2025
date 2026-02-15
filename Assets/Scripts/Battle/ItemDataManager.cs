using UnityEngine;

/// <summary>
/// ゲーム内のアイテムを管理するクラスです。
/// </summary>
public class ItemDataManager : DontDestroySingleton<ItemDataManager>
{
    /// <summary>
    /// 読み込んだアイテムデータの一覧です。
    /// </summary>
    [SerializeField] private ItemDatabase _itemDatabase;

    public override void Awake()
    {
        base.Awake();
        Initialize();
    }

    public void Initialize()
    {
        LoadItemData();
        Debug.Log("[ItemDatamanager]すべてのデータのロードが完了しました。");
    }

    /// <summary>
    /// アイテムデータをロードします。
    /// </summary>
    private void LoadItemData()
    {
        _itemDatabase.Initialize();
    }

    /// <summary>
    /// IDからアイテムデータを取得します。
    /// </summary>
    public ItemData GetItemDataById(int itemId)
    {
        return _itemDatabase.GetItemData(itemId);
    }
}