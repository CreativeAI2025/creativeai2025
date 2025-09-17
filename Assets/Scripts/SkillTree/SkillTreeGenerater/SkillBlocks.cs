using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class SkillBlocks : MonoBehaviour
{
    [SerializeField] GameObject hidePanel;

    private SkillTreeManager skillTreeManager;

    string this_name;//自分自身の名前
    int id;//自分自身のID
    string info = "説明";
    int cost = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 親から SkillTreeManager を探す
        skillTreeManager = GetComponentInParent<SkillTreeManager>();
        this_name = this.gameObject.name;
        id = int.Parse(Regex.Replace(this_name, @"[^0-9]", ""));
        CheckActiveBlock();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //Debug.Log(this_name + "が消えました");
            Destroy(this.gameObject);
        }
    }

    public void OnClick()
    {
        Debug.Log("ID:" + id + "が押されました");
        //　習得済なら何もしない
        if (skillTreeManager.HasSkill(id))
        {
            //bug.Log("ID:" + id + "習得済");
            return;
        }

        // 習得可能？
        if (skillTreeManager.CanLearnSkill(cost, id))
        {
            // 習得可能なら習得する
            skillTreeManager.LearnSkill(cost, id);
            //bug.Log("ID:" + id + "習得");
            CangeLearnBlock(Color.blue);
        }
        else
        {
            // 習得不可能ならログを出す
            //bug.Log("ID:" + id + "習得できません");
        }
    }

    public void CheckActiveBlock()
    {

        // 習得可能？
        if (skillTreeManager.CanLearnSkill(cost, id))
        {
            //Debug.Log("ID:" + id + "はFALSE");
            hidePanel.SetActive(false);
        }
        else
        {
            //Debug.Log("ID:" + id + "はTRUE");
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
        skillTreeManager.UpdateSkillInfoText(id);
    }
}
