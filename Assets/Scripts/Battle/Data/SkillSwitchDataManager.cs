using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class SkillSwitchDataManager : DontDestroySingleton<SkillSwitchDataManager>
{
    private List<SkillSwitchDataManager> SkillSwitchDataList;
    private string filePath;
    public override void Awake()
    {
        base.Awake();
        filePath = Path.Combine(Application.persistentDataPath, "SkillSwitchData.json");
        print(filePath);
    }

    public void SaveSkillSwitchData()
    {
        
    }
}
