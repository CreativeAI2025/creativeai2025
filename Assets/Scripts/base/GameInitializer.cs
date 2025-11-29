using UnityEngine;
using System;

/// <summary>
/// ゲームを初期化するためのクラス
/// </summary>
public class GameInitializer : MonoBehaviour
{
    /*
    private async void Start()
    {
        InitializeGame();
    }
    */

    public async void InitializeGame()
    {
        /// <summary>
        /// パーティキャラクター（ゾフィ、リナ、ノア）のデータを登録するためのクラス
        /// このデータは不変
        /// </summary>
        CharacterDataManager characterDataManager = CharacterDataManager.Instance;
        /// <summary>
        /// アイテムの定義データを登録するためのクラス
        /// このデータは不変
        /// </summary>
        ItemDataManager itemDatamanager = ItemDataManager.Instance;
        /// <summary>
        /// スキル定義データを登録するためのクラス
        /// </summary>
        SkillDataManager skillDataManager = SkillDataManager.Instance;
        /// <summary>
        /// パーティキャラクターの状態に関する登録（パーティメンバーの初期化（最初はゾフィのみ）、所持金の初期化など）を行うためのクラス
        /// </summary>
        CharacterStatusManager characterStatusManager = CharacterStatusManager.Instance;
        /// <summary>
        /// 敵キャラクターのデータを登録するためのクラス
        /// </summary>
        EnemyDataManager enemyDataManager = EnemyDataManager.Instance;
        Debug.Log("ロードを開始します。");
        try
        {
            await characterDataManager.Initialize();
            await itemDatamanager.Initialize();
            characterStatusManager.Initialize();
            await skillDataManager.Initialize();
            FlagManager.Instance.DeleteFlagFile();
            await enemyDataManager.Initialize();
        }
        catch (Exception e)
        {
            Debug.LogError($"データロード中にエラーが発生しました：{e}");
            // エラー処理など
        }
    }
}
