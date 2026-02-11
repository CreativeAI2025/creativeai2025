using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SkillpointManager : DontDestroySingleton<SkillpointManager>
{
    private Dictionary<string, int> _points;
    private string _skillpointSaveFilePath;
    public override void Awake()
    {
        base.Awake();
        _skillpointSaveFilePath = string.Join('/', Application.persistentDataPath, "SkillpointData.dat");
        if (File.Exists(_skillpointSaveFilePath))
        {
            SKillpointData points = SaveUtility.SaveFileToData<SKillpointData>(_skillpointSaveFilePath);
            _points = points.Points;
        }
        else
        {
            Debug.Log("スキルポイントデータがありませんでした");
            SaveInitPoints();
        }
    }

    private void SaveInitPoints()
    {
        IFileAssetLoader loader = SaveUtility.FileAssetLoaderFactory();
        string skillpointFilePath = loader.GetPath("SkillpointList.json");
        SKillpointData points = SaveUtility.JsonToData<SKillpointData>(skillpointFilePath);
        _points = points.Points;
        SaveSkillpoint();
    }

    /// <summary>
    /// 特定のキャラクターを任意のスキルポイントに変更する
    /// </summary>
    /// <param name="characterId"></param>
    /// <param name="point"></param>
    public void AddSkillpoint(int characterId, int point)
    {
        string id = characterId.ToString();
        _points[id] += point;
        SaveSkillpoint();
    }

    /// <summary>
    /// 特定のキャラクターのこれまで獲得したスキルポイントの値を返す
    /// </summary>
    /// <param name="characterId"></param>
    /// <returns></returns>
    public int GetSkillPoint(int characterId)
    {
        string id = characterId.ToString();
        return _points[id];
    }

    private void SaveSkillpoint()
    {

        SKillpointData saveSkillpointData = new SKillpointData()
        {
            Points = _points
        };
        SaveUtility.DataToSaveFile(saveSkillpointData, _skillpointSaveFilePath);
    }

    public void DeleteSkillpointFile()
    {
        File.Delete(_skillpointSaveFilePath);
        SaveInitPoints();
    }

}
