using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class TargetSelectionUIController : MonoBehaviour, IBattleUIController
{
    // 💡 Inspectorで設定する、各ターゲットマーカーのGameObjectのリスト
    [SerializeField]
    private List<GameObject> _targetMarkers;

    [SerializeField] private List<TextMeshProUGUI> _targetNameTexts;

    private int _currentActiveTargets = 0;

    /// <summary>
    /// UIを初期化し、表示するターゲット数を設定します。
    /// </summary>
    /// <param name="targetCount">アクティブなターゲットの総数（例: 生存している敵/味方の数）</param>
    public void Initialize(int targetCount, List<string> targetNames)
    {
        _currentActiveTargets = targetCount;

        // 全てのUIをリセット
        HideAllCursors();
        ClearAllNames(); // 💡 名前をクリア

        // 💡 追記: 有効なターゲットの名前を設定
        for (int i = 0; i < targetCount; i++)
        {
            if (i < _targetNameTexts.Count && i < targetNames.Count)
            {
                // UI上のマーカー位置 (i) に、ターゲット名 (targetNames[i]) を設定
                _targetNameTexts[i].text = targetNames[i];
                _targetNameTexts[i].gameObject.SetActive(true);
            }
        }

        // ... (HPバーやその他のUIの表示/非表示ロジック) ...
    }

    /// <summary>
    /// 選択中のカーソルを表示します。
    /// 💡 index は WindowController から渡される、オフセット適用済みの絶対インデックスです。
    /// </summary>
    /// <param name="index">UIマーカーのインデックス (0 = 味方1, 3 = 敵1 など)</param>
    public void ShowSelectedCursor(int index)
    {
        HideAllCursors();
        if (index >= 0 && index < _targetMarkers.Count)
        {
            // 💡 該当するインデックスのUIマーカーをアクティブにする
            _targetMarkers[index].SetActive(true);
        }
    }

    /// <summary>
    /// 💡 新規: 全体ターゲットのカーソルを全て表示します。
    /// </summary>
    /// <param name="activeCount">有効なターゲットの数</param>
    /// <param name="offset">UIマーカーの開始位置（味方なら0、敵なら3など）</param>
    public void ShowAllActiveCursors(int activeCount)
    {
        HideAllCursors();

        for (int i = 0; i < activeCount; i++)
        {
            int cursorIndex = i;
            if (cursorIndex < _targetMarkers.Count)
            {
                _targetMarkers[cursorIndex].SetActive(true); // 全てアクティブに
            }
        }
    }

    /// <summary>
    /// 全てのカーソルを非表示にします。
    /// </summary>
    private void HideAllCursors()
    {
        foreach (var marker in _targetMarkers)
        {
            marker.SetActive(false);
        }
    }

    // 全ての名前テキストを非表示にするヘルパー
    private void ClearAllNames()
    {
        foreach (var text in _targetNameTexts)
        {
            text.gameObject.SetActive(false); // または text.text = string.Empty;
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}