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
    List<Skill> nodeSkillList = new List<Skill>();
    SkillBlocks[] skillBlocks;//skillBlockPanelの子オブジェクトを格納
    public static SkillTreeManager instance;
    bool onceAction;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Set();
    }

    /// <summary>
    /// 初期化
    /// </summary> <summary>
    void Set()
    {
        onceAction = true;
        UpdateSkillPointText();
        UpdateSkillInfoText(0);
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
                nodeSkillList.Add(n);
            }
            onceAction = false;
        }
    }

    /// <summary>
    /// スキルポイントの更新
    /// </summary>
    void UpdateSkillPointText()
    {
        skillPointText.text = string.Format("SP：{0}", skillPoint);
    }

    /// <summary>
    /// スキル・ステータスの説明をテキストに代入
    /// </summary>
    /// <param name="id"></param>
    public void UpdateSkillInfoText(int id)
    {
        string text = "データがありません";
        if (id == 0)
        {
            text = "最初のパネルを押してスキルツリーを広げていこう‼";
        }

        foreach (var n in dataSetting.nodeSkillData)
        {
            if (n.getId().Equals(id))
            {
                text = n.toSkillstring(n.getType());
            }
        }

        foreach (var n in dataSetting.nodeStatusData)
        {
            if (n.getId().Equals(id))
            {
                text = n.getExplain();
            }
        }
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
        SkillEntry skillEntry = null;
        Skill skill = null;

        dataSetting.LoadSkills();

        //Jsonファイルのスキル所得状況の更新
        foreach (Skill d in nodeSkillList)
        {
            if (d.getId().Equals(id))
            {
                skill = d;
            }
        }


        if (skill != null) skillEntry = System.Array.Find(dataSetting.getSkillEntryList().skills, s => s.name == skill.getName());

        // 獲得したスキルをskillListに追加
        foreach (Node n in dataSetting.nodeData)
        {
            if (n.getId() == id)
            {
                if (skillEntry != null) skillEntry.get = true;
                skillList.Add(n);
            }
        }

        ChechActiveBlocks();
        skillPoint -= cost;
        UpdateSkillPointText();
        dataSetting.SaveSkillData();// JSON に保存
        Debug.Log("保存先: " + Application.persistentDataPath);

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
