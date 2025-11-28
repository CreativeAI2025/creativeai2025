// #if UNITY_EDITOR
// using UnityEngine;
// using UnityEditor;
// using System.IO;

// public class SkillScriptableObjectCreater : EditorWindow
// {
//     private string filePath = "";                  // 読み込む JSON のパス
//     private DefaultAsset folderAsset = null;       // ScriptableObject を保存するフォルダ
//     private string outputFolderPath = "";          // 保存先の絶対パス

//     [MenuItem("Tools/Skill/Skill ScriptableObject Creater")]
//     public static void ShowWindow()
//     {
//         GetWindow<SkillScriptableObjectCreater>("Skill SO Creater");
//     }

//     private void OnGUI()
//     {
//         GUILayout.Label("Skill ScriptableObject Generator", EditorStyles.boldLabel);

//         // JSONファイルパス
//         filePath = EditorGUILayout.TextField("JSON File Path", filePath);

//         // 保存フォルダの選択
//         folderAsset = (DefaultAsset)EditorGUILayout.ObjectField(
//             "Output Folder",
//             folderAsset,
//             typeof(DefaultAsset),
//             false
//         );

//         // 実際のパスへ変換
//         if (folderAsset != null)
//         {
//             outputFolderPath = AssetDatabase.GetAssetPath(folderAsset);
//             EditorGUILayout.LabelField("Folder Path", outputFolderPath);
//         }

//         GUILayout.Space(20);

//         if (GUILayout.Button("Generate ScriptableObjects"))
//         {
//             GenerateScriptableObjects();
//         }
//     }

//     private void GenerateScriptableObjects()
//     {
//         if (string.IsNullOrEmpty(filePath))
//         {
//             Debug.LogError("JSON ファイルパスが未入力です。");
//             return;
//         }

//         if (folderAsset == null)
//         {
//             Debug.LogError("保存先フォルダが設定されていません。");
//             return;
//         }

//         if (!File.Exists(filePath))
//         {
//             Debug.LogError("指定された JSON ファイルが存在しません: " + filePath);
//             return;
//         }

//         // JSON読み込み
//         string json = File.ReadAllText(filePath);

//         // JSON → SkillData 型または List<SkillData> 型にパースする処理をここに書く
//         // （以下はサンプル）
//         SkillData sample = JsonUtility.FromJson<SkillData>(json);

//         // ScriptableObject 作成
//         SkillScriptableObject asset = ScriptableObject.CreateInstance<SkillScriptableObject>();
//         asset.skillName = sample.name;
//         asset.mpCost = sample.mpCost;
//         asset.power = sample.power;

//         // アセット保存
//         string savePath = $"{outputFolderPath}/{asset.skillName}.asset";
//         AssetDatabase.CreateAsset(asset, savePath);
//         AssetDatabase.SaveAssets();

//         Debug.Log($"ScriptableObject を生成しました: {savePath}");
//     }
// }

// [System.Serializable]
// public class SkillData
// {
//     public string skillName;
//     public int mpCost;
//     public int power;
// }

// public class SkillScriptableObject : ScriptableObject
// {
//     public string skillName;
//     public int mpCost;
//     public int power;
// }
// #endif
