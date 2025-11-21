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
    private List<EnemyData> _enemyData = new();

    public override void Awake()
    {
        base.Awake();
    }

    public async Task Initialize()
    {
        await Task.WhenAll(
            LoadEnemyData()
        );
        Debug.Log("[EnemyDataManager]すべてのデータのロードが完了しました。");
    }

    /// <summary>
    /// 敵キャラクターのデータをロードします。
    /// </summary>
    public async Task LoadEnemyData()
    {
        AsyncOperationHandle<IList<EnemyData>> handle = Addressables.LoadAssetsAsync<EnemyData>(AddressablesLabels.Enemy, null);
        await handle.Task;
        _enemyData = new List<EnemyData>(handle.Result);
        handle.Release();
    }

    /// <summary>
    /// IDから敵キャラクターのデータを取得します。
    /// </summary>
    public EnemyData GetEnemyDataById(int enemyId)
    {
        return _enemyData.Find(enemy => enemy.enemyId == enemyId);
    }

    /// <summary>
    /// 全てのデータを取得します。
    /// </summary>
    public List<EnemyData> GetAllData()
    {
        return _enemyData;
    }
}