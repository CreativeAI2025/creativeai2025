using UnityEngine;
using System.Collections.Generic;

public class SkillTreeGanerate : MonoBehaviour
{
    [SerializeField] GameObject icon;
    [SerializeField] GameObject icon1;
    [SerializeField] GameObject icon2;
    [SerializeField] DataSetting dataSetting;

    [SerializeField] Transform parentTransform;
    public bool activeGenerate;

    // int maxRetry = 0;
    // int retry = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dataSetting.DataSet();
        View();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            dataSetting.reset();
            View();
        }
    }

    void View()
    {
        activeGenerate = true;
        // maxRetry = 100;
        // retry = 0;

        dataSetting.set();
        dataSetting.TagSet();
        dataSetting.SkillDataSet();

        DrawNodes();//ノードの表示
        activeGenerate = false;
    }

    void DrawNodes()
    {
        int id = 0;
        Dictionary<int, string> tagData = dataSetting.getTagData();

        foreach (Node n in DataSetting.nodeData)
        {
            GameObject prefab;

            if (tagData[n.getId()] == "スキル")
            {
                prefab = icon;
            }
            else if (tagData[n.getId()] == "ステータス")
            {
                prefab = icon1;
            }
            else
            {
                prefab = icon2;
            }

            // Canvas の子として生成
            GameObject obj = Instantiate(prefab, parentTransform);
            obj.name = $"ID:{id++}";//名前の付与

            // UI の座標設定
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(n.getX(), n.getY());
        }
    }
}
