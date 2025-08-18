using UnityEngine;

public class SkillTreeGanerate : MonoBehaviour
{
    [SerializeField] GameObject icon;
    [SerializeField] DataSetting dataSetting;

    // int maxRetry = 0;
    // int retry = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        View();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void View() {
        // maxRetry = 100;
        // retry = 0;

        dataSetting.set();
        dataSetting.TagSet();
        dataSetting.SkillDataSet();

        DrawNodes();//ノードの表示
    }

    void DrawNodes() {
        foreach (Node n in DataSetting.nodeData) {

            // if (n.getId() == 0) fill(255, 255, 0);
            // if (n.getId() != 0){
            // if(tagData.get(n.getId()) == "スキル"){
            //     fill(255,0,0);
            // }else if(tagData.get(n.getId()) == "ステータス"){
            //     fill(0,0,255);
            // }else{
            //     fill(0);
            // }
            // }

            //ellipse(n.getX(), n.getY(), cellSize / 1.5, cellSize / 1.5);
            //Debug.Log(n);
            //Debug.Log(n.getX() + "," + n.getY());
            Instantiate(icon,new Vector3(n.getX(), n.getY(),0.0f),Quaternion.identity);
        }
    }
}
