
using System.Collections.Generic;
using UnityEngine;
namespace SimpleRpg
{
    [CreateAssetMenu(fileName = "ParameterTable", menuName = "Scriptable Objects/ParameterTable")]
    public class ParameterTable : ScriptableObject
    {
         public Sprite characterIcon;  
        public int CharacterID;
        public string CharacterName;
        public List<ParameterRecord> parameterRecords;
    }
}