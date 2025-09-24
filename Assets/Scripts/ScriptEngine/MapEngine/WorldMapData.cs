using UnityEngine;

[CreateAssetMenu(fileName = "WorldMapData", menuName = "Map/WorldMapData")]
public class WorldMapData : ScriptableObject
{
    [Header("ワールドマップ設定")]
    public string worldName = "ワールド名";
    public Sprite backgroundImage;
    
    [Header("マップポイント一覧")]
    public MapPointData[] mapPoints;
    
    [Header("初期設定")]
    public int startingPointIndex = 0; 
    
    [System.Serializable]
    public class MapPointData
    {
        [Header("基本情報")]
        public string pointName = "エリア名";
        public string sceneName = "シーン名";
        public string areaId = ""; // フラグシステムで使用するエリアID
        
        [Header("位置")]
        public Vector2 position = Vector2.zero;
        
        [Header("状態")]
        public bool unlockedByDefault = false;
        
        [Header("隣接エリア（インデックス）")]
        public int[] neighborIndices = new int[0];
        
        [Header("UI表示")]
        public Sprite icon;
        public string description = "エリアの説明";
        
        // 実行時の状態（外部フラグシステムから取得）
        [System.NonSerialized]
        public bool isUnlocked;
    }
    
    /// <summary>
    /// 指定したインデックスのMapPointDataを取得
    /// </summary>
    public MapPointData GetMapPoint(int index)
    {
        if (index >= 0 && index < mapPoints.Length)
        {
            return mapPoints[index];
        }
        return null;
    }
    
    /// <summary>
    /// 隣接エリアのインデックスを取得
    /// </summary>
    public int[] GetNeighborIndices(int pointIndex)
    {
        MapPointData point = GetMapPoint(pointIndex);
        return point?.neighborIndices ?? new int[0];
    }
    
    /// <summary>
    /// 初期化：デフォルト解放状態を設定
    /// </summary>
    public void InitializeUnlockStates()
    {
        for (int i = 0; i < mapPoints.Length; i++)
        {
            mapPoints[i].isUnlocked = mapPoints[i].unlockedByDefault;
        }
    }
    
    /// <summary>
    /// 外部フラグシステムから解放状態を更新（インデックス指定）
    /// </summary>
    /// <param name="flagCheckFunction">フラグチェック関数（インデックス指定）</param>
    public void UpdateUnlockStatesFromFlags(System.Func<int, bool> flagCheckFunction)
    {
        for (int i = 0; i < mapPoints.Length; i++)
        {
            // デフォルト解放 または フラグチェック関数で解放判定
            mapPoints[i].isUnlocked = mapPoints[i].unlockedByDefault || flagCheckFunction(i);
        }
    }
    
    /// <summary>
    /// 外部フラグシステムから解放状態を更新（エリアID指定）
    /// </summary>
    /// <param name="flagCheckFunction">フラグチェック関数（エリアID指定）</param>
    public void UpdateUnlockStatesFromAreaIds(System.Func<string, bool> flagCheckFunction)
    {
        for (int i = 0; i < mapPoints.Length; i++)
        {
            bool unlocked = mapPoints[i].unlockedByDefault;
            
            // エリアIDが設定されている場合はフラグをチェック
            if (!string.IsNullOrEmpty(mapPoints[i].areaId))
            {
                unlocked = unlocked || flagCheckFunction(mapPoints[i].areaId);
            }
            
            mapPoints[i].isUnlocked = unlocked;
        }
    }
    
    /// <summary>
    /// エリアを解放
    /// </summary>
    public void UnlockArea(int index)
    {
        MapPointData point = GetMapPoint(index);
        if (point != null)
        {
            point.isUnlocked = true;
        }
    }
    
    /// <summary>
    /// エリアがアンロックされているかチェック
    /// </summary>
    public bool IsAreaUnlocked(int index)
    {
        MapPointData point = GetMapPoint(index);
        return point?.isUnlocked ?? false;
    }
    
    /// <summary>
    /// FlagManagerを使用して解放状態を更新
    /// </summary>
    public void UpdateUnlockStatesFromFlagManager()
    {
        if (FlagManager.Instance == null)
        {
            Debug.LogWarning("FlagManager.Instance が null です。デフォルト状態で初期化します。");
            InitializeUnlockStates();
            return;
        }
        
        for (int i = 0; i < mapPoints.Length; i++)
        {
            bool unlocked = mapPoints[i].unlockedByDefault;
            
            // エリアIDが設定されている場合はFlagManagerでフラグをチェック
            if (!string.IsNullOrEmpty(mapPoints[i].areaId))
            {
                unlocked = unlocked || FlagManager.Instance.HasFlag(mapPoints[i].areaId);
            }
            
            mapPoints[i].isUnlocked = unlocked;
        }
    }
    
    /// <summary>
    /// 指定されたエリアのフラグを立てて解放する
    /// </summary>
    /// <param name="index">マップポイントのインデックス</param>
    public void UnlockAreaWithFlag(int index)
    {
        MapPointData point = GetMapPoint(index);
        if (point == null) return;
        
        // エリアIDが設定されている場合はFlagManagerでフラグを立てる
        if (!string.IsNullOrEmpty(point.areaId) && FlagManager.Instance != null)
        {
            FlagManager.Instance.AddFlag(point.areaId);
            Debug.Log($"エリア '{point.pointName}' のフラグ '{point.areaId}' を設定しました");
        }
        
        // 解放状態を更新
        point.isUnlocked = true;
    }
    
    /// <summary>
    /// すべてのエリアのフラグ状態をデバッグ出力
    /// </summary>
    public void DebugPrintFlagStates()
    {
        if (FlagManager.Instance == null)
        {
            Debug.Log("FlagManager.Instance が null です");
            return;
        }
        
        string output = "=== ワールドマップ フラグ状態 ===\n";
        for (int i = 0; i < mapPoints.Length; i++)
        {
            MapPointData point = mapPoints[i];
            string flagStatus = "フラグなし";
            
            if (!string.IsNullOrEmpty(point.areaId))
            {
                bool hasFlag = FlagManager.Instance.HasFlag(point.areaId);
                flagStatus = hasFlag ? "解放済み" : "未解放";
            }
            
            output += $"[{i}] {point.pointName} ({point.areaId}): {flagStatus} | isUnlocked: {point.isUnlocked}\n";
        }
        
        Debug.Log(output);
    }
    
    /// <summary>
    /// エリア名からインデックスを検索
    /// </summary>
    /// <param name="areaId">エリアID</param>
    /// <returns>見つからない場合は-1</returns>
    public int FindAreaIndexByAreaId(string areaId)
    {
        for (int i = 0; i < mapPoints.Length; i++)
        {
            if (mapPoints[i].areaId == areaId)
                return i;
        }
        return -1;
    }
    
    /// <summary>
    /// エリアIDでエリアを解放
    /// </summary>
    /// <param name="areaId">エリアID</param>
    /// <returns>解放に成功したかどうか</returns>
    public bool UnlockAreaByAreaId(string areaId)
    {
        int index = FindAreaIndexByAreaId(areaId);
        if (index >= 0)
        {
            UnlockAreaWithFlag(index);
            return true;
        }
        
        Debug.LogWarning($"エリアID '{areaId}' が見つかりませんでした");
        return false;
    }
    
    /// <summary>
    /// ゲーム進行に応じてエリアを段階的に解放
    /// </summary>
    public void UpdateProgressBasedUnlocks()
    {
        if (FlagManager.Instance == null) return;
        
        // チュートリアル完了でステージ1解放
        if (FlagManager.Instance.HasFlag("tutorial_completed"))
        {
            UnlockAreaByAreaId("stage1_forest_unlocked");
        }
        
        // ステージ1クリアでステージ2解放
        if (FlagManager.Instance.HasFlag("stage1_completed"))
        {
            UnlockAreaByAreaId("stage2_mountain_unlocked");
        }
        
        // ステージ2クリアでステージ3解放
        if (FlagManager.Instance.HasFlag("stage2_completed"))
        {
            UnlockAreaByAreaId("stage3_desert_unlocked");
        }
        
        // 必要に応じて他の解放条件も追加...
        
        // 解放状態を更新
        UpdateUnlockStatesFromFlagManager();
    }
}

