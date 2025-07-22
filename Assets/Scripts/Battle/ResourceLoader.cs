using UnityEngine;
    /// <summary>
    /// 定義データをロードするクラスです。
    /// </summary>
    public class ResourceLoader : MonoBehaviour
    {
        void Start()
        {
            LoadDefinitionData();
        }

        /// <summary>
        /// 定義データをロードします。
        /// </summary>
        void LoadDefinitionData()
        {
            CharacterDataManager.LoadCharacterData();
            CharacterDataManager.LoadExpTables();
            CharacterDataManager.LoadParameterTables();

            EnemyDataManager.LoadEnemyData();
            ItemDataManager.LoadItemData();
            SkillDataManager.LoadSkillData();
        }
    }