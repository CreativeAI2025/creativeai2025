using UnityEngine;
using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// ゲーム内の魔法データを管理するクラスです。
/// </summary>
public class SkillDataManager : DontDestroySingleton<SkillDataManager>
{
    [SerializeField] private SkillDatabase _skillDatabase;
    public override void Awake()
    {
        base.Awake();
    }

    public void Initialize()
    {
        LoadSkillData();
        Debug.Log("[SkillDatamanager]すべてのデータのロードが完了しました。");
    }

    /// <summary>
    /// 魔法データをロードします。
    /// </summary>
    public void LoadSkillData()
    {
        _skillDatabase.Initialize();
    }

    /// <summary>
    /// IDから魔法データを取得します。
    /// </summary>
    public SkillData GetSkillDataById(int skillId)
    {
        return _skillDatabase.GetSkillData(skillId);
    }
}