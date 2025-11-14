using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class WorldmapManager : DontDestroySingleton<WorldmapManager>
{
    // イベント
    public event Action OnWorldmapStart { add => _onWorldmapStart += value; remove => _onWorldmapStart -= value; }
    private Action _onWorldmapStart;
    public event Action OnWorldmapEnd { add => _onWorldmapEnd += value; remove => _onWorldmapEnd -= value; }
    private Action _onWorldmapEnd;

    // 内部データ
    private Vector2Int _spawnPoint;
    private string _nextScene;
    private bool _isWorldmapActive = false;

    [Header("Worldmap UI")]
    [SerializeField] private GameObject worldmapCanvasPrefab;
    private GameObject worldmapCanvasInstance;

    [Header("Input設定")]
    [SerializeField] private InputSetting inputSetting;

    [Header("マップデータ")]
    [SerializeField] private WorldMapData worldMapData;

    private GameObject[] mapPointObjects;
    private int currentIndex = 0;
    private int previousIndex = -1;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private float scaleMultiplier = 1.2f;

    //=====================================================
    // 起動（ObjectEngine から呼ばれる）
    //=====================================================
    public async void StartWorldmapAsync()
    {
        if (_isWorldmapActive) return;

        _isWorldmapActive = true;
        _onWorldmapStart?.Invoke();

        // UIを生成
        if (worldmapCanvasPrefab != null)
            worldmapCanvasInstance = Instantiate(worldmapCanvasPrefab);

        // WorldMapのUI初期化
        InitializeMapUI();
        // 入力ループ
        await HandleInputLoop();

        // 終了処理
        EndWorldmap();
    }

    //=====================================================
    // 終了
    //=====================================================
    private void EndWorldmap()
    {
        _onWorldmapEnd?.Invoke();
    }

    public void SetNextScene(string sceneName) => _nextScene = sceneName;
    public void SetSpawnPoint(Vector2Int point) => _spawnPoint = point;

    public Vector2Int GetSpawnPoint() => _spawnPoint;
    public string GetNextScene() => _nextScene;

    //=====================================================
    // UI初期化
    //=====================================================
    void InitializeMapUI()
    {
        if (worldMapData == null)
        {
            Debug.LogError("WorldMapData が設定されていません！");
            return;
        }

        // マップポイント生成
        mapPointObjects = new GameObject[worldMapData.mapPoints.Length];
        for (int i = 0; i < worldMapData.mapPoints.Length; i++)
        {
            GameObject point = new GameObject("MapPoint_" + i);
            point.transform.SetParent(worldmapCanvasInstance.transform, false);
            Image img = point.AddComponent<Image>();
            img.sprite = worldMapData.mapPoints[i].icon;
            mapPointObjects[i] = point;
        }

        ApplyMapPointPositions();
        UpdateUnlockStatesFromFlags();
        UpdateSelectionVisuals();
    }

    void ApplyMapPointPositions()
    {
        for (int i = 0; i < mapPointObjects.Length; i++)
        {
            RectTransform rt = mapPointObjects[i].GetComponent<RectTransform>();
            if (rt != null)
                rt.anchoredPosition = worldMapData.mapPoints[i].position;
        }
        ApplyIconsToMapPoints();
    }

    void ApplyIconsToMapPoints()
    {
        for (int i = 0; i < mapPointObjects.Length; i++)
        {
            var image = mapPointObjects[i].GetComponent<Image>();
            if (image != null && worldMapData.mapPoints[i].icon != null)
                image.sprite = worldMapData.mapPoints[i].icon;
        }
    }

    //=====================================================
    // 入力ループ
    //=====================================================
    async UniTask HandleInputLoop()
    {
        while (_isWorldmapActive)
        {
            HandleInput();
            await UniTask.Yield();
        }
    }

    void HandleInput()
    {
        int nextIndex = -1;

        // 上下左右入力
        if (inputSetting.GetForwardKeyDown()) nextIndex = GetNeighborIndex(Direction.Up);
        if (inputSetting.GetBackKeyDown()) nextIndex = GetNeighborIndex(Direction.Down);
        if (inputSetting.GetLeftKeyDown()) nextIndex = GetNeighborIndex(Direction.Left);
        if (inputSetting.GetRightKeyDown()) nextIndex = GetNeighborIndex(Direction.Right);

        if (nextIndex != -1 && worldMapData.IsAreaUnlocked(nextIndex))
        {
            previousIndex = currentIndex;
            currentIndex = nextIndex;
            UpdateSelectionVisuals();
        }

        // 決定
        if (inputSetting.GetDecideInputDown())
        {
            if (worldMapData.IsAreaUnlocked(currentIndex))
            {
                PlayerPrefs.SetInt("LastWorldMapIndex", currentIndex);
                PlayerPrefs.Save();

                // 次のシーンと出現地点を設定して終了
                SetNextScene(worldMapData.mapPoints[currentIndex].sceneName);
                SetSpawnPoint(worldMapData.mapPoints[currentIndex].spawnPosition);
                _isWorldmapActive = false;
                Debug.Log("マップ選択された");
            }
            else
            {
                Debug.Log("このエリアはまだ解放されていません");
            }
        }
    }

    enum Direction { Up, Down, Left, Right }

    int GetNeighborIndex(Direction dir)
    {
        if (worldMapData == null || currentIndex >= worldMapData.mapPoints.Length)
            return -1;

        WorldMapData.MapPointData current = worldMapData.mapPoints[currentIndex];
        int[] neighborIndices = worldMapData.GetNeighborIndices(currentIndex);
        int bestIndex = -1;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < neighborIndices.Length; i++)
        {
            int idx = neighborIndices[i];
            if (idx >= worldMapData.mapPoints.Length || !worldMapData.IsAreaUnlocked(idx))
                continue;

            Vector2 dirVec = worldMapData.mapPoints[idx].position - current.position;
            switch (dir)
            {
                case Direction.Up:
                    if (dirVec.y > 0 && Mathf.Abs(dirVec.x) < Mathf.Abs(dirVec.y) && dirVec.y < bestDistance)
                    { bestDistance = dirVec.y; bestIndex = idx; }
                    break;
                case Direction.Down:
                    if (dirVec.y < 0 && Mathf.Abs(dirVec.x) < Mathf.Abs(dirVec.y) && -dirVec.y < bestDistance)
                    { bestDistance = -dirVec.y; bestIndex = idx; }
                    break;
                case Direction.Left:
                    if (dirVec.x < 0 && Mathf.Abs(dirVec.y) < Mathf.Abs(dirVec.x) && -dirVec.x < bestDistance)
                    { bestDistance = -dirVec.x; bestIndex = idx; }
                    break;
                case Direction.Right:
                    if (dirVec.x > 0 && Mathf.Abs(dirVec.y) < Mathf.Abs(dirVec.x) && dirVec.x < bestDistance)
                    { bestDistance = dirVec.x; bestIndex = idx; }
                    break;
            }
        }
        return bestIndex;
    }

    //=====================================================
    // 解放状態・UI更新
    //=====================================================
    void UpdateUnlockStatesFromFlags()
    {
        if (worldMapData == null || FlagManager.Instance == null)
            return;

        worldMapData.UpdateUnlockStatesFromAreaIds(areaId =>
        {
            return FlagManager.Instance.HasFlag(areaId);
        });
    }

    public void UnlockArea(int index)
    {
        if (worldMapData == null || index >= worldMapData.mapPoints.Length)
            return;

        WorldMapData.MapPointData point = worldMapData.mapPoints[index];
        if (!string.IsNullOrEmpty(point.areaId) && FlagManager.Instance != null)
            FlagManager.Instance.AddFlag(point.areaId);

        worldMapData.UnlockArea(index);
    }

    void UpdateSelectionVisuals()
    {
        if (mapPointObjects == null) return;

        if (previousIndex >= 0 && previousIndex < mapPointObjects.Length)
            ResetVisuals(previousIndex);

        if (currentIndex >= 0 && currentIndex < mapPointObjects.Length)
            HighlightVisuals(currentIndex);
    }

    void HighlightVisuals(int index)
    {
        GameObject mapPoint = mapPointObjects[index];
        var image = mapPoint.GetComponent<Image>();
        if (image != null)
            image.color = selectedColor;

        StartCoroutine(AnimateScale(mapPoint, Vector3.one * scaleMultiplier, 0.2f));
    }

    void ResetVisuals(int index)
    {
        GameObject mapPoint = mapPointObjects[index];
        var image = mapPoint.GetComponent<Image>();
        if (image != null)
            image.color = normalColor;

        StartCoroutine(AnimateScale(mapPoint, Vector3.one, 0.15f));
    }

    IEnumerator AnimateScale(GameObject target, Vector3 targetScale, float duration)
    {
        Vector3 startScale = target.transform.localScale;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float eased = EaseOutBack(t);
            target.transform.localScale = Vector3.Lerp(startScale, targetScale, eased);
            yield return null;
        }
        target.transform.localScale = targetScale;
    }

    float EaseOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }
}
