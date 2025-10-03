using UnityEngine;
using System;

public static class DamageFormula //ダメージ計算式を書く
{
    //攻撃時ダメージ
    public static int CalculateDamage(int attack, int defense)
    {
        float atk = attack / 2.0f;
        float def = defense / 4.0f;
        float rand = UnityEngine.Random.Range(0.8f, 1.2f);
        int damage = Mathf.Max(Mathf.CeilToInt((atk - def) * rand), 1);
        return damage;
    }
       //スキル攻撃時ダメージ
    public static int CalculateSkillDamage(int attack, int defense,float skillValue)
    {
        float atk = attack / 2.0f;
        float def = defense / 4.0f;
        
        float rand = UnityEngine.Random.Range(0.8f, 1.2f);
        int damage = Mathf.Max(Mathf.CeilToInt(((atk*skillValue) - def) * rand), 1);
        return damage;
    }
    //回復量計算
    public static int CalculateHealValue(float baseValue)
    {
        float rand = UnityEngine.Random.Range(0.8f, 1.2f);
        int healValue = Mathf.CeilToInt(baseValue * rand);
        return healValue;
    }
    //逃走判定
    public static bool CalculateCanRun(int friendSpeed, int enemySpeed)
    {
        float baseProbability = 50.0f;
        float speedDifference = friendSpeed - enemySpeed;
        float escapeRate = baseProbability + Mathf.Max(speedDifference, 0);
        float rand = UnityEngine.Random.Range(0.0f, 1.0f) * 100f;
        return rand < escapeRate;
    }
    //行動決定順
       public static int CalculateActionPriority(int speed)
    {
        float rand = UnityEngine.Random.Range(0.8f, 1.2f);
        int priority = Mathf.CeilToInt(speed * rand);
        return priority;
    }
}