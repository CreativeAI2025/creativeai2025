using UnityEngine;

public class SkillPanelManager : MonoBehaviour
{
    [SerializeField] GameObject panel0;
    [SerializeField] GameObject panel1;
    [SerializeField] GameObject panel2;

    void Start()
    {
        SkillPanel0();
    }

    public void SkillPanel0()
    {
        panel0.SetActive(true);
        panel1.SetActive(false);
        panel2.SetActive(false);
    }
    public void SkillPanel1()
    {
        panel0.SetActive(false);
        panel1.SetActive(true);
        panel2.SetActive(false);
    }
    public void SkillPanel2()
    {
        panel0.SetActive(false);
        panel1.SetActive(false);
        panel2.SetActive(true);
    }
}
