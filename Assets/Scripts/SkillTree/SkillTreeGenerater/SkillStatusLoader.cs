using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEditor.Overlays;

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
    [SerializeField] List<SkillJsonFile> skillJsonFiles = new List<SkillJsonFile>();
    [SerializeField] List<StatusJsonFile> statusJsonFiles = new List<StatusJsonFile>();
    private SkillEntryList skillEntryList;//Json読み込み用リスト
    private StatusEntryList statusEntryList;//Json読み込み用リスト
    private string savePath;
    public SkillEntryList GetSkillEntryList()
    {
        return this.skillEntryList;
    }

    public StatusEntryList GetStatusEntryList()
    {
        return this.statusEntryList;
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
            savePath = Application.persistentDataPath + file.GetSaveFileName();//保存ファイル名

            if (System.IO.File.Exists(savePath))
            {
                // 保存済みデータを読む
                string json = System.IO.File.ReadAllText(savePath);
                skillEntryList = JsonUtility.FromJson<SkillEntryList>(json);
                Debug.Log("保存済みスキルデータをロードしました");
            }
            else
            {
                // 初回は TextAsset から読む
                skillEntryList = JsonUtility.FromJson<SkillEntryList>(file.GetTextAsset().text);
                Debug.Log("初期スキルデータをロードしました");
            }
        }
    }

    /// <summary>
    /// スキルの取得情報をJSON に保存
    /// </summary>
    public void SaveSkillData()
    {
        foreach (var file in skillJsonFiles)
        {
            string json = JsonUtility.ToJson(skillEntryList, true); // true = インデントつき
            string path = Application.persistentDataPath + file.GetSaveFileName();
            File.WriteAllText(path, json);

            //Debug.Log("スキルデータを保存しました: " + path);
        }
    }

    /// <summary>
    /// スキルのJsonFile(Application.persistentDataPath)の初期化
    /// </summary>
    public void ResetSkillJsonFile()
    {
        foreach (SkillJsonFile file in skillJsonFiles)
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath); // 保存済みデータを削除
            }
            skillEntryList = JsonUtility.FromJson<SkillEntryList>(file.GetTextAsset().text); // 初期データを読み込み
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

        if (textAsset == null) Debug.LogError("指定したキャラクターのSkillJsonFileがありません"); ;

        return textAsset;
    }


    /// <summary>
    /// ステータスのJsonファイルの読み込み
    /// </summary>
    public void LoadStatuses()
    {
        foreach (var file in statusJsonFiles)
        {
            savePath = Application.persistentDataPath + file.GetSaveFileName();//保存ファイル名

            if (System.IO.File.Exists(savePath))
            {
                string json = System.IO.File.ReadAllText(savePath);
                statusEntryList = JsonUtility.FromJson<StatusEntryList>(json);
                Debug.Log("保存済みステータスデータをロードしました");
            }
            else
            {
                statusEntryList = JsonUtility.FromJson<StatusEntryList>(file.GetTextAsset().text);
                Debug.Log("初期ステータスデータをロードしました");
            }
        }
    }

    /// <summary>
    /// ステータスの取得情報をJSON に保存
    /// </summary>
    public void SaveStatusData()
    {
        foreach (var file in statusJsonFiles)
        {
            string json = JsonUtility.ToJson(statusEntryList, true); // true = インデントつき
            string path = Application.persistentDataPath + file.GetSaveFileName();
            File.WriteAllText(path, json);

            //Debug.Log("ステータスデータを保存しました: " + path);
        }
    }

    /// <summary>
    /// ステータスのJsonFile(Application.persistentDataPath)の初期化
    /// </summary>
    public void ResetStatusJsonFile()
    {
        foreach (StatusJsonFile file in statusJsonFiles)
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath); // 保存済みデータを削除
            }
            statusEntryList = JsonUtility.FromJson<StatusEntryList>(file.GetTextAsset().text); // 初期データを読み込み
            SaveStatusData(); // 再保存
        }
        Debug.Log("すべてのステータスデータを初期化しました");
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
