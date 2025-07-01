using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         LoadDefinitionData();
    }

    // Update is called once per frame
    void LoadDefinitionData()
        {
            CharacterDataManager.LoadCharacterData();
            CharacterDataManager.LoadExpTables();
            CharacterDataManager.LoadParameterTables();

            EnemyDataManager.LoadEnemyData();
            ItemDataManager.LoadItemData();
            MagicDataManager.LoadMagicData();
        }
}
