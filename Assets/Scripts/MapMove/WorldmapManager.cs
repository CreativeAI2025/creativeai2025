/*using UnityEngine;
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

    //public Vector2Int GetSpawnPoint() => _spawnpoint;
    //public string GetNextScene() => _nextScene;

    public void SetNextScene(string sceneName)
    {
        _nextScene = sceneName;
    }

    public void SetSpawnPoint(Vector2Int point)
    {
        _spawnPoint = point;
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
}*/

using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.SceneManagement;

public class WorldmapManager : DontDestroySingleton<WorldmapManager>
{
    private Action _onWorldmapStart;
    private Action _onWorldmapEnd;

    private Vector2Int _spawnPoint;
    private string _nextScene;
    private bool _isWorldmapActive = false;

    public event Action OnWorldmapStart { add => _onWorldmapStart += value; remove => _onWorldmapStart -= value; }
    public event Action OnWorldmapEnd { add => _onWorldmapEnd += value; remove => _onWorldmapEnd -= value; }

    public async UniTask StartWorldmapAsync()
    {
        if (_isWorldmapActive) return;
        _isWorldmapActive = true;
        _onWorldmapStart?.Invoke();

        // ワールドマップシーンをロード
        await SceneManager.LoadSceneAsync("Worldmap", LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Worldmap"));

        // 決定が終わるまで待機
        await UniTask.WaitUntil(() => !_isWorldmapActive);
    }

    public void FinishWorldmap()
    {
        if (!_isWorldmapActive) return;
        _isWorldmapActive = false;

        // ワールドマップシーンを閉じる
        SceneManager.UnloadSceneAsync("Worldmap");

        _onWorldmapEnd?.Invoke();
    }

    public void SetNextScene(string sceneName) => _nextScene = sceneName;
    public void SetSpawnPoint(Vector2Int point) => _spawnPoint = point;

    public Vector2Int GetSpawnPoint() => _spawnPoint;
    public string GetNextScene() => _nextScene;
}
