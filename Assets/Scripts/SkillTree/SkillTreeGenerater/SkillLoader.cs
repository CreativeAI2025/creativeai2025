using UnityEngine;
using System.IO;
using System.Collections.Generic;

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

[System.Serializable]
public class SkillJsonFile
{
    [Header("キャラクターの名前")] public string characterName;
    [Header("キャラクターが取得できるスキルの元データ")] public TextAsset textAsset;
    [Header("キャラクターの取得したスキルの保存ファイル名")] public string saveFileName;

    public string GetcharacterName()
    {
        return this.characterName;
    }

    public TextAsset GetTextAsset()
    {
        return this.textAsset;
    }

    public string GetSaveFileName()
    {
        return "/" + this.saveFileName + ".json";
    }
}

public class SkillLoader : MonoBehaviour
{
    public static SkillLoader instance;
    //[SerializeField] public TextAsset skillDataJson;//Json読み込み専用
    [SerializeField] List<SkillJsonFile> skillJsonFiles = new List<SkillJsonFile>();
    private SkillEntryList skillList;//Json読み込み用リスト
    private string savePath;
    public SkillEntryList GetSkillEntryList()
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
        foreach (var file in skillJsonFiles)
        {
            savePath = Application.persistentDataPath + file.GetSaveFileName();//保存ファイル名

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
                skillList = JsonUtility.FromJson<SkillEntryList>(file.GetTextAsset().text);
                Debug.Log("初期スキルデータをロードしました");
            }
        }
    }

    /// <summary>
    /// JSON に保存
    /// </summary>
    public void SaveSkillData()
    {
        foreach (var file in skillJsonFiles)
        {
            string json = JsonUtility.ToJson(skillList, true); // true = インデントつき
            string path = Application.persistentDataPath + file.GetSaveFileName();
            File.WriteAllText(path, json);

            //Debug.Log("スキルデータを保存しました: " + path);
        }
    }

    /// <summary>
    /// Json(Application.persistentDataPath)の初期化
    /// </summary>
    public void ResetSkillJsonFile()
    {
        foreach (SkillJsonFile file in skillJsonFiles)
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath); // 保存済みデータを削除
            }
            skillList = JsonUtility.FromJson<SkillEntryList>(file.GetTextAsset().text); // 初期データを読み込み
            SaveSkillData(); // 再保存
        }
        Debug.Log("すべてのスキルデータを初期化しました");
    }

    /// <summary>
    /// 引数で指定したキャラクターのJsonファイルを返す
    /// </summary>
    /// <param name="characterName"></param>
    /// <returns></returns>
    public TextAsset GetSkillJsonFile(string characterName)
    {
        TextAsset textAsset = null;

        foreach (SkillJsonFile file in skillJsonFiles)
        {
            if (file.GetcharacterName().Equals(characterName))
            {
                textAsset = file.GetTextAsset();
            }
        }

        if (textAsset == null) Debug.LogError("指定したキャラクターのJsonFileがありません"); ;

        return textAsset;
    }
}
