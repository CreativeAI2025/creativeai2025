using UnityEngine;
using System;

/// <summary>
/// ゲームを初期化するためのクラス
/// 不変データなどの取得をここで行う。
/// </summary>
public class GameInitializer : MonoBehaviour
{
    private async void Start()
    {
        /// <summary>
        /// パーティキャラクター（ゾフィ、リナ、ノア）のデータを登録するためのクラス
        /// </summary>
        CharacterDataManager characterDataManager = CharacterDataManager.Instance;
        /// <summary>
        /// パーティキャラクターの状態に関する登録（パーティメンバーの初期化（最初はゾフィのみ）、所持金の初期化など）を行うためのクラス
        /// </summary>
        CharacterStatusManager characterStatusManager = CharacterStatusManager.Instance;
        Debug.Log("ロードを開始します。");
        try
        {
            await characterDataManager.Initialize();
            Debug.Log("CharacterDataManagerのロード完了");
            characterStatusManager.Initialize();
            Debug.Log("CharacterStatusManagerの初期化完了");
        }
        catch (Exception e)
        {
            Debug.LogError($"データロード中にエラーが発生しました：{e}");
            // エラー処理など
        }
    }
}
