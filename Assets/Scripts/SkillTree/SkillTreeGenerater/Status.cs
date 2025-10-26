using UnityEngine;
using System;

public class Status : IComparable<Status>
{
    private int id;
    private string tag;
    private string name;
    private int power;//効果量
    private string type;//種類（攻撃上昇、HP上昇など）
    private int sp; // SP（コスト）
    private string explain;
    public Status(string name, int power, string type, string explain, int sp)
    {
        this.id = 0;//ID
        this.tag = "ステータス";//分類
        this.name = name;//名前
        this.power = power;//効果量
        this.type = type;//種類
        this.sp = sp;//獲得に必要なコスト
        this.explain = explain;
    }

    public int getId()
    {
        return this.id;
    }
    public string getName()
    {
        return this.name;
    }
    public int GetPower()
    {
        return this.power;
    }

    public new string GetType()
    {
        return this.type;
    }

    public int GetSp()
    {
        return this.sp;
    }

    public string getExplain()
    {
        return this.explain;
    }

    public void setSp(int mp)
    {
        this.sp = mp;
    }

    public void setId(int id)
    {
        this.id = id;
    }
    public void setExplain(string explain)
    {
        this.explain = explain;
    }

#pragma warning disable CS0114 // メンバーは継承されたメンバーを非表示にします。override キーワードがありません
    public string ToString()
#pragma warning restore CS0114 // メンバーは継承されたメンバーを非表示にします。override キーワードがありません
    {
        return "名前：" + this.name
        + ", 種類：" + type
        + ", 強さ：" + power + "%"
        + ", SP：" + sp;
    }

    /// <summary>
    /// ソート用
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(Status other)
    {
        if (other == null) return 1;
        return this.id.CompareTo(other.id);
    }
}
