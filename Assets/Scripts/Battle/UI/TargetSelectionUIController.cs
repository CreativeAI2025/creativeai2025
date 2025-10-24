using UnityEngine;
using System.Collections.Generic;

public class TargetSelectionUIController : MonoBehaviour, IBattleUIController
{
    // 💡 Inspectorで設定する、各ターゲットマーカーのGameObjectのリスト
    [SerializeField]
    private List<GameObject> _targetMarkers;

    // 現在表示すべき有効なターゲットの数
    private int _currentActiveTargets = 0;

    /// <summary>
    /// UIを初期化し、表示するターゲット数を設定します。
    /// 💡 このメソッド定義がCS1739エラーを解消します。
    /// </summary>
    /// <param name="targetCount">アクティブなターゲットの総数</param>
    public void Initialize(int targetCount)
    {
        _currentActiveTargets = targetCount;

        // 全てのマーカー/カーソルを非表示にしてリセット
        HideAllCursors();

        // ここで、ターゲットの数に応じてHPバーなどのUIを有効化する処理が入ります
        for (int i = 0; i < _targetMarkers.Count; i++)
        {
            // ターゲット数が _targetMarkers のインデックス内なら、カーソルオブジェクトをアクティブにする
            if (i < targetCount)
            {
                // _targetMarkers[i].gameObject.SetActive(true); // マーカー自体がアクティブになるべきならここを有効化
            }
        }
    }

    /// <summary>
    /// 選択中のカーソルを表示します。
    /// </summary>
    /// <param name="index">選択中のターゲットのインデックス（_activeTargetsリストに対応）</param>
    public void ShowSelectedCursor(int index)
    {
        HideAllCursors();
        if (index >= 0 && index < _targetMarkers.Count)
        {
            // 💡 選択されているターゲットのカーソルを表示
            _targetMarkers[index].SetActive(true);
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

    /// <summary>
    /// UIを表示します。
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// UIを非表示にします。
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}