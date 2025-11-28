#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

[System.Serializable]
public class SkillScriptableObject
{
    [Header("キャラクターの名前")]
    public string characterName;

    [Header("ScriptableObjectを生成するフォルダー先の指定")]
    public DefaultAsset targetFolder;

    public TextAsset textAsset;
}

[CustomEditor(typeof(SkillScriptableObjectCreaterEditorRuntime))]
public class SkillScriptableObjectCreaterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SkillScriptableObjectCreaterEditorRuntime myTarget = (SkillScriptableObjectCreaterEditorRuntime)target;

        if (GUILayout.Button("Generate Skill ScriptableObjects"))
        {
            GenerateScriptableObjects(myTarget);
        }
    }

    private void GenerateScriptableObjects(SkillScriptableObjectCreaterEditorRuntime runtimeTarget)
    {
        int id = 0;

        foreach (var list in runtimeTarget.creatSetting)
        {
            // フォルダ確認
            if (list.targetFolder == null)
            {
                Debug.LogError($"{list.characterName} の targetFolder が設定されていません。");
                continue;
            }

            string folderPath = AssetDatabase.GetAssetPath(list.targetFolder);

            DeleteFolderContents(folderPath);

            if (!Directory.Exists(folderPath))
            {
                Debug.LogError($"フォルダが存在しません: {folderPath}");
                continue;
            }

            // スキルデータ生成
            runtimeTarget.LoadSkills(list);

            foreach (var skill in runtimeTarget.skills)
            {
                string assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(folderPath, $"{list.characterName}_SkillData_{id}.asset"));

                SkillData asset = ScriptableObject.CreateInstance<SkillData>();
                runtimeTarget.SetupSkillAsset(asset, skill);

                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                id++;
            }

            runtimeTarget.skills.Clear();
        }

        EditorUtility.FocusProjectWindow();
        Debug.Log("ScriptableObjects generated successfully!");
    }

    private void DeleteFolderContents(string folderPath)
    {
        string[] files = Directory.GetFiles(folderPath);
        foreach (string file in files)
        {
            if (file.EndsWith(".meta")) continue;
            string assetPath = file.Replace("\\", "/");
            AssetDatabase.DeleteAsset(assetPath);
        }

        AssetDatabase.Refresh();
    }
}

// -------------------------------------------------------------
// Runtime用のデータ保持クラス（Editorから操作）
// -------------------------------------------------------------
public class SkillScriptableObjectCreaterEditorRuntime : MonoBehaviour
{
    public List<SkillScriptableObject> creatSetting = new List<SkillScriptableObject>();

    [Header("スキル重み設定")]
    public float powerValue = 1f;
    public float probabilityValue = 1f;
    public float durationValue = 1f;
    public float subjectValue = 1f;

    [Header("追加効果の重み")]
    public float sub_powerValue = 1f;
    public float sub_probabilityValue = 1f;
    public float sub_durationValue = 1f;
    public float sub_subjectValue = 1f;

    [SerializeField] public DataSetting dataSetting;
    [SerializeField] public bool is_dataSetting = false;
    [SerializeField] public DataSetting1 dataSetting1;
    [SerializeField] public bool is_dataSetting1 = false;

    [HideInInspector] public List<Skill> skills = new List<Skill>();

    // -------------------------------------------------------------
    // スキル生成用メソッド（Editorから呼び出す）
    // -------------------------------------------------------------
    public void LoadSkills(SkillScriptableObject list)
    {
        Dictionary<int, string[]> skillData = new Dictionary<int, string[]>();

        if (is_dataSetting)
        {
            skillData = dataSetting.SkillJsonLoader(list.characterName, list.textAsset);
            for (int i = 0; i < skillData.Count; i++)
            {
                skills.Add(dataSetting.SerchSkillDescription(skillData[i]));
            }
            skills = dataSetting.SetEvaluationValue(powerValue, probabilityValue, durationValue, subjectValue, skills);
        }

        if (is_dataSetting1)
        {
            skillData = dataSetting1.SkillJsonLoader(list.characterName, list.textAsset);
            for (int i = 0; i < skillData.Count; i++)
            {
                skills.Add(dataSetting1.SerchSkillDescription(skillData[i]));
            }
            skills = dataSetting1.SetEvaluationValue(skills);
        }
    }

    // -------------------------------------------------------------
    // SkillData アセットにスキル情報を設定する
    // -------------------------------------------------------------
    public void SetupSkillAsset(SkillData asset, Skill skill)
    {
        asset.skillId = skill.GetId();
        asset.skillName = skill.GetName();
        asset.cost = skill.GetMp();
        asset.skillDesc = skill.GetExplain();
        asset.skillEffect = new SkillEffect();

        // ここで skillCategory, EffectTarget などを設定
        // 既存の switch 文ロジックをここにコピーすればOK
        // 省略可能: 詳細ロジックは元コードを参照
    }
}
#endif