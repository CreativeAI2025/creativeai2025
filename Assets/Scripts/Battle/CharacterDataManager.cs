using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


/// <summary>
/// ゲーム内の味方キャラクターのデータを管理するクラスです。
/// </summary>
public class CharacterDataManager : DontDestroySingleton<CharacterDataManager>
{
    /// <summary>
    /// 読み込んだキャラクターの経験値表の一覧です。
    /// </summary>
    private List<ExpTable> _expTables = new();

    /// <summary>
    /// 読み込んだキャラクターのパラメータ表の一覧です。
    /// </summary>
    private List<ParameterTable> _parameterTables = new();

    /// <summary>
    /// 読み込んだキャラクターのデータの一覧です。
    /// </summary>
    private List<CharacterData> _characterDataList = new();

    public override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 経験値表のデータをロードします。
    /// </summary>
    public async void LoadExpTables()
    {
        AsyncOperationHandle<IList<ExpTable>> handle = Addressables.LoadAssetsAsync<ExpTable>(AddressablesLabels.ExpTable, null);
        await handle.Task;
        _expTables = new List<ExpTable>(handle.Result);
        handle.Release();
    }

    /// <summary>
    /// 経験値表のデータを取得します。
    /// </summary>
    public ExpTable GetExpTable()
    {
        ExpTable expTable = null;
        if (_expTables.Count > 0)
        {
            expTable = _expTables[0];
        }
        return expTable;
    }

    /// <summary>
    /// パラメータ表のデータをロードします。
    /// </summary>
    public async void LoadParameterTables()
    {
        AsyncOperationHandle<IList<ParameterTable>> handle = Addressables.LoadAssetsAsync<ParameterTable>(AddressablesLabels.ParameterTable, null);
        await handle.Task;
        _parameterTables = new List<ParameterTable>(handle.Result);
        handle.Release();
    }

    /// <summary>
    /// IDからパラメータ表のデータを取得します。
    /// </summary>
    /// <param name="characterId">キャラクターID</param>
    public ParameterTable GetParameterTable(int characterId)
    {
        return _parameterTables.Find(parameterTable => parameterTable.characterId == characterId);
    }

    /// <summary>
    /// キャラクターの定義データをロードします。
    /// </summary>
    public async void LoadCharacterData()
    {
        AsyncOperationHandle<IList<CharacterData>> handle = Addressables.LoadAssetsAsync<CharacterData>(AddressablesLabels.Character, null);
        await handle.Task;
        _characterDataList = new List<CharacterData>(handle.Result);
        handle.Release();
    }

    /// <summary>
    /// キャラクターのIDからキャラクターの定義データを取得します。
    /// </summary>
    /// <param name="characterId">キャラクターID</param>
    public CharacterData GetCharacterData(int characterId)
    {
        return _characterDataList.Find(character => character.characterId == characterId);
    }

    /// <summary>
    /// キャラクターのIDからキャラクターの名前を取得します。
    /// </summary>
    /// <param name="characterId">キャラクターID</param>
    public string GetCharacterName(int characterId)
    {
        var characterData = GetCharacterData(characterId);
        return characterData.characterName;
    }

    /// <summary>
    /// キャラクターのIDから覚える魔法データを取得します。
    /// </summary>
    /// <param name="characterId">キャラクターID</param>
    public List<CharacterSkillRecord> GetCharacterSkillList(int characterId)
    {
        var characterData = _characterDataList.Find(character => character.characterId == characterId);
        return characterData.characterSkillRecords;
    }

    /// <summary>
    /// キャラクターのIDとレベルから現在覚えられる魔法データ一覧を取得します。
    /// </summary>
    /// <param name="characterId">キャラクターID</param>
    /// <param name="level">キャラクターのレベル</param>
    public List<SkillData> GetLearnableSkill(int characterId, int level)
    {
        var SkillList = GetCharacterSkillList(characterId);
        var records = SkillList.Where(x => x.level <= level);
        List<SkillData> skillDataList = new();
        foreach (var record in records)
        {
            var skillData = SkillDataManager.Instance.GetSkillDataById(record.skillId);
            skillDataList.Add(skillData);
        }
        return skillDataList;
    }
}