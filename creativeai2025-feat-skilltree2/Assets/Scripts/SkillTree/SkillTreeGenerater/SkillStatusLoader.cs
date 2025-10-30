using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEditor.Overlays;

//SP・MPはスキルツリーシーンを開くと適切な値がセットされます（現時点）

// JSONデータのためのクラス
[System.Serializable]
public class SkillEntry
{
    public string name;
    public string explain;
    public bool get;
    public int mp;
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
    public int count;
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

[System.Serializable]
public class StatusJsonFile
{
    [Header("キャラクターの名前")] public string characterName;
    [Header("キャラクターが取得できるステータスの元データ")] public TextAsset textAsset;
    [Header("キャラクターの取得したステータスの保存ファイル名")] public string saveFileName;

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

public class SkillStatusLoader : MonoBehaviour
{
    public static SkillStatusLoader instance;
    //[SerializeField] public TextAsset skillDataJson;//Json読み込み専用
    [SerializeField] List<SkillJsonFile> skillJsonFiles = new List<SkillJsonFile>();//インスペクターでセッティング
    [SerializeField] List<StatusJsonFile> statusJsonFiles = new List<StatusJsonFile>();//インスペクターでセッティング

    // ★追加：キャラクターごとにスキルとステータスを保持
    private Dictionary<string, SkillEntryList> skillEntryDict = new Dictionary<string, SkillEntryList>();
    private Dictionary<string, StatusEntryList> statusEntryDict = new Dictionary<string, StatusEntryList>();

    private string savePath;

    public SkillEntryList GetSkillEntryList(string characterName)
    {
        if (skillEntryDict.ContainsKey(characterName))
            return skillEntryDict[characterName];
        Debug.LogWarning("指定したキャラクターのスキルデータがロードされていません: " + characterName);
        return null;
    }

    public StatusEntryList GetStatusEntryList(string characterName)
    {
        if (statusEntryDict.ContainsKey(characterName))
            return statusEntryDict[characterName];
        Debug.LogWarning("指定したキャラクターのステータスデータがロードされていません: " + characterName);
        return null;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // シーン切り替えでも残す

            // ★追加：起動時にロード
            LoadSkills();
            LoadStatuses();
        }
        else if (instance != this)
        {
            Destroy(gameObject); // 2つ目以降を消す
        }
    }

    /// <summary>
    /// スキルJsonファイルの読み込み
    /// </summary>
    public void LoadSkills()
    {
        foreach (var file in skillJsonFiles)
        {
            LoadSkill(file.GetcharacterName());
        }
    }

    /// <summary>
    /// キャラクター個別でスキルをロード
    /// </summary>
    public void LoadSkill(string characterName)
    {
        var file = skillJsonFiles.Find(f => f.GetcharacterName() == characterName);
        if (file == null)
        {
            Debug.LogError("指定したキャラクターのSkillJsonFileが見つかりません: " + characterName);
            return;
        }

        savePath = Application.persistentDataPath + file.GetSaveFileName();//保存ファイル名

        SkillEntryList skillEntryList;

        if (System.IO.File.Exists(savePath))
        {
            // 保存済みデータを読む
            string json = System.IO.File.ReadAllText(savePath);
            skillEntryList = JsonUtility.FromJson<SkillEntryList>(json);
            //Debug.Log("保存済みスキルデータをロードしました");
        }
        else
        {
            // 初回は TextAsset から読む
            skillEntryList = JsonUtility.FromJson<SkillEntryList>(file.GetTextAsset().text);
            //Debug.Log("初期スキルデータをロードしました");
        }

        skillEntryDict[characterName] = skillEntryList;
    }

    /// <summary>
    /// スキルの取得情報をJSON に保存（全キャラ）
    /// </summary>
    public void SaveSkillData()
    {
        foreach (var key in skillEntryDict.Keys)
        {
            SaveSkillData(key);
        }
    }

    /// <summary>
    /// スキルの取得情報をJSON に保存（キャラ個別）
    /// </summary>
    public void SaveSkillData(string characterName)
    {
        if (!skillEntryDict.ContainsKey(characterName)) return;

        var file = skillJsonFiles.Find(f => f.GetcharacterName() == characterName);
        if (file == null)
        {
            Debug.LogError("指定キャラクターのSkillJsonFileが見つかりません: " + characterName);
            return;
        }

        string json = JsonUtility.ToJson(skillEntryDict[characterName], true); // true = インデントつき
        string path = Application.persistentDataPath + file.GetSaveFileName();
        File.WriteAllText(path, json);

        //Debug.Log("スキルデータを保存しました: " + path);
    }

    /// <summary>
    /// スキルのJsonFile(Application.persistentDataPath)の初期化
    /// </summary>
    public void ResetSkillJsonFile()
    {
        foreach (SkillJsonFile file in skillJsonFiles)
        {
            ResetSkillJsonFile(file.GetcharacterName());
        }
        Debug.Log("すべてのスキルデータを初期化しました");
    }

    /// <summary>
    /// キャラクター個別のスキルデータ初期化
    /// </summary>
    public void ResetSkillJsonFile(string characterName)
    {
        var file = skillJsonFiles.Find(f => f.GetcharacterName() == characterName);
        if (file == null) return;

        savePath = Application.persistentDataPath + file.GetSaveFileName();
        if (File.Exists(savePath))
        {
            File.Delete(savePath); // 保存済みデータを削除
        }
        var skillEntryList = JsonUtility.FromJson<SkillEntryList>(file.GetTextAsset().text); // 初期データを読み込み
        skillEntryDict[characterName] = skillEntryList;
        SaveSkillData(characterName); // 再保存
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

        if (textAsset == null) Debug.LogError("指定したキャラクターのSkillJsonFileがありません"); ;

        return textAsset;
    }

    /// <summary>
    /// 引数で指定したキャラクターのSkillの個数を返す
    /// </summary>
    /// <param name="characterName"></param>
    /// <returns></returns>
    public int GetSkillSum(string characterName)
    {
        int count = 0;
        TextAsset textAsset = GetSkillJsonFile(characterName);
        SkillEntryList skillEntryList = JsonUtility.FromJson<SkillEntryList>(textAsset.text);

        foreach (var skill in skillEntryList.skills)
        {
            count++;
        }
        return count;
    }


    /// <summary>
    /// ステータスのJsonファイルの読み込み
    /// </summary>
    public void LoadStatuses()
    {
        foreach (var file in statusJsonFiles)
        {
            LoadStatus(file.GetcharacterName());
        }
    }

    /// <summary>
    /// キャラクター個別でステータスをロード
    /// </summary>
    public void LoadStatus(string characterName)
    {
        var file = statusJsonFiles.Find(f => f.GetcharacterName() == characterName);
        if (file == null)
        {
            Debug.LogError("指定したキャラクターのStatusJsonFileが見つかりません: " + characterName);
            return;
        }

        savePath = Application.persistentDataPath + file.GetSaveFileName();//保存ファイル名

        StatusEntryList statusEntryList;

        if (System.IO.File.Exists(savePath))
        {
            string json = System.IO.File.ReadAllText(savePath);
            statusEntryList = JsonUtility.FromJson<StatusEntryList>(json);
            //Debug.Log("保存済みステータスデータをロードしました");
        }
        else
        {
            statusEntryList = JsonUtility.FromJson<StatusEntryList>(file.GetTextAsset().text);
            //Debug.Log("初期ステータスデータをロードしました");
        }

        statusEntryDict[characterName] = statusEntryList;
    }

    /// <summary>
    /// ステータスの取得情報をJSON に保存（全キャラ）
    /// </summary>
    public void SaveStatusData()
    {
        foreach (var key in statusEntryDict.Keys)
        {
            SaveStatusData(key);
        }
    }

    /// <summary>
    /// ステータスの取得情報をJSON に保存（キャラ個別）
    /// </summary>
    public void SaveStatusData(string characterName)
    {
        if (!statusEntryDict.ContainsKey(characterName)) return;

        var file = statusJsonFiles.Find(f => f.GetcharacterName() == characterName);
        if (file == null)
        {
            Debug.LogError("指定キャラクターのStatusJsonFileが見つかりません: " + characterName);
            return;
        }

        string json = JsonUtility.ToJson(statusEntryDict[characterName], true); // true = インデントつき
        string path = Application.persistentDataPath + file.GetSaveFileName();
        File.WriteAllText(path, json);

        //Debug.Log("ステータスデータを保存しました: " + path);
    }

    /// <summary>
    /// ステータスのJsonFile(Application.persistentDataPath)の初期化
    /// </summary>
    public void ResetStatusJsonFile()
    {
        foreach (StatusJsonFile file in statusJsonFiles)
        {
            ResetStatusJsonFile(file.GetcharacterName());
        }
        Debug.Log("すべてのステータスデータを初期化しました");
    }

    /// <summary>
    /// キャラクター個別のステータス初期化
    /// </summary>
    public void ResetStatusJsonFile(string characterName)
    {
        var file = statusJsonFiles.Find(f => f.GetcharacterName() == characterName);
        if (file == null) return;

        savePath = Application.persistentDataPath + file.GetSaveFileName();
        if (File.Exists(savePath))
        {
            File.Delete(savePath); // 保存済みデータを削除
        }
        var statusEntryList = JsonUtility.FromJson<StatusEntryList>(file.GetTextAsset().text); // 初期データを読み込み
        statusEntryDict[characterName] = statusEntryList;
        SaveStatusData(characterName); // 再保存
    }

    /// <summary>
    /// 引数で指定したキャラクターのステータスのJsonファイルを返す
    /// </summary>
    /// <param name="characterName"></param>
    /// <returns></returns>
    public TextAsset GetStatusJsonFile(string characterName)
    {
        TextAsset textAsset = null;

        foreach (StatusJsonFile file in statusJsonFiles)
        {
            if (file.GetcharacterName().Equals(characterName))
            {
                textAsset = file.GetTextAsset();
            }
        }

        if (textAsset == null) Debug.LogError("指定したキャラクターのSutatusJsonFileがありません"); ;

        return textAsset;
    }
}
