using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// ゲーム内の味方キャラクターのデータを管理するクラスです。
/// </summary>
public class CharacterDataManager : DontDestroySingleton<CharacterDataManager>
{
    /// <summary>
    /// 読み込んだキャラクターの経験値表の一覧です。
    /// </summary>
    private List<ExpTable> _expTables;

    /// <summary>
    /// 読み込んだキャラクターのパラメータ表の一覧です。
    /// </summary>
    private List<ParameterTable> _parameterTables;
    private Dictionary<int, ParameterTable> _parameterTableDisc;

    /// <summary>
    /// 読み込んだキャラクターのデータの一覧です。
    /// </summary>
    private List<CharacterData> _characterDataList;
    private Dictionary<int, CharacterData> characterDataDict;

    public override void Awake()
    {
        base.Awake();
    }

    public async Task Initialize()
    {
        await Task.WhenAll(
            LoadExpTables(),
            LoadParameterTables(),
            LoadCharacterData()
        );
        Debug.Log("[CharacterDataManager]すべてのデータのロードが完了しました。");
    }

    /// <summary>
    /// 経験値表のデータをロードします。
    /// </summary>
    public async Task LoadExpTables()
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
    public async Task LoadParameterTables()
    {
        AsyncOperationHandle<IList<ParameterTable>> handle = Addressables.LoadAssetsAsync<ParameterTable>(AddressablesLabels.ParameterTable, null);
        await handle.Task;
        _parameterTables = new List<ParameterTable>(handle.Result);
        handle.Release();
        _parameterTableDisc = _parameterTables.ToDictionary(table => table.characterId, table => table);
    }

    /// <summary>
    /// IDからパラメータ表のデータを取得します。
    /// </summary>
    /// <param name="characterId">キャラクターID</param>
    public ParameterTable GetParameterTable(int characterId)
    {
        return _parameterTableDisc[characterId];
    }

    /// <summary>
    /// キャラクターの定義データをロードします。
    /// </summary>
    public async Task LoadCharacterData()
    {
        AsyncOperationHandle<IList<CharacterData>> handle = Addressables.LoadAssetsAsync<CharacterData>(AddressablesLabels.Character, null);
        await handle.Task;
        _characterDataList = new List<CharacterData>(handle.Result);
        handle.Release();
        characterDataDict = _characterDataList.ToDictionary(data => data.characterId, data => data);
    }

    /// <summary>
    /// キャラクターのIDからキャラクターの定義データを取得します。
    /// </summary>
    /// <param name="characterId">キャラクターID</param>
    public CharacterData GetCharacterData(int characterId)
    {
        return characterDataDict[characterId];
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
}