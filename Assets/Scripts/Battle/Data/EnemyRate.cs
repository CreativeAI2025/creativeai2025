using System;


/// <summary>
/// 敵のIDと出現しやすさを保持するクラスです。
/// </summary>
[Serializable]
public class EnemyRate
{
    /// <summary>
    /// 敵のIDです。
    /// </summary>
    public int enemyId;

    /// <summary>
    /// 敵の出現しやすさです。
    /// 敵の出現率は、敵の出現しやすさの合計値で割った値になります。
    /// </summary>
    public int rate;

}