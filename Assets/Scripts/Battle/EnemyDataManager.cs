using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// ゲーム内の敵キャラクターのデータを管理するクラスです。
/// </summary>
public class EnemyDataManager : DontDestroySingleton<EnemyDataManager>
{
    /// <summary>
    /// 読み込んだ敵キャラクターのデータの一覧です。
    /// </summary>
    private List<EnemyData> _enemyDataList = new();
    private Dictionary<int, Sprite> _enemySpriteDict;

    public override void Awake()
    {
        base.Awake();
    }

    public async Task Initialize()
    {
        await Task.WhenAll(
            LoadEnemyData()
        );
        await LoadAllEnemySprites();
        Debug.Log("[EnemyDataManager]すべてのデータのロードが完了しました。");
    }

    /// <summary>
    /// 敵キャラクターのデータをロードします。
    /// </summary>
    public async Task LoadEnemyData()
    {
        AsyncOperationHandle<IList<EnemyData>> handle = Addressables.LoadAssetsAsync<EnemyData>(AddressablesLabels.Enemy, null);
        await handle.Task;
        _enemyDataList = new List<EnemyData>(handle.Result);
        handle.Release();
    }

    /// <summary>
    /// 全敵のSpriteデータをロードします。
    /// </summary>
    private async Task LoadAllEnemySprites()
    {
        _enemySpriteDict = new Dictionary<int, Sprite>();

        // CharacterDataList を基に、すべての Sprite を並行してロード
        var loadTasks = new List<Task>();
        var spriteHandles = new List<AsyncOperationHandle<Sprite>>();

        foreach (var data in _enemyDataList)
        {
            // AssetReferenceSprite からロード操作を開始
            AsyncOperationHandle<Sprite> handle = data.sprite.LoadAssetAsync<Sprite>();
            spriteHandles.Add(handle);

            // ロード完了を待つ Task をリストに追加
            loadTasks.Add(handle.Task.ContinueWith(t =>
            {
                if (t.Status == TaskStatus.RanToCompletion && handle.Status == AsyncOperationStatus.Succeeded)
                {
                    // ロード成功時のみ Dictionary に追加
                    lock (_enemySpriteDict) // 並行処理のためロック推奨
                    {
                        _enemySpriteDict[data.enemyId] = handle.Result;
                    }
                }
                else
                {
                    Debug.LogError($"Spriteロード失敗: ID {data.enemyId}");
                }
            }));
        }

        // 全てのロード完了を待機
        await Task.WhenAll(loadTasks);
    }

    /// <summary>
    /// IDから敵キャラクターのデータを取得します。
    /// </summary>
    public EnemyData GetEnemyDataById(int enemyId)
    {
        return _enemyDataList.Find(enemy => enemy.enemyId == enemyId);
    }

    /// <summary>
    /// 全てのデータを取得します。
    /// </summary>
    public List<EnemyData> GetAllData()
    {
        return _enemyDataList;
    }

    /// <summary>
    /// 敵IDからイメージを返す
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Sprite GetEnemySprite(int id)
    {
        if (_enemySpriteDict != null && _enemySpriteDict.ContainsKey(id))
        {
            return _enemySpriteDict[id];
        }
        return null; // ロードされていない、またはIDが存在しない
    }
}