using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[System.Serializable]
public class SkillScriptableObject
{
    [Header("キャラクターの名前")] public string characterName;
    [Header("ScriptableObjectを生成するフォルダー先の指定")] public DefaultAsset targetFolder;
    public TextAsset textAsset;
}

public class SkillScriptableObjectCreater : MonoBehaviour
{
    [SerializeField] private List<SkillScriptableObject> creatSetting = new List<SkillScriptableObject>();
    [Header("効果量の重み"), SerializeField] float powerValue = 1f;
    [Header("発動確率の重み"), SerializeField] float probabilityValue = 1f;
    [Header("効果時間（ターン数）の重み"), SerializeField] float durationValue = 1f;
    [Header("攻撃対象の重み"), SerializeField] float subjectValue = 1f;
    [SerializeField] DataSetting dataSetting;
    List<Skill> skills = new List<Skill>();
    Dictionary<int, string[]> skillData = new Dictionary<int, string[]>();// スキル名とスキルの説明のデータ

    [ContextMenu("Generate Skill ScriptableObject")]
    public void GenerateScriptableObject()
    {
        foreach (var list in creatSetting)
        {
            skillData = dataSetting.SkillJsonLoader(list.characterName, list.textAsset);

            for (int i = 0; i < skillData.Count; i++)
            {
                // データ格納
                skills.Add(dataSetting.SerchSkillDescription(skillData[i]));
            }

            skills = dataSetting.SetEvaluationValue(powerValue, probabilityValue, durationValue, subjectValue, skills);

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

            int id = 0;
            foreach (var skill in skills)
            {
                string assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(folderPath, $"{list.characterName}_SkillData_{id}.asset"));
                //
                // ScriptableObject生成
                SkillData asset = ScriptableObject.CreateInstance<SkillData>();
                asset.skillId = id;
                asset.skillName = skill.GetName();
                asset.cost = skill.GetMp();
                asset.skillDesc = skill.GetExplain();

                SkillCategory skillCategory = SkillCategory.None;

                switch (skill.GetAction())
                {
                    case "物理攻撃":
                        skillCategory = SkillCategory.Damage;
                        break;
                    case "魔法攻撃":
                        skillCategory = SkillCategory.Damage;
                        break;
                    case "特殊攻撃":
                        skillCategory = SkillCategory.Damage;
                        break;
                    case "回復":
                        skillCategory = SkillCategory.Recovery;
                        break;
                    case "復活":
                        skillCategory = SkillCategory.None;
                        break;
                    case "強化":
                        skillCategory = SkillCategory.Support;
                        break;
                    case "弱体":
                        skillCategory = SkillCategory.None;
                        break;
                    case "":
                        break;
                    default:
                        break;
                }


                EffectTarget effectTarget = EffectTarget.EnemySolo;

                switch (skill.GetSubject())
                {
                    case "相手":
                        effectTarget = EffectTarget.EnemySolo;
                        break;
                    case "相手全体":
                        effectTarget = EffectTarget.EnemyAll;
                        break;
                    case "味方1人":
                        effectTarget = EffectTarget.FriendSolo;
                        break;
                    case "味方全体":
                        effectTarget = EffectTarget.FriendAll;
                        break;
                    case "自分":
                        effectTarget = EffectTarget.Own;
                        break;
                    case "不明":
                        break;
                    default:
                        break;
                }


                asset.skillEffect = new SkillEffect(
                        skillCategory,
                        effectTarget,
                        skill.GetPower(),
                        skill.GetProbability(),
                        skill.GetStatus(),
                        skill.GetDuration()
                    );

                // アセット作成
                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                //

                id++;
            }

            skillData.Clear();
            skills.Clear();
        }

        EditorUtility.FocusProjectWindow();
    }

    /// <summary>
    /// フォルダ内の中身だけ削除する（フォルダ自体は残す）
    /// </summary>
    /// <param name="folderPath"></param>
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
        Debug.Log($"フォルダをクリーンアップしました: {folderPath}");
    }
}
