using UnityEngine;
using System;

public class WorldmapManager : DontDestroySingleton<WorldmapManager>
{
    public event Action OnWorldmapStart { add => _onWorldmapStart += value; remove => _onWorldmapStart -= value; }
    private Action _onWorldmapStart;
    public event Action OnWorldmapEnd { add => _onWorldmapEnd += value; remove => _onWorldmapEnd -= value; }
    private Action _onWorldmapEnd;
    private InputSetting _inputSetting;
    private Vector2Int _spawnPoint;
    private string _nextScene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Initialize()
    {
        _onWorldmapStart?.Invoke();
    }

    private void End()
    {
        _onWorldmapEnd?.Invoke();
    }

    public Vector2Int GetSpawnPoint()
    {
        return _spawnPoint;
    }

    public string GetNextScene()
    {
        return _nextScene;
    }
}
