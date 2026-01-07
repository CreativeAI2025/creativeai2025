using UnityEngine;

/// <summary>
/// ゲーム内の敵キャラクターのデータを管理するクラスです。
/// </summary>
public class EnemyDataManager : DontDestroySingleton<EnemyDataManager>
{
    /// <summary>
    /// 読み込んだ敵キャラクターのデータの一覧です。
    /// </summary>
    [SerializeField] private EnemyDatabase _enemyDatabase;

    public override void Awake()
    {
        base.Awake();
    }

    public void Initialize()
    {
        LoadEnemyData();
        Debug.Log("[EnemyDataManager]すべてのデータのロードが完了しました。");
    }

    /// <summary>
    /// 敵キャラクターのデータをロードします。
    /// </summary>
    public void LoadEnemyData()
    {
        _enemyDatabase.Initialize();
    }

    /// <summary>
    /// IDから敵キャラクターのデータを取得します。
    /// </summary>
    public EnemyData GetEnemyDataById(int enemyId)
    {
        return _enemyDatabase.GetEnemyData(enemyId);
    }
}