using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SkillType
{
    STONETHROWING,
    BEAM,
    SPEAR,
    DRAGON,
    WHIP,
    MONSTER,
}
public class SkillManager : MonoBehaviour
{

    [SerializeField] Text skillPointText;
    [SerializeField] Text skillInfoText;
    [SerializeField] GameObject skillBlockPanel;
    int skillPoint = 100;

    [SerializeField] List<SkillType> skillList = new List<SkillType>();
    SkillBlock[] skillBlocks;

    public static SkillManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        skillBlocks = skillBlockPanel.GetComponentsInChildren<SkillBlock>();
    }
    void Start()
    {
        UpdateSkillPointText();
        UpdateSkillInfoText("");
    }

    void UpdateSkillPointText()
    {
        skillPointText.text = string.Format("消費可能ポイント：{0}", skillPoint);
    }

    public void UpdateSkillInfoText(string text)
    {
        skillInfoText.text = text;
    }

    public bool HasSkill(SkillType skillType)
    {
        return skillList.Contains(skillType);//スキルが習得済みであればTrueを返す
    }

    public bool CanLearnSkill(int cost, SkillType skillType)
    {
        if (skillPoint < cost)
        {
            return false;
        }

        if (skillType == SkillType.SPEAR)
        {
            return HasSkill(SkillType.STONETHROWING) && HasSkill(SkillType.BEAM);
        }

        if (skillType == SkillType.DRAGON)
        {
            return HasSkill(SkillType.SPEAR);
        }

        if (skillType == SkillType.MONSTER)
        {
            return HasSkill(SkillType.WHIP);
        }

        return true;
    }

    public void LearnSkill(int cost, SkillType skillType)
    {
        skillList.Add(skillType);
        ChechActiveBlocks();
        skillPoint -= cost;
        UpdateSkillPointText();
    }

    void ChechActiveBlocks()
    {
        foreach (SkillBlock skillBlock in skillBlocks)
        {
            skillBlock.CheckActiveBlock();
        }
    }
}
