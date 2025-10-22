using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillTreeManager : MonoBehaviour
{
    [SerializeField] DataSetting dataSetting;
    [SerializeField] SkillTreeGanerate skillTreeGanerate;
    [SerializeField] ParameterTable parameterTable;

    [SerializeField] TextMeshProUGUI skillPointText;//SPのテキスト
    [SerializeField] TextMeshProUGUI skillInfoText;//スキルの表示
    [SerializeField] GameObject skillBlockPanel;
    [SerializeField] int skillPoint = 1000;

    List<Node> skillList = new List<Node>();//取得済みのものを格納
    List<Skill> nodeSkillList = new List<Skill>();
    List<Status> nodeStatusList = new List<Status>();
    SkillBlocks[] skillBlocks;//skillBlockPanelの子オブジェクトを格納

    ParameterRecord startStatus;

    bool onceAction;

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
        UpdateSkillInfoText(0, true);
        skillList = new List<Node>();
        skillBlocks = skillBlockPanel.GetComponentsInChildren<SkillBlocks>();
        startStatus = parameterTable.parameterRecords[0];
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

            foreach (Status n in dataSetting.nodeStatusData)
            {
                nodeStatusList.Add(n);
            }

            foreach (Skill n in dataSetting.nodeSkillData)
            {
                SkillStatusLoader.instance.LoadSkills();
                SkillStatusLoader.instance.LoadStatuses();
                SkillEntry skillEntry = null;
                Skill skill = null;

                //Jsonファイルのスキル所得状況の更新
                foreach (Skill d in nodeSkillList)
                {
                    if (d.GetId().Equals(n.GetId()))
                    {
                        skill = d;
                    }
                }


                if (skill != null) skillEntry = System.Array.Find(SkillStatusLoader.instance.GetSkillEntryList().skills, s => s.name == skill.GetName());

                // 獲得したステータスアップをカウント
                foreach (Skill d in nodeSkillList)
                {
                    if (d.GetId() == n.GetId())
                    {
                        if (skillEntry != null) skillEntry.mp = d.GetMp();//取得状況の変更
                    }
                }
            }

            SkillStatusLoader.instance.SaveSkillData();// スキルのJSONに保存
            Debug.Log("保存先: " + Application.persistentDataPath);
            onceAction = false;
        }
    }

    void FixedUpdate()
    {
        UpdateSkillPointText();
    }

    /// <summary>
    /// スキルポイントの更新
    /// </summary>
    public void UpdateSkillPointText()
    {
        skillPointText.text = string.Format("SP：{0}", skillPoint);
    }

    /// <summary>
    /// スキル・ステータスの説明をテキストに代入(id,説明を表示するかしないか)
    /// </summary>
    /// <param name="id"></param>
    public void UpdateSkillInfoText(int id, bool canLearned, string newInfo = null)
    {
        string text = "データがありません";
        if (canLearned)
        {
            if (id == 0)
            {
                text = "最初のパネルを押してスキルツリーを広げていこう‼";
            }

            foreach (var n in dataSetting.nodeSkillData)
            {
                if (n.GetId().Equals(id))
                {
                    text = "スキル:" + n.GetName() + newInfo + "\n" + n.GetExplain() + "\n必要SP:" + n.GetSp() + " 必要MP:" + n.GetMp();
                }
            }


            foreach (var n in dataSetting.nodeStatusData)
            {
                if (n.getId().Equals(id))
                {
                    text = n.getExplain() + newInfo + "\n必要SP:" + n.GetSp();
                    Debug.Log(n.ToString());
                }
            }
        }
        else
        {
            text = "?";
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
    /// <returns></returns>
    public bool CanLearnSkill(int cost, int id)
    {
        //Debug.Log($"{skillPoint},{cost}");
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
        SkillStatusLoader.instance.LoadSkills();
        SkillStatusLoader.instance.LoadStatuses();

        // Debug.Log("=== LearnSkill Debug ===");
        // Debug.Log("SkillStatusLoader.instance: " + (SkillStatusLoader.instance == null ? "null" : "OK"));

        // if (SkillStatusLoader.instance != null)
        // {
        //     Debug.Log("SkillStatusLoader.instance.GetStatusEntryList(): " + (SkillStatusLoader.instance.GetStatusEntryList() == null ? "null" : "OK"));

        //     if (SkillStatusLoader.instance.GetStatusEntryList() != null)
        //     {
        //         Debug.Log("statuses: " + (SkillStatusLoader.instance.GetStatusEntryList().statuses == null ? "null" : "OK"));
        //         Debug.Log("statuses.Length: " + (SkillStatusLoader.instance.GetStatusEntryList().statuses == null ? "N/A" : SkillStatusLoader.instance.GetStatusEntryList().statuses.Length.ToString()));
        //     }
        // }

        SkillEntry skillEntry = null;
        Skill skill = null;
        StatusEntry statusEntry = null;
        Status status = null;

        //Jsonファイルのスキル所得状況の更新
        foreach (Skill d in nodeSkillList)
        {
            if (d.GetId().Equals(id))
            {
                skill = d;
            }
        }


        if (skill != null) skillEntry = System.Array.Find(SkillStatusLoader.instance.GetSkillEntryList().skills, s => s.name == skill.GetName());

        //Jsonファイルのスキル所得状況の更新
        foreach (Status d in nodeStatusList)
        {
            if (d.getId().Equals(id))
            {
                status = d;
            }
        }

        if (status != null)
        {
            foreach (var s in SkillStatusLoader.instance.GetStatusEntryList().statuses)
            {
                if (s.name == status.getName())
                {
                    statusEntry = s;
                    break; // 見つかったら抜ける
                }
            }
        }

        // 獲得したステータスアップをカウント
        foreach (Node n in dataSetting.nodeData)
        {
            if (n.getId() == id)
            {
                if (skillEntry != null) skillEntry.get = true;//取得状況の変更
                if (statusEntry != null)
                {
                    statusEntry.count++;//取得状況の変更
                    switch (status.GetType())//ステータス更新
                    {
                        case "攻撃力":
                            parameterTable.parameterRecords[0].Attack = BeforeStatus(startStatus.Attack, status.GetPower(), statusEntry.count);
                            break;
                        case "防御力":
                            parameterTable.parameterRecords[0].Defence = BeforeStatus(startStatus.Defence, status.GetPower(), statusEntry.count);
                            break;
                        case "魔法力":
                            parameterTable.parameterRecords[0].MagicAttack = BeforeStatus(startStatus.MagicAttack, status.GetPower(), statusEntry.count);
                            break;
                        case "魔法防御力":
                            parameterTable.parameterRecords[0].MagicDefence = BeforeStatus(startStatus.MagicDefence, status.GetPower(), statusEntry.count);
                            break;
                        case "素早さ":
                            parameterTable.parameterRecords[0].Speed = BeforeStatus(startStatus.Speed, status.GetPower(), statusEntry.count);
                            break;
                        case "回避率":
                            parameterTable.parameterRecords[0].Evasion = BeforeStatus(startStatus.Evasion, status.GetPower(), statusEntry.count);
                            break;
                        case "最大HP":
                            parameterTable.parameterRecords[0].HP = BeforeStatus(startStatus.HP, status.GetPower(), statusEntry.count);
                            break;
                        case "最大MP":
                            parameterTable.parameterRecords[0].MP = BeforeStatus(startStatus.MP, status.GetPower(), statusEntry.count);
                            break;
                        default:
                            break;
                    }
                }
                skillList.Add(n);
            }
        }

        skillPoint -= cost;

        ChechActiveBlocks();
        UpdateSkillPointText();
        SkillStatusLoader.instance.SaveSkillData();// スキルのJSONに保存
        SkillStatusLoader.instance.SaveStatusData();// ステータスのJSONに保存
        Debug.Log("保存先: " + Application.persistentDataPath);
    }

    /// <summary>
    /// ステータスアップ後のステータスを返す(引数：初めのステータス、上昇率、アップ回数)
    /// </summary>
    /// <returns></returns>
    int BeforeStatus(int startStatus, int power, int count)
    {
        int status = 0;
        for (int i = 0; i < count; i++)
        {
            if (i == 0) status = startStatus + (startStatus * (power / 100));
            status = status + (status * (power / 100));
        }

        return status;
    }

    /// <summary>
    /// スキルツリーマネージャーからIDに対応するSPを受け取る
    /// </summary>
    /// <returns></returns>
    public int GetMySp(int id)
    {
        int sp = 0;
        //Jsonファイルのスキル所得状況の更新
        foreach (Skill d in nodeSkillList)
        {
            if (d.GetId().Equals(id))
            {
                sp = d.GetSp();
            }
        }

        //Jsonファイルのスキル所得状況の更新
        foreach (Status d in nodeStatusList)
        {
            if (d.getId().Equals(id))
            {
                sp = d.GetSp();
            }
        }
        return sp;
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