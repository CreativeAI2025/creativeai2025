using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

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

    [Header("追加効果の効果量の重み"), SerializeField] float sub_powerValue = 1f;
    [Header("追加効果の発動確率の重み"), SerializeField] float sub_probabilityValue = 1f;
    [Header("追加効果の効果時間（ターン数）の重み"), SerializeField] float sub_durationValue = 1f;
    [Header("追加効果の攻撃対象の重み"), SerializeField] float sub_subjectValue = 1f;

    [SerializeField] DataSetting dataSetting;
    List<Skill> skills = new List<Skill>();
    Dictionary<int, string[]> skillData = new Dictionary<int, string[]>();// スキル名とスキルの説明のデータ

    [ContextMenu("Generate Skill ScriptableObject")]
    public void GenerateScriptableObject()
    {
        int id = 0;

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
                        skillCategory = SkillCategory.PhysicalDamage;
                        break;
                    case "魔法攻撃":
                        skillCategory = SkillCategory.MagicDamage;
                        break;
                    case "特殊攻撃":
                        skillCategory = SkillCategory.MagicDamage;
                        break;
                    case "回復":
                        skillCategory = SkillCategory.Recovery;
                        break;
                    case "状態異常回復":
                        skillCategory = SkillCategory.EffectRecovery;
                        break;
                    case "復活":
                        skillCategory = SkillCategory.Revive;
                        break;
                    case "強化":
                        skillCategory = SkillCategory.Buff;
                        break;
                    case "弱体":
                        skillCategory = SkillCategory.DeBuff;
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

                SkillCategory extra_skillCategory = SkillCategory.None;

                switch (skill.sub_action)
                {
                    case "物理攻撃":
                        extra_skillCategory = SkillCategory.PhysicalDamage;
                        break;
                    case "魔法攻撃":
                        extra_skillCategory = SkillCategory.MagicDamage;
                        break;
                    case "特殊攻撃":
                        extra_skillCategory = SkillCategory.MagicDamage;
                        break;
                    case "回復":
                        extra_skillCategory = SkillCategory.Recovery;
                        break;
                    case "状態異常回復":
                        extra_skillCategory = SkillCategory.EffectRecovery;
                        break;
                    case "復活":
                        extra_skillCategory = SkillCategory.Revive;
                        break;
                    case "強化":
                        extra_skillCategory = SkillCategory.Buff;
                        break;
                    case "弱体":
                        extra_skillCategory = SkillCategory.DeBuff;
                        break;
                    case "":
                        break;
                    default:
                        break;
                }


                EffectTarget extra_effectTarget = EffectTarget.EnemySolo;

                switch (skill.sub_subject)
                {
                    case "相手":
                        extra_effectTarget = EffectTarget.EnemySolo;
                        break;
                    case "相手全体":
                        extra_effectTarget = EffectTarget.EnemyAll;
                        break;
                    case "味方1人":
                        extra_effectTarget = EffectTarget.FriendSolo;
                        break;
                    case "味方全体":
                        extra_effectTarget = EffectTarget.FriendAll;
                        break;
                    case "自分":
                        extra_effectTarget = EffectTarget.Own;
                        break;
                    case "不明":
                        break;
                    case "null":
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
                        skill.GetDuration(),
                        skill.isSub,
                        extra_skillCategory,
                        extra_effectTarget,
                        skill.sub_power,
                        skill.sub_probability,
                        skill.sub_status,
                        skill.sub_duration
                    );

                if (skill.GetStatus() != null)
                {
                    string[] statuses = Regex.Split(skill.GetStatus(), "[,、]+");// 説明文

                    BuffStatusCategory buffStatusCategory = BuffStatusCategory.Attack;

                    for (int i = 0; i < statuses.Length; i++)//メインのバフ要素の格納
                    {
                        switch (statuses[i])
                        {
                            case "攻撃力":
                                buffStatusCategory = BuffStatusCategory.Attack;
                                break;
                            case "防御力":
                                buffStatusCategory = BuffStatusCategory.Defence;
                                break;
                            case "回避率":
                                buffStatusCategory = BuffStatusCategory.Evasion;
                                break;
                            case "魔力":
                                buffStatusCategory = BuffStatusCategory.MagicAttack;
                                break;
                            case "魔法防御力":
                                buffStatusCategory = BuffStatusCategory.MagicDefence;
                                break;
                            case "魔法防御":
                                buffStatusCategory = BuffStatusCategory.MagicDefence;
                                break;
                            case "素早さ":
                                buffStatusCategory = BuffStatusCategory.Speed;
                                break;
                            case "全ステータス":
                                buffStatusCategory = BuffStatusCategory.All;
                                break;
                            default:
                                break;
                        }

                        asset.skillEffect.buff.Add(new Buff(buffStatusCategory, skill.GetDuration(), skill.GetPower(), effectTarget));
                    }
                }


                StatusEffectCategory statusEffectCategory = StatusEffectCategory.None;

                if (skill.GetExtra() != null)
                {
                    switch (skill.GetExtra())//状態異常
                    {
                        case "毒":
                            statusEffectCategory = StatusEffectCategory.Poison;
                            break;
                        case "麻痺":
                            statusEffectCategory = StatusEffectCategory.Paralysis;
                            break;
                        case "睡眠":
                            statusEffectCategory = StatusEffectCategory.Sleep;
                            break;
                        default:
                            break;
                    }

                    asset.skillEffect.StatusEffect.Add(new StatusEffect(statusEffectCategory));//メインの状態異常要素の格納
                }

                if (skill.isSub)
                {
                    //追加効果の対象
                    switch (skill.sub_subject)
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

                    if (skill.sub_status != null)
                    {
                        string[] statuses = Regex.Split(skill.sub_status, "[,、]+");// 説明文

                        BuffStatusCategory buffStatusCategory = BuffStatusCategory.None;

                        for (int i = 0; i < statuses.Length; i++)//追加効果のバフ要素の格納
                        {
                            switch (statuses[i])
                            {
                                case "攻撃力":
                                    buffStatusCategory = BuffStatusCategory.Attack;
                                    break;
                                case "防御力":
                                    buffStatusCategory = BuffStatusCategory.Defence;
                                    break;
                                case "回避率":
                                    buffStatusCategory = BuffStatusCategory.Evasion;
                                    break;
                                case "魔力":
                                    buffStatusCategory = BuffStatusCategory.MagicAttack;
                                    break;
                                case "魔法防御力":
                                    buffStatusCategory = BuffStatusCategory.MagicDefence;
                                    break;
                                case "魔法防御":
                                    buffStatusCategory = BuffStatusCategory.MagicDefence;
                                    break;
                                case "素早さ":
                                    buffStatusCategory = BuffStatusCategory.Speed;
                                    break;
                                case "全ステータス":
                                    buffStatusCategory = BuffStatusCategory.All;
                                    break;
                                default:
                                    break;
                            }

                            asset.skillEffect.extar_buff.Add(new Buff(buffStatusCategory, skill.sub_duration, skill.sub_power, effectTarget));
                        }
                    }


                    statusEffectCategory = StatusEffectCategory.None;

                    if (skill.sub_extra != null)
                    {
                        switch (skill.sub_extra)//状態異常
                        {
                            case "毒":
                                statusEffectCategory = StatusEffectCategory.Poison;
                                break;
                            case "麻痺":
                                statusEffectCategory = StatusEffectCategory.Paralysis;
                                break;
                            case "睡眠":
                                statusEffectCategory = StatusEffectCategory.Sleep;
                                break;
                            default:
                                break;
                        }

                        asset.skillEffect.extra_StatusEffect.Add(new StatusEffect(statusEffectCategory));//追加効果の状態異常要素の格納
                    }
                }


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
