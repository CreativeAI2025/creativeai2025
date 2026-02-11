using UnityEngine;
using System;

/// <summary>
/// ゲームを初期化するためのクラス
/// </summary>
public class GameInitializer : MonoBehaviour
{
    /// <summary>
    /// 新しくゲームを開始する際にステータスやフラグなどを初期化する。
    /// </summary>
    public void InitializeGame()
    {
        Debug.Log("ロードを開始します。");
        try
        {
            CharacterStatusManager.Instance.Initialize();
            FlagManager.Instance.DeleteFlagFile();
            SkillpointManager.Instance.DeleteSkillpointFile();
        }
        catch (Exception e)
        {
            Debug.LogError($"データロード中にエラーが発生しました：{e}");
            // エラー処理など
        }
    }

    /// <summary>
    /// 続きから（＝強くてニューゲーム）を押した際に、ステータスやフラグを初期化する。
    /// </summary>
    public void ContinueGame()
    {
        CharacterStatusManager.Instance.Initialize();
        FlagManager.Instance.DeleteFlagFile();
    }
}
