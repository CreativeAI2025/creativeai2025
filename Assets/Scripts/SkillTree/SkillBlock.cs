using UnityEngine;
using UnityEngine.UI;

public class SkillBlock : MonoBehaviour
{
    // [SerializeField] Text nameText;
    // [SerializeField] Text infoText;
    [SerializeField] SkillType skillType;
    [SerializeField] int cost;
    [SerializeField] new string name;
    [SerializeField] string info;
    [SerializeField] GameObject hidePanel;

    void Start()
    {
        // infoText.text = info;
        // nameText.text = name;
        CheckActiveBlock();
    }

    public void OnClick()
    {
        //　習得済なら何もしない
        if (SkillManager.instance.HasSkill(this.skillType))
        {
            Debug.Log("習得済");
            return;
        }

        // 習得可能？
        if (SkillManager.instance.CanLearnSkill(cost, skillType))
        {
            // 習得可能なら習得する
            SkillManager.instance.LearnSkill(cost, this.skillType);
            Debug.Log("習得");
            CangeLearnBlock(Color.blue);
        }
        else
        {
            // 習得不可能ならログを出す
            Debug.Log("習得できません");
        }
    }

    public void CheckActiveBlock()
    {
        // 習得可能？
        if (SkillManager.instance.CanLearnSkill(cost, skillType))
        {
            hidePanel.SetActive(false);
        }
        else
        {
            hidePanel.SetActive(true);
        }
    }

    void CangeLearnBlock(Color color)
    {
        Image image = GetComponent<Image>();
        image.color = color;
    }

    public void OnCursor()
    {
        SkillManager.instance.UpdateSkillInfoText(info);
    }
}
