using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

    /// <summary>
    /// ゲーム内の魔法データを管理するクラスです。
    /// </summary>
public class SkillDataManager : DontDestroySingleton<SkillDataManager>
{
    /// <summary>
    /// 読み込んだ魔法データの一覧です。
    /// </summary>
    private List<SkillData> _skillDataList = new();
        
    public override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 魔法データをロードします。
    /// </summary>
    public async void LoadSkillData()
    {
        AsyncOperationHandle<IList<SkillData>> handle = Addressables.LoadAssetsAsync<SkillData>(AddressablesLabels.Skill, null);
        await handle.Task;
        _skillDataList = new List<SkillData>(handle.Result);
        handle.Release();
    }

    /// <summary>
    /// IDから魔法データを取得します。
    /// </summary>
    public SkillData GetSkillDataById(int skillId)
    {
        return _skillDataList.Find(skill => skill.skillId == skillId);
    }

    /// <summary>
    /// 全てのデータを取得します。
    /// </summary>
    public List<SkillData> GetAllData()
    {
        return _skillDataList;
    }
}