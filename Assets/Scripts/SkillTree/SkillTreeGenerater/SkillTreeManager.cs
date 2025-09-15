using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillTreeManager : MonoBehaviour
{
    [SerializeField] DataSetting dataSetting;
    [SerializeField] SkillTreeGanerate skillTreeGanerate;

    [SerializeField] Text skillPointText;//SPのテキスト
    [SerializeField] Text skillInfoText;//スキルの表示
    [SerializeField] GameObject skillBlockPanel;
    int skillPoint = 100;

    List<Node> skillList = new List<Node>();//取得済みのものを格納
    SkillBlocks[] skillBlocks;//skillBlockPanelの子オブジェクトを格納
    public static SkillTreeManager instance;
    bool onceAction;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        //skillBlocks = skillBlockPanel.GetComponentsInChildren<SkillBlocks>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Set();
    }

    /// <summary>
    /// 初期化
    /// </summary> <summary>
    /// 
    /// </summary>
    void Set()
    {
        onceAction = true;
        UpdateSkillPointText();
        UpdateSkillInfoText("");
        skillList = new List<Node>();
        skillBlocks = skillBlockPanel.GetComponentsInChildren<SkillBlocks>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) onceAction = true;
        if (!skillTreeGanerate.activeGenerate && onceAction)
        {
            Set();
            //skillList = dataSetting.nodeData;//ノードデースキルリストに格納

            foreach (int[] n in dataSetting.connections)
            {
                //Debug.Log(n[0] + "," + n[1]);
            }

            foreach (Skill n in dataSetting.nodeSkillData)
            {
                Debug.Log(n.toString());
            }
            onceAction = false;
        }
    }

    void UpdateSkillPointText()
    {
        skillPointText.text = string.Format("SP：{0}", skillPoint);
    }

    public void UpdateSkillInfoText(string text)
    {
        skillInfoText.text = text;
    }

    /// <summary>
    /// スキルが習得済みであればTrueを返す
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public bool HasSkill(int id)
    {
        foreach (Node s in skillList)
        {
            if (s.getId() == id)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 取得可能であればTrueを返す
    /// </summary>
    /// <param name="cost"></param>
    /// <param name="node"></param>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <param name="cost"></param>
    /// <param name="node"></param>
    /// <returns></returns>
    public bool CanLearnSkill(int cost, int id)
    {
        if (id == 0) return true;
        if (skillPoint < cost)
        {
            return false;
        }


        bool hasParentSkill = false;

        foreach (int[] d in dataSetting.connections)
        {
            if (id == d[1] || id == d[0])
            {
                if (HasSkill(d[0]) || HasSkill(d[1]))
                {
                    hasParentSkill = true;
                    break;
                }
            }
        }

        if (!hasParentSkill) return false;

        return true;
    }

    /// <summary>
    /// 取得する
    /// </summary>
    /// <param name="cost"></param>
    /// <param name="skillType"></param>
    public void LearnSkill(int cost, int id)
    {
        foreach (Node n in dataSetting.nodeData)
        {
            if (n.getId() == id)
            {
                skillList.Add(n);
            }
        }

        ChechActiveBlocks();
        skillPoint -= cost;
        UpdateSkillPointText();
    }

    /// <summary>
    /// 取得可能なスキルをチェックする
    /// </summary>
    void ChechActiveBlocks()
    {
        // Destroy 済みのオブジェクトを含まない最新の配列を取り直す
        skillBlocks = skillBlockPanel.GetComponentsInChildren<SkillBlocks>();
        foreach (SkillBlocks skillBlocks in skillBlocks)
        {
            skillBlocks.CheckActiveBlock();
        }
    }

}
