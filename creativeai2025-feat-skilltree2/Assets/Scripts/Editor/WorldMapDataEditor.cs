using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldMapData))]
public class WorldMapDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WorldMapData worldMap = (WorldMapData)target;
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("ワールドマップ設定", EditorStyles.boldLabel);
        
        // 基本情報
        worldMap.worldName = EditorGUILayout.TextField("ワールド名", worldMap.worldName);
        worldMap.backgroundImage = (Sprite)EditorGUILayout.ObjectField("背景画像", worldMap.backgroundImage, typeof(Sprite), false);
        worldMap.startingPointIndex = EditorGUILayout.IntField("開始ポイントインデックス", worldMap.startingPointIndex);
        
        EditorGUILayout.Space();
        
        // マップポイント管理
        EditorGUILayout.LabelField("マップポイント管理", EditorStyles.boldLabel);
        
        if (GUILayout.Button("新しいポイントを追加"))
        {
            AddNewMapPoint(worldMap);
        }
        
        if (worldMap.mapPoints != null)
        {
            EditorGUILayout.LabelField($"総ポイント数: {worldMap.mapPoints.Length}");
            
            for (int i = 0; i < worldMap.mapPoints.Length; i++)
            {
                DrawMapPoint(worldMap, i);
            }
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("接続確認"))
        {
            ValidateConnections(worldMap);
        }
        
        if (GUI.changed)
        {
            EditorUtility.SetDirty(worldMap);
        }
    }
    
    private void DrawMapPoint(WorldMapData worldMap, int index)
    {
        var point = worldMap.mapPoints[index];
        
        EditorGUILayout.BeginVertical("box");
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"ポイント {index}: {point.pointName}", EditorStyles.boldLabel);
        
        if (GUILayout.Button("削除", GUILayout.Width(50)))
        {
            RemoveMapPoint(worldMap, index);
            return;
        }
        EditorGUILayout.EndHorizontal();
        
        point.pointName = EditorGUILayout.TextField("名前", point.pointName);
        point.sceneName = EditorGUILayout.TextField("シーン名", point.sceneName);
        point.position = EditorGUILayout.Vector2Field("位置", point.position);
        point.unlockedByDefault = EditorGUILayout.Toggle("初期解放", point.unlockedByDefault);
        point.description = EditorGUILayout.TextField("説明", point.description);
        
        // 隣接エリア設定
        EditorGUILayout.LabelField("隣接エリア:");
        EditorGUI.indentLevel++;
        
        if (point.neighborIndices == null) point.neighborIndices = new int[0];
        
        int newSize = EditorGUILayout.IntField("隣接数", point.neighborIndices.Length);
        if (newSize != point.neighborIndices.Length)
        {
            System.Array.Resize(ref point.neighborIndices, newSize);
        }
        
        for (int i = 0; i < point.neighborIndices.Length; i++)
        {
            point.neighborIndices[i] = EditorGUILayout.IntField($"隣接 {i}", point.neighborIndices[i]);
        }
        
        EditorGUI.indentLevel--;
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }
    
    private void AddNewMapPoint(WorldMapData worldMap)
    {
        if (worldMap.mapPoints == null)
        {
            worldMap.mapPoints = new WorldMapData.MapPointData[1];
        }
        else
        {
            System.Array.Resize(ref worldMap.mapPoints, worldMap.mapPoints.Length + 1);
        }
        
        worldMap.mapPoints[worldMap.mapPoints.Length - 1] = new WorldMapData.MapPointData();
        EditorUtility.SetDirty(worldMap);
    }
    
    private void RemoveMapPoint(WorldMapData worldMap, int index)
    {
        if (worldMap.mapPoints == null || index < 0 || index >= worldMap.mapPoints.Length) return;
        
        var newArray = new WorldMapData.MapPointData[worldMap.mapPoints.Length - 1];
        for (int i = 0, j = 0; i < worldMap.mapPoints.Length; i++)
        {
            if (i != index)
            {
                newArray[j++] = worldMap.mapPoints[i];
            }
        }
        
        worldMap.mapPoints = newArray;
        EditorUtility.SetDirty(worldMap);
    }
    
    private void ValidateConnections(WorldMapData worldMap)
    {
        if (worldMap.mapPoints == null) return;
        
        bool hasErrors = false;
        
        for (int i = 0; i < worldMap.mapPoints.Length; i++)
        {
            var point = worldMap.mapPoints[i];
            if (point.neighborIndices == null) continue;
            
            foreach (int neighborIndex in point.neighborIndices)
            {
                if (neighborIndex < 0 || neighborIndex >= worldMap.mapPoints.Length)
                {
                    Debug.LogError($"ポイント{i}の隣接インデックス{neighborIndex}が無効です");
                    hasErrors = true;
                }
            }
        }
        
        if (!hasErrors)
        {
            Debug.Log("すべての接続が正常です！");
        }
    }
}
