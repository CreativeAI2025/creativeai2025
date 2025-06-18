using System.Collections.Generic;
using UnityEngine;
namespace SimpleRpg
{
    [CreateAssetMenu(fileName = "ExpTable", menuName = "Scriptable Objects/ExpTable")]
    public class ExpTable : ScriptableObject
    {
        public List<ExpRecord> expRecords;
    }
}