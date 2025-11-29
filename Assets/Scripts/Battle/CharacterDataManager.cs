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
    private Dictionary<int, Sprite> _characterSpriteDict;

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
        await LoadAllCharacterSprites();
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
    /// 全キャラクターのSpriteデータをロードします。
    /// </summary>
    private async Task LoadAllCharacterSprites()
    {
        _characterSpriteDict = new Dictionary<int, Sprite>();

        // CharacterDataList を基に、すべての Sprite を並行してロード
        var loadTasks = new List<Task>();
        var spriteHandles = new List<AsyncOperationHandle<Sprite>>();

        foreach (var data in _characterDataList)
        {
            // AssetReferenceSprite からロード操作を開始
            AsyncOperationHandle<Sprite> handle = data.sprite.LoadAssetAsync<Sprite>();
            spriteHandles.Add(handle);

            // ロード完了を待つ Task をリストに追加
            loadTasks.Add(handle.Task.ContinueWith(t =>
            {
                if (t.Status == TaskStatus.RanToCompletion && handle.Status == AsyncOperationStatus.Succeeded)
                {
                    // ロード成功時のみ Dictionary に追加
                    lock (_characterSpriteDict) // 並行処理のためロック推奨
                    {
                        _characterSpriteDict[data.characterId] = handle.Result;
                    }
                }
                else
                {
                    Debug.LogError($"Spriteロード失敗: ID {data.characterId}");
                }
            }));
        }

        // 全てのロード完了を待機
        await Task.WhenAll(loadTasks);
    }

    /// <summary>
    /// キャラクターのIDからキャラクターの定義データを取得します。
    /// </summary>
    /// <param name="characterId">キャラクターID</param>
    public CharacterData GetCharacterData(int characterId)
    {
        if (!characterDataDict.ContainsKey(characterId))
        {
            return null;
        }
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

    /// <summary>
    /// キャラクターのIDからSpriteを取得します。（同期アクセス）
    /// </summary>
    /// <param name="characterId">キャラクターID</param>
    public Sprite GetCharacterSprite(int characterId) // 🚨 新しく追加
    {
        if (_characterSpriteDict != null && _characterSpriteDict.ContainsKey(characterId))
        {
            return _characterSpriteDict[characterId];
        }
        return null; // ロードされていない、またはIDが存在しない
    }
}