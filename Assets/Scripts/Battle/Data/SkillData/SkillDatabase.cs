using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SkillDatabase", menuName = "Scriptable Objects/SkillDatabase")]
public class SkillDatabase : ScriptableObject
{
    public SkillData[] skillDataList;
    private Dictionary<int, int> _skillDataDict;

    public void Initialize()
    {
        _skillDataDict = new Dictionary<int, int>();
        for (int i = 0; i < skillDataList.Length; i++)
        {
            var skillData = skillDataList[i];
            _skillDataDict.Add(skillData.skillId, i);
        }
    }

    public SkillData GetSkillData(int id)
    {
        return skillDataList[_skillDataDict[id]];
    }
}
