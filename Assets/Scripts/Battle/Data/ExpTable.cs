using System.Collections.Generic;
using UnityEngine;

    [CreateAssetMenu(fileName = "ExpTable", menuName = "Scriptable Objects/ExpTable")]
    public class ExpTable : ScriptableObject
    {
        public List<ExpRecord> expRecords;
    }
