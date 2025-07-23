using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 戦闘中のアクションを処理するクラスです。
/// </summary>
public class BattleActionProcessor : MonoBehaviour
{
    /// <summary>
    /// ターン内のアクションのリストです。
    /// </summary>
    List<BattleAction> _actions = new();

    /// <summary>
    /// ターン内のアクションのリストを初期化します。
    /// </summary>
    public void InitializeActions()
    {
        _actions.Clear();
    }

    /// <summary>
    /// ターン内のアクションのリストに要素を追加します。
    /// </summary>
    public void RegisterAction(BattleAction action)
    {
        _actions.Add(action);
    }
}