using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CharacterDatabase", menuName = "Scriptable Objects/CharacterDatabase")]
public class CharacterDatabase : ScriptableObject
{
    public CharacterData[] characterDataList;
    private Dictionary<int, int> _characterDataDict;

    public void Initialize()
    {
        _characterDataDict = new Dictionary<int, int>();
        for (int i = 0; i < characterDataList.Length; i++)
        {
            var characterData = characterDataList[i];
            _characterDataDict.Add(characterData.characterId, i);
        }
    }

    public CharacterData GetCharacterData(int id)
    {
        return characterDataList[_characterDataDict[id]];
    }
}
