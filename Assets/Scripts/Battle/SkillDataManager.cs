using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public List<SkillData> _skillDataList;
    private Dictionary<int, SkillData> _skillDataDictionary;
    public override void Awake()
    {
        base.Awake();
        LoadSkillData();
    }

    /*
    /// <summary>
    /// デバッグ用
    /// </summary>
    public void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            int rand = Random.Range(1, _skillDataDictionary.Count + 1);
            SkillData sd = GetSkillDataById(rand);
            Debug.Log("[SkillDataManager]ランダムなスキルデータを習得" + sd);
        }
    }
    */

    /// <summary>
    /// 魔法データをロードします。
    /// </summary>
    public async void LoadSkillData()
    {
        AsyncOperationHandle<IList<SkillData>> handle = Addressables.LoadAssetsAsync<SkillData>(AddressablesLabels.Skill, null);
        await handle.Task;
        _skillDataList = new List<SkillData>(handle.Result);
        handle.Release();
        _skillDataDictionary = _skillDataList.ToDictionary(skill => skill.skillId, skill => skill);
        Debug.Log("[SkillDataManager]LoadSkillData Count:" + _skillDataDictionary.Count);
    }

    /// <summary>
    /// IDから魔法データを取得します。
    /// </summary>
    public SkillData GetSkillDataById(int skillId)
    {
        _skillDataDictionary.TryGetValue(skillId, out SkillData skillData);
        return skillData;
    }

    /// <summary>
    /// 全てのデータを取得します。
    /// </summary>
    public List<SkillData> GetAllData()
    {
        return _skillDataList;
    }
}