using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class SkillBlocks1 : MonoBehaviour
{
    [Header("iconの親"), SerializeField] GameObject parent;
    [Header("アイコンフレーム(取得済み)"), SerializeField] GameObject hidePanel;
    [Header("アイコンフレーム(未取得(習得可能))"), SerializeField] GameObject hidePanel1;
    private SkillTreeManager1 skillTreeManager1;

    string my_parent_name;//親の名前
    int id;//自分自身のID
    int cost;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 親から SkillTreeManager1 を探す
        skillTreeManager1 = GetComponentInParent<SkillTreeManager1>();
        my_parent_name = parent.gameObject.name;
        id = int.Parse(Regex.Replace(my_parent_name, @"[^0-9]", ""));
        CheckActiveBlock();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //Debug.Log(this_name + "が消えました");
            Destroy(parent.gameObject);
        }
    }

    /// <summary>
    /// アイコンを押したときの処理
    /// </summary>
    public void OnClick()
    {
        cost = skillTreeManager1.GetMySp(id);
        Debug.Log("ID:" + id + "が押されました");
        //　習得済なら何もしない
        if (skillTreeManager1.HasSkill(id))
        {
            SoundManager.Instance.PlaySE(4, 0.5f);
            skillTreeManager1.UpdateSkillInfoText(id, true, "(習得済み)");
            Debug.Log("ID:" + id + "習得済");
            return;
        }

        // 習得可能？
        if (skillTreeManager1.CanLearnSkill(cost, id))
        {
            SoundManager.Instance.PlaySE(3, 0.5f);
            // 習得可能なら習得する
            skillTreeManager1.LearnSkill(cost, id);
            skillTreeManager1.UpdateSkillInfoText(id, true, "(習得済み)");
            hidePanel.SetActive(true);
            hidePanel1.SetActive(false);
            Debug.Log("ID:" + id + "習得");
            //CangeLearnBlock(Color.blue);
        }
        else
        {
            SoundManager.Instance.PlaySE(4, 0.5f);
            // 習得不可能ならログを出す
            Debug.Log("ID:" + id + "習得できません");
        }
    }

    /// <summary>
    /// 習得可能であればアイコンを隠さない
    /// </summary>
    public void CheckActiveBlock()
    {
        cost = skillTreeManager1.GetMySp(id);

        if (skillTreeManager1.HasSkill(id))
        {
            hidePanel.SetActive(true);
            hidePanel1.SetActive(false);
            CangeLearnBlock(Color.white);
        }
        // 習得可能？
        if (skillTreeManager1.CanLearnSkill(cost, id) && !skillTreeManager1.HasSkill(id))
        {
            //Debug.Log("ID:" + id + "はFALSE");
            hidePanel1.SetActive(true);
            CangeLearnBlock(Color.white);
        }
        else if (!skillTreeManager1.HasSkill(id))
        {
            //Debug.Log("ID:" + id + "はTRUE");
            //hidePanel.SetActive(true);
            CangeLearnBlock(Color.gray);
            hidePanel1.SetActive(false);
        }
    }

    /// <summary>
    /// 取得したものの色を変える
    /// </summary>
    /// <param name="color"></param>
    void CangeLearnBlock(Color color)
    {
        Image image = GetComponent<Image>();
        image.color = color;
    }

    public void OnCursor()
    {
        skillTreeManager1.UpdateSkillInfoText(id, true);
        if (skillTreeManager1.HasSkill(id))
        {
            skillTreeManager1.UpdateSkillInfoText(id, true, "(習得済み)");
        }
    }

    public void OnCursorExsist()
    {
        skillTreeManager1.UpdateSkillInfoText(id, false);
    }
}