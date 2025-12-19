using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "Scriptable Objects/EnemyDatabase")]
public class EnemyDatabase : ScriptableObject
{
    public EnemyData[] enemyDataList;
    private Dictionary<int, int> _enemyDataDict;

    public void Initialize()
    {
        _enemyDataDict = new Dictionary<int, int>();
        for (int i = 0; i < enemyDataList.Length; i++)
        {
            var enemyData = enemyDataList[i];
            _enemyDataDict.Add(enemyData.enemyId, i);
        }
    }

    public EnemyData GetEnemyData(int id)
    {
        return enemyDataList[_enemyDataDict[id]];
    }
}
