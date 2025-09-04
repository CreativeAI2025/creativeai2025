using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    [SerializeField] DataSetting dataSetting;
    [SerializeField] SkillTreeGanerate skillTreeGanerate;
    bool onceAction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        onceAction = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) onceAction = true;
        if (!skillTreeGanerate.activeGenerate && onceAction)
        {
            foreach (int[] n in dataSetting.connections)
            {
                Debug.Log(n[0] + "," + n[1]);
            }
            onceAction = false;
        }
    }
}
