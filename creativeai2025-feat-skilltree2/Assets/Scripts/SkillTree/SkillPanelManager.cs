using System.Collections.Generic;
using UnityEngine;

public class SkillPanelManager : MonoBehaviour
{
    [SerializeField] GameObject panel0;
    [SerializeField] GameObject panel1;
    [SerializeField] GameObject panel2;

    [SerializeField] List<SkillTreeManager> skillTreeManagers = new List<SkillTreeManager>();
    [SerializeField] List<SkillTreeGanerate> skillTreeGanerates;
    void Start()
    {
        ShowPanel(0);
    }

    public void SkillPanel0() => ShowPanel(0);
    public void SkillPanel1() => ShowPanel(1);
    public void SkillPanel2() => ShowPanel(2);

    void ShowPanel(int index)
    {
        // すべて非表示にする
        panel0.SetActive(false);
        panel1.SetActive(false);
        panel2.SetActive(false);

        // 指定パネルだけON
        switch (index)
        {
            case 0: panel0.SetActive(true); break;
            case 1: panel1.SetActive(true); break;
            case 2: panel2.SetActive(true); break;
        }

        // 表示されたパネルのSkillTreeManagerだけに再描画命令
        for (int i = 0; i < skillTreeManagers.Count; i++)
        {
            if (i == index)
            {
                skillTreeManagers[i].gameObject.SetActive(true);
                skillTreeGanerates[i].RedrawLinesSafe();
            }
            else
            {
                skillTreeManagers[i].gameObject.SetActive(false);
            }
        }

        // スキルポイント更新
        UpdateSkillPointText(index);
    }

    void UpdateSkillPointText(int num)
    {
        skillTreeManagers[num].UpdateSkillPointText();
    }
}
