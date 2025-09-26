
using System.Collections.Generic;
using UnityEngine;

    [CreateAssetMenu(fileName = "ParameterTable", menuName = "Scriptable Objects/ParameterTable")]
    public class ParameterTable : ScriptableObject
    {
         public Sprite characterIcon;  
        public int characterId;
        public List<ParameterRecord> parameterRecords;
    }
