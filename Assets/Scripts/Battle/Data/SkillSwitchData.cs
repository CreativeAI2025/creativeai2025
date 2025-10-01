using UnityEngine;

[System.Serializable]
public class SkillSwitchData
{
    /// <summary>
    /// スキルの詳細を手に入れる場合は、
    /// 「SkillDataManager.Instance.GetSkillDataById(int skillId)」（返り値SkillDataクラス）
    /// によって手に入ります。
    /// </summary>
    
    /// <summary>
    /// スキルID
    /// </summary>
    public int skillId;
    /// <summary>
    /// このスキルを習得したかどうかの判別
    /// </summary>
    public bool isLearned;
}
