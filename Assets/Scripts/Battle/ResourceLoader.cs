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
            CharacterDataManager.Instance.LoadCharacterData();
            CharacterDataManager.Instance.LoadExpTables();
            CharacterDataManager.Instance.LoadParameterTables();

            EnemyDataManager.Instance.LoadEnemyData();
            ItemDataManager.Instance.LoadItemData();
            SkillDataManager.Instance.LoadSkillData();
        }
    }