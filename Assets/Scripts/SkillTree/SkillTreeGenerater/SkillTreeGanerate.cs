using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class SkillTreeGanerate : MonoBehaviour
{
    [Header("物理スキルのアイコン"), SerializeField] GameObject physicsIocn;
    [Header("魔法スキルのアイコン"), SerializeField] GameObject magicIcon;
    [Header("回復スキルのアイコン"), SerializeField] GameObject healIocn;
    [Header("デデバフスキルのアイコン"), SerializeField] GameObject buffIcon;
    [Header("デバフスキルのアイコン"), SerializeField] GameObject debuffIocn;
    [Header("ステータスアップのアイコン"), SerializeField] GameObject statusIcon;
    [Header("はじめのアイコン"), SerializeField] GameObject startIcon;
    [SerializeField] GameObject line;
    [SerializeField] DataSetting dataSetting;

    [SerializeField] Transform skillBlocksPanel;
    [SerializeField] Transform linesPanel;
    public bool activeGenerate;//生成中？
    [SerializeField] int maxRetry = 100;
    int retry = 0;

    public int sum_sp = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sum_sp = 0;
        dataSetting.DataSet();
        View();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            sum_sp = 0;
            dataSetting.Reset();
            View();
        }
    }

    void View()
    {
        retry = 0;

        activeGenerate = true;

        do
        {
            dataSetting.Set();
            retry++;
            if (retry > maxRetry)
            {
                Debug.Log("警告: 入力0ノードが消せませんでした");
                break;
            }
        } while (!hasNodeWithZeroInput());


        dataSetting.TagSet();
        dataSetting.SkillDataSet();
        dataSetting.StatusDataSet();

        DrawNodes();//ノードの表示
        DrawLines();//ノード間の線の描写

        foreach (int[] c in dataSetting.connections)
        {
            //Debug.Log(c[0] + "," + c[1]);
        }

        activeGenerate = false;
        Debug.Log($"新しいスキルツリー {retry}回再生成を行った");
    }

    /// <summary>
    /// ノードの生成
    /// </summary>
    void DrawNodes()
    {
        int id = 0;
        Dictionary<int, string> tagData = dataSetting.getTagData();//スキル・ステータスのタグを格納

        foreach (Node n in dataSetting.nodeData)
        {
            GameObject prefab = startIcon;


            if (tagData[n.getId()] == "スキル")
            {
                foreach (var data in dataSetting.nodeSkillData)
                {
                    if (n.getId().Equals(data.GetId()))
                    {
                        Debug.Log($"{n.getId()},{data.GetId()},{data.GetAction()}");
                        if (data.GetAction().Equals("物理攻撃"))
                        {
                            prefab = physicsIocn;
                        }
                        else if (data.GetAction().Equals("特殊攻撃") || data.GetAction().Equals("魔法攻撃"))
                        {
                            prefab = magicIcon;
                        }
                        else if (data.GetAction().Equals("回復") || data.GetAction().Equals("復活"))
                        {
                            prefab = healIocn;
                        }
                        else if (data.GetAction().Equals("強化"))
                        {
                            prefab = buffIcon;
                        }
                        else if (data.GetAction().Equals("弱体"))
                        {
                            prefab = debuffIocn;
                        }
                    }
                    sum_sp += data.GetSp();
                }
            }
            else if (tagData[n.getId()] == "ステータス")
            {
                prefab = statusIcon;
                sum_sp += dataSetting.statusSp;
            }
            else
            {
                prefab = startIcon;
            }

            // Canvas の子として生成
            GameObject obj = Instantiate(prefab, skillBlocksPanel);
            obj.name = $"ID:{id++}";//名前の付与

            // UI の座標設定
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(n.getX(), n.getY());
        }
    }

    /// <summary>
    /// ラインの生成
    /// </summary>
    void DrawLines()
    {
        foreach (int[] c in dataSetting.connections)
        {
            float posX = 0f;
            float posY = 0f;
            float angle = getAngle(c[0], c[1]);

            foreach (Node n in dataSetting.nodeData)
            {
                float[] dist = checkNodeDist(c[0], c[1]);
                if (n.getId().Equals(c[0]))
                {
                    posX = n.getX() + (dist[0] / 2.0f);
                    posY = n.getY() + (dist[1] / 2.0f);
                }
            }

            // Canvas の子として生成
            GameObject obj = Instantiate(line, linesPanel);

            obj.name = $"{c[0]}と{c[1]}間のライン";//名前の付与

            // UI の座標設定
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(posX, posY);
            rect.localEulerAngles = new Vector3(0, 0, angle);
            rect.localScale = new Vector3(1, 1, 0);
        }
    }

    /// <summary>
    /// ノード間の距離（配列（X座標,Y座標））を取得
    /// </summary>
    /// <param name="beforeID"></param>
    /// <param name="afterID"></param>
    /// <returns></returns>
    float[] checkNodeDist(int beforeID, int afterID)
    {
        float[] dist = { 0, 0 };
        float beforePosX = 0;
        float afterPosX = 0;
        float beforePosY = 0;
        float afterPosY = 0;


        foreach (Node n in dataSetting.nodeData)
        {
            if (n.getId().Equals(beforeID))
            {
                beforePosX = n.getX();
                beforePosY = n.getY();
            }

            if (n.getId().Equals(afterID))
            {
                afterPosX = n.getX();
                afterPosY = n.getY();
            }
        }

        dist[0] = afterPosX - beforePosX;
        dist[1] = afterPosY - beforePosY;

        return dist;
    }

    /// <summary>
    /// 2点間の角度の取得
    /// </summary>
    /// <param name="beforeID"></param>
    /// <param name="afterID"></param>
    /// <returns></returns>
    float getAngle(int beforeID, int afterID)
    {
        float[] dist = checkNodeDist(beforeID, afterID);

        float rad = Mathf.Atan2(dist[1], dist[0]);
        return rad * Mathf.Rad2Deg;
    }

    /// <summary>
    /// エッジの数が0のものがあるかないか返す
    /// </summary>
    /// <returns></returns>
    bool hasNodeWithZeroInput()
    {
        List<int> endList = new List<int>();
        foreach (int[] pair in dataSetting.connections)
        {
            //Debug.Log(pair[0] + "," + pair[1]);
            endList.Add(pair[1]);
        }

        endList.Sort();
        //Debug.Log(dataSetting.getNodeSum());

        // foreach (int list in endList)
        // {
        //     Debug.Log(list);
        // }

        for (int i = 1; i < dataSetting.getNodeSum(); i++)
        {
            if (!endList.Contains(i))
            {
                //Debug.Log(i + " はリストに含まれています");
            }
            else
            {
                //Debug.Log(i + " はリストに含まれていません");
                return true;
            }
        }

        return false;
    }
}
