using UnityEngine;

/// <summary>
/// ゲーム内の味方キャラクターのデータを管理するクラスです。
/// </summary>
public class CharacterDataManager : DontDestroySingleton<CharacterDataManager>
{
    /// <summary>
    /// 読み込んだキャラクターの経験値表の一覧です。
    /// </summary>
    [SerializeField] private ExpTable _expTable;

    /// <summary>
    /// 読み込んだキャラクターのデータの一覧です。
    /// </summary>
    [SerializeField] private CharacterDatabase _characterDatabase;

    public override void Awake()
    {
        base.Awake();
        Initialize();
    }

    public void Initialize()
    {
        LoadCharacterData();
        Debug.Log("[CharacterDataManager]すべてのデータのロードが完了しました。");
    }

    /// <summary>
    /// 経験値表のデータを取得します。
    /// </summary>
    public ExpTable GetExpTable()
    {
        return _expTable;
    }

    /// <summary>
    /// IDからパラメータ表のデータを取得します。
    /// </summary>
    /// <param name="characterId">キャラクターID</param>
    public ParameterTable GetParameterTable(int characterId)
    {
        var characterData = GetCharacterData(characterId);
        return characterData.parameterTable;
    }

    /// <summary>
    /// キャラクターの定義データをロードします。
    /// </summary>
    public void LoadCharacterData()
    {
        _characterDatabase.Initialize();
        Debug.Log("[CharacterDataManager]LoadCharacterData");
    }

    /// <summary>
    /// キャラクターのIDからキャラクターの定義データを取得します。
    /// </summary>
    /// <param name="characterId">キャラクターID</param>
    public CharacterData GetCharacterData(int characterId)
    {
        return _characterDatabase.GetCharacterData(characterId);
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