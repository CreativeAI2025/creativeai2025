using UnityEngine;
using System;

public class Skill
{
    private int id;//各ノードのID

    //メイン効果
    private string explain;
    private string tag;//タグ名（スキルorステータス）
    private string name;//スキルの名前
    private string subject;//対象
    private string action;//行動(攻撃、回復など)
    private int probability;//確率
    private float power;//効果量
    private string type;//種類（物理攻撃、特殊攻撃など）
    private string status;//バフ・デバフ（ステータス）
    private string extra;//追加効果
    private int duration;//持続ターン

    //追加効果
    public bool isSub;//追加効果があるかどうか
    public string sub_tag;//タグ名（スキルorステータス）
    public string sub_name;//スキルの名前
    public string sub_subject;//対象
    public string sub_action;//行動(攻撃、回復など)
    public int sub_probability;//確率
    public float sub_power;//効果量
    public string sub_type;//種類（物理攻撃、特殊攻撃など）
    public string sub_status;//バフ・デバフ（ステータス）
    public string sub_extra;//追加効果
    public int sub_duration;//持続ターン


    private int mp; // MP（使用コスト）
    private int sp;//　SP（スキル獲得ポイント）
    private float evaluationValue;//評価値

    public Skill(string name, string explain, string subject, string action, int probability, float power, string type, string status, string extra, int duration,
    bool isSub = false, string sub_subject = null, string sub_action = null, int sub_probability = 0, float sub_power = 0f, string sub_type = null,
    string sub_status = null, string sub_extra = null, int sub_duration = 0)
    {//コンストラクタ（スキル用（詳細情報））
        this.id = 0;//ID
        this.explain = explain;
        this.tag = "スキル";//分類
        this.name = name;//名前

        //メイン効果
        this.subject = subject;//対象
        this.action = action;//行動
        this.probability = probability;//確率
        this.power = power;//強さ
        this.type = type;//種類
        this.status = status;
        this.extra = extra;
        this.duration = duration;

        //追加効果
        this.isSub = isSub;
        this.sub_subject = sub_subject;//対象
        this.sub_action = sub_action;//行動
        this.sub_probability = sub_probability;//確率
        this.sub_power = sub_power;//強さ
        this.sub_type = sub_type;//種類
        this.sub_status = sub_status;
        this.sub_extra = sub_extra;
        this.sub_duration = sub_duration;

        this.mp = 0;//スキル獲得に必要なコスト
        this.sp = 0;//スキル使用に必要なコスト
        this.evaluationValue = 0;
    }

    public int GetId() { return id; }
    public string GetExplain() { return explain; }
    public string GetTag() { return tag; }
    public string GetName() { return name; }
    public string GetSubject() { return subject; }
    public string GetAction() { return action; }
    public int GetProbability() { return probability; }
    public float GetPower() { return power; }
    public string GetTypeName() { return type; }
    public string GetStatus() { return status; }
    public string GetExtra() { return extra; }
    public int GetDuration() { return duration; }
    public int GetMp() { return mp; }
    public int GetSp() { return sp; }
    public float GetEvaluationValue() { return evaluationValue; }

    public void SetMp(int mp)
    {
        this.mp = mp;
    }

    public void SetSp(int sp)
    {
        this.sp = sp;
    }

    public void SetId(int id)
    {
        this.id = id;
    }

    public void SetExplain(string explain)
    {
        this.explain = explain;
    }

    public void SetEvaluationValue(float evaluationValue)
    {
        this.evaluationValue = evaluationValue;
    }

    public string toString()
    {
        return "ID:" + id
        + "スキル名: " + name
        + ", 対象: " + subject
        + ", 行動: " + action
        + ", 発動確率" + probability
        + ", 効果量: " + power
        + ", 種類: " + type
        + ", 追加効果: " + extra
        + ", 持続ターン: " + duration
        + ", MP:" + mp
        + ", SP" + sp
        + ", 評価値" + evaluationValue;
    }

    /// <summary>
    /// 元の文をきれいにした文字列を返す（引数：スキルの種類）
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public string toSkillstring(string type)
    {
        if (type != null)
        {
            if ("物理攻撃" == type || "特殊攻撃" == type)
            {
                return "スキル:" + name + "\n" + probability + "％の確率で" + subject + "に" + power + "ダメージの" + type + "\n必要SP:" + sp + " 必要MP:" + mp;
            }
            else if ("回復" == type)
            {
                return "スキル:" + name + "\n" + probability + "％の確率で" + subject + "に" + power + "の" + type + "\n必要SP:" + sp + " 必要MP:" + mp;
            }
            else if ("バフ" == type)
            {
                return "スキル:" + name + "\n" + probability + "％の確率で" + subject + "の" + status + "を" + duration + "ターン上昇させる" + "\n必要SP:" + sp + " 必要MP:" + mp;
            }
            else if ("デバフ" == type)
            {
                return "スキル:" + name + "\n" + probability + "％の確率で" + subject + "の" + status + "を" + duration + "ターン減少させる" + "\n必要SP:" + sp + " 必要MP:" + mp;
            }
            else if ("毒" == type)
            {
                return "スキル:" + name + "\n" + probability + "％の確率で" + subject + "を" + duration + "ターン" + type + "状態にする" + "\n必要SP:" + sp + " 必要MP:" + mp;
            }
            else if ("%で復活" == type)
            {
                return "スキル:" + name + "\n" + probability + "％の確率で" + subject + "をHP" + power + type + "させる" + "\n必要SP:" + sp + " 必要MP:" + mp;
            }
        }
        return "スキル名: " + name
          + ", 対象: " + subject
          + ", 行動: " + action
          + ", 発動確率" + probability
          + ", 効果量: " + power
          + ", 種類: " + type
          + ", ステータス: " + status
          + ", 追加効果: " + extra
          + ", 持続ターン: " + duration
          + ", MP:" + mp
          + ", SP" + sp
          + ", 評価値" + evaluationValue;
    }
}
