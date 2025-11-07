using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SkillData")]
public class SkillData : ScriptableObject
{
    /// <summary>
    /// 魔法のIDです。
    /// </summary>
    public int skillId;

    /// <summary>
    /// 魔法の名前です。
    /// </summary>
    public string skillName;

    /// <summary>
    /// 魔法の説明です。
    /// </summary>
    public string skillDesc;

    /// <summary>
    /// 魔法の消費MPです。
    /// </summary>
    public int cost;

    /// <summary>
    /// 魔法の効果リストです。
    /// </summary>
    public SkillEffect skillEffect;

    public override string ToString()
    {
        return "SkillId:" + skillId + "\nSkillName:" + skillName + "\nSkillDesc:" + skillDesc;
    }
}
