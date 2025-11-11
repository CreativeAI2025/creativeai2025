using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldMapController : MonoBehaviour
{
    [SerializeField] private InputSetting inputSetting;
    [Header("選択表現")]
    public GameObject[] mapPointObjects; // 各マップポイントのGameObject
    public Color selectedColor = Color.yellow; // 選択時の色
    public Color normalColor = Color.white; // 通常時の色
    public float scaleMultiplier = 1.2f; // 選択時のスケール倍率
    public float moveSpeed = 10f; // アニメーション速度

    [Header("マップデータ")]
    public WorldMapData worldMapData; // ワールドマップデータ
    private int currentIndex = 0;
    private int previousIndex = -1;

    [Header("背景画像")]
    public Image backgroundUIImage; // Canvas 上の Image をアサイン

    void Start()
    {
        // 背景画像を設定
        if (backgroundUIImage != null && worldMapData != null && worldMapData.backgroundImage != null)
        {
            backgroundUIImage.sprite = worldMapData.backgroundImage;
            backgroundUIImage.preserveAspect = true; // 画像の縦横比を保持
        }

        // ワールドマップデータが設定されていない場合はエラー
        if (worldMapData == null || worldMapData.mapPoints == null || worldMapData.mapPoints.Length == 0)
        {
            Debug.LogError("WorldMapData が設定されていないか、マップポイントが空です");
            return;
        }

        // UIにWorldMapData.positionを反映
        ApplyMapPointPositions();

        // 開始ポイントを設定
        if (PlayerPrefs.HasKey("LastWorldMapIndex"))
            currentIndex = PlayerPrefs.GetInt("LastWorldMapIndex");
        else
            currentIndex = worldMapData.startingPointIndex;

        if (currentIndex >= worldMapData.mapPoints.Length)
            currentIndex = 0;
            
        // 解放状態を初期化
        worldMapData.InitializeUnlockStates();
        UpdateUnlockStatesFromFlags();
        
        // 初期選択状態を設定
        UpdateSelectionVisuals();
    }

    void Update()
    {
        HandleInput();
    }

    void ApplyMapPointPositions()
    {
        for (int i = 0; i < mapPointObjects.Length; i++)
        {
            RectTransform rt = mapPointObjects[i].GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = worldMapData.mapPoints[i].position;
            }
        }
        ApplyIconsToMapPoints();
    }

    void ApplyIconsToMapPoints()
    {
        for (int i = 0; i < mapPointObjects.Length; i++)
        {
            var image = mapPointObjects[i].GetComponent<UnityEngine.UI.Image>();
            if (image != null && worldMapData.mapPoints[i].icon != null)
            {
                image.sprite = worldMapData.mapPoints[i].icon;
            }
        }
    }

    void HandleInput()
    {
        int nextIndex = -1;

        // 上下左右入力
        if (inputSetting.GetForwardKeyDown()) nextIndex = GetNeighborIndex(Direction.Up);
        if (inputSetting.GetBackKeyDown()) nextIndex = GetNeighborIndex(Direction.Down);
        if (inputSetting.GetLeftKeyDown()) nextIndex = GetNeighborIndex(Direction.Left);
        if (inputSetting.GetRightKeyDown())   nextIndex = GetNeighborIndex(Direction.Right);


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
                var selectedPoint = worldMapData.mapPoints[currentIndex];

                if (WorldmapManager.Instance != null)
                {
                    WorldmapManager.Instance.SetNextScene(selectedPoint.sceneName);
                    WorldmapManager.Instance.SetSpawnPoint(selectedPoint.spawnPosition);
                    // ワールドマップ終了を通知
                    WorldmapManager.Instance.FinishWorldmap();
                }

                SceneManager.LoadScene(selectedPoint.sceneName);
            }
            else
            {
                Debug.Log("このエリアはまだ解放されていません");
            }
        }
    }

    // 隣接エリアを方向で取得
    int GetNeighborIndex(Direction dir)
    {
        if (worldMapData == null || currentIndex >= worldMapData.mapPoints.Length)
            return -1;
            
        WorldMapData.MapPointData current = worldMapData.mapPoints[currentIndex];
        int[] neighborIndices = worldMapData.GetNeighborIndices(currentIndex);

        Debug.Log($"Current = {currentIndex}, neighbors = {string.Join(",", neighborIndices)}");

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

    enum Direction { Up, Down, Left, Right }
    
    /// <summary>
    /// FlagManagerからの情報で解放状態を更新
    /// </summary>
    void UpdateUnlockStatesFromFlags()
    {
        if (worldMapData == null || FlagManager.Instance == null)
            return;
            
        // エリアIDベースでフラグチェック
        worldMapData.UpdateUnlockStatesFromAreaIds((areaId) => 
        {
            return FlagManager.Instance.HasFlag(areaId);
        });
    }
    
    /// <summary>
    /// エリアを解放（フラグを設定）
    /// </summary>
    /// <param name="index">マップポイントのインデックス</param>
    public void UnlockArea(int index)
    {
        if (worldMapData == null || index >= worldMapData.mapPoints.Length)
            return;
            
        WorldMapData.MapPointData point = worldMapData.mapPoints[index];
        
        // エリアIDが設定されている場合はフラグを立てる
        if (!string.IsNullOrEmpty(point.areaId) && FlagManager.Instance != null)
        {
            FlagManager.Instance.AddFlag(point.areaId);
        }
        
        // WorldMapDataの解放状態も更新
        worldMapData.UnlockArea(index);
    }
    
    /// <summary>
    /// 選択状態の視覚的表現を更新
    /// </summary>
    void UpdateSelectionVisuals()
    {
        if (mapPointObjects == null) return;
        
        // 前回の選択を通常状態に戻す
        if (previousIndex >= 0 && previousIndex < mapPointObjects.Length && mapPointObjects[previousIndex] != null)
        {
            ResetVisuals(previousIndex);
        }
        
        // 現在の選択を強調表示
        if (currentIndex >= 0 && currentIndex < mapPointObjects.Length && mapPointObjects[currentIndex] != null)
        {
            HighlightVisuals(currentIndex);
        }
    }
    
    /// <summary>
    /// 指定されたマップポイントを強調表示
    /// </summary>
    void HighlightVisuals(int index)
    {
        GameObject mapPoint = mapPointObjects[index];
        
        // 色の変更
        var image = mapPoint.GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            image.color = selectedColor;
        }
        
        // スケールの変更（アニメーション付き）
        StartCoroutine(AnimateScale(mapPoint, Vector3.one * scaleMultiplier, 0.2f));
    }
    
    /// <summary>
    /// 指定されたマップポイントを通常状態に戻す
    /// </summary>
    void ResetVisuals(int index)
    {
        GameObject mapPoint = mapPointObjects[index];
        
        // 色をリセット
        var image = mapPoint.GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            image.color = normalColor;
        }
        
        // スケールをリセット（アニメーション付き）
        StartCoroutine(AnimateScale(mapPoint, Vector3.one, 0.15f));
    }
    
    /// <summary>
    /// スケールをアニメーションで変更
    /// </summary>
    /// <param name="target">対象のGameObject</param>
    /// <param name="targetScale">目標スケール</param>
    /// <param name="duration">アニメーション時間</param>
    IEnumerator AnimateScale(GameObject target, Vector3 targetScale, float duration)
    {
        Vector3 startScale = target.transform.localScale;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            
            // イージング効果（EaseOutBack風）
            float easedProgress = EaseOutBack(progress);
            
            target.transform.localScale = Vector3.Lerp(startScale, targetScale, easedProgress);
            yield return null;
        }
        
        target.transform.localScale = targetScale;
    }
    
    /// <summary>
    /// EaseOutBack風のイージング関数
    /// </summary>
    /// <param name="t">進行度 (0-1)</param>
    /// <returns>イージング適用後の値</returns>
    float EaseOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }
}

