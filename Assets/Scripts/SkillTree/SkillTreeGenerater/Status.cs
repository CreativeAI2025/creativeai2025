using UnityEngine;
using System;

public class Status : IComparable<Status>
{
    private int id;
    private string tag;
    private string name;
    private int power;//効果量
    private string type;//種類（攻撃上昇、HP上昇など）
    private int mp; // MP（コスト）
    private string explain;
    public Status(string name, int power, string type, string explain)
    {
        this.id = 0;//ID
        this.tag = "ステータス";//分類
        this.name = name;//名前
        this.power = power;//効果量
        this.type = type;//種類
        this.mp = 0;//獲得に必要なコスト
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

    public string getType()
    {
        return this.type;
    }

    public string getExplain()
    {
        return this.explain;
    }

    public void setMp(int mp)
    {
        this.mp = mp;
    }

    public void setId(int id)
    {
        this.id = id;
    }
    public void setExplain(string explain)
    {
        this.explain = explain;
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
