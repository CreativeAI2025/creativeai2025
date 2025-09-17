using UnityEngine;
using System.IO;

// JSONデータのためのクラス
[System.Serializable]
public class SkillEntry
{
    public string name;
    public string explain;
    public bool get;
}

// JSON全体をまとめる型
[System.Serializable]
public class SkillEntryList
{
    public SkillEntry[] skills;
}

// JSONデータのためのクラス
[System.Serializable]
public class StatusEntry
{
    public string name;
    public string explain;
}

// JSON全体をまとめる型
[System.Serializable]
public class StatusEntryList
{
    public StatusEntry[] statuses;
}

public class SkillLoader : MonoBehaviour
{
    public static SkillLoader instance;
    [SerializeField] public TextAsset skillDataJson;//Json読み込み専用
    private SkillEntryList skillList;//Json読み込み用リスト
    private string savePath;
    public SkillEntryList getSkillEntryList()
    {
        return this.skillList;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    /// <summary>
    /// スキルJsonファイルの読み込み
    /// </summary>
    public void LoadSkills()
    {
        savePath = Application.persistentDataPath + "/skillData.json";

        if (System.IO.File.Exists(savePath))
        {
            // 保存済みデータを読む
            string json = System.IO.File.ReadAllText(savePath);
            skillList = JsonUtility.FromJson<SkillEntryList>(json);
            Debug.Log("保存済みスキルデータをロードしました");
        }
        else
        {
            // 初回は TextAsset から読む
            skillList = JsonUtility.FromJson<SkillEntryList>(skillDataJson.text);
            Debug.Log("初期スキルデータをロードしました");
        }
    }

    /// <summary>
    /// JSON に保存
    /// </summary>
    public void SaveSkillData()
    {
        string json = JsonUtility.ToJson(skillList, true); // true = インデントつき
        string path = Application.persistentDataPath + "/skillData.json";
        File.WriteAllText(path, json);

        Debug.Log("スキルデータを保存しました: " + path);
    }

    /// <summary>
    /// Json(Application.persistentDataPath)の初期化
    /// </summary>
    public void ResetSkillJsonFile()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath); // 保存済みデータを削除
        }
        skillList = JsonUtility.FromJson<SkillEntryList>(skillDataJson.text); // 初期データを読み込み
        SaveSkillData(); // 再保存
        Debug.Log("スキルデータを初期化しました");
    }
}
