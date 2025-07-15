using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public int enemyId;
    public string enemyName;
    public Sprite sprite;
    public int HP;
    public int MP;
    public int Attack;
    public int Defence;
    public int MagicAttack;
    public int MagicDefence;
    public int Speed;
    public int Evasion;
    public int exp;
    public int gold;
    public List<EnemyActionRecord> enemyActionRecords;
}
