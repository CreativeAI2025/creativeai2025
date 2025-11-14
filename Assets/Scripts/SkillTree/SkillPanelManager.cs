using System.Collections.Generic;
using UnityEngine;

public class SkillPanelManager : MonoBehaviour
{
    [SerializeField] GameObject panel0;
    [SerializeField] GameObject panel1;
    [SerializeField] GameObject panel2;

    [SerializeField] List<SkillTreeManager> skillTreeManagers = new List<SkillTreeManager>();
    [SerializeField] List<SkillTreeManager1> skillTreeManagers1 = new List<SkillTreeManager1>();

    void Start()
    {

    }

    public void SkillPanel0()
    {
        panel0.SetActive(true);
        panel1.SetActive(false);
        panel2.SetActive(false);
        UpdateSkillPointText(0);
    }
    public void SkillPanel1()
    {
        panel0.SetActive(false);
        panel1.SetActive(true);
        panel2.SetActive(false);
        UpdateSkillPointText(1);
    }
    public void SkillPanel2()
    {
        panel0.SetActive(false);
        panel1.SetActive(false);
        panel2.SetActive(true);
        UpdateSkillPointText(2);
    }

    void UpdateSkillPointText(int num)
    {
        skillTreeManagers[num].UpdateSkillPointText();
        skillTreeManagers1[num].UpdateSkillPointText();
    }
}
