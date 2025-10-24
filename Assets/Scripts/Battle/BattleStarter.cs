using UnityEngine;


/// <summary>
/// 戦闘の開始処理を行うクラスです。
/// </summary>
public class BattleStarter : MonoBehaviour
{
    /// <summary>
    /// 戦闘開始メッセージを表示する時間です。
    /// </summary>
    [SerializeField]
    float _startMessageTime = 10.0f;
    /// <summary>
    /// 戦闘の管理を行うクラスへの参照です。
    /// </summary>
    BattleManager _battleManager;

    /// <summary>
    /// 戦闘の開始処理を行います。
    /// </summary>
    public void StartBattle(BattleManager battleManager)
    {
        _battleManager = battleManager;

        // 戦闘関連のUIを非表示にします。
        HideAllUI();

        // スプライトを表示します。
        ShowSprites();

        // ステータスのUIを表示します。
        ShowStatus();

        // コマンドウィンドウを表示します。
        ShowCommand();

        // 敵の名前ウィンドウを表示します。
        ShowEnemyNameWindow();
        // 敵出現のメッセージを表示します。
        ShowEnemyAppearMessage();

        // テスト用機能
        //BattleManager.Instance.StartInputCommandPhase();
    }

    /// <summary>
    /// 戦闘関連のUIを全て非表示にします。
    /// </summary>
    void HideAllUI()
    {
        BattleManager.Instance.GetWindowManager().HideAllWindow();
    }

    /// <summary>
    /// 戦闘関連のスプライトを表示します。
    /// </summary>
    void ShowSprites()
    {
        var battleSpriteController = BattleManager.Instance.GetBattleSpriteController();
        //battleSpriteController.SetSpritePosition();
        //battleSpriteController.ShowBackground();
        battleSpriteController.ShowEnemy(BattleManager.Instance.EnemyIds);
    }

    /// <summary>
    /// 現在のステータスを表示します。
    /// </summary>
    void ShowStatus()
    {
        // ここは将来いらない
        int characterId = 1;
        var characterStatus = CharacterStatusManager.Instance.GetCharacterStatusById(characterId);
        if (characterStatus == null)
        {
            Logger.Instance.LogWarning($"キャラクターステータスが取得できませんでした。 ID : {characterId}");
            return;
        }

        var controller = BattleManager.Instance.GetWindowManager().GetStatusWindowController();
        controller.SetCharacterStatus(characterStatus);
        controller.ShowWindow();
    }

    /// <summary>
    /// コマンド入力のUIを表示します。
    /// </summary>
    void ShowCommand()
    {
        var controller = BattleManager.Instance.GetWindowManager().GetCommandWindowController();
        controller.ShowWindow();
        controller.InitializeCommand();
    }

    /// <summary>
    /// 敵キャラクターの名前表示ウィンドウを表示します。
    /// </summary>
    void ShowEnemyNameWindow()
    {
        var controller = BattleManager.Instance.GetWindowManager().GetEnemyNameWindowController();
        controller.ShowWindow();

        var enemyIds = BattleManager.Instance.EnemyIds;
        var enemyData = EnemyDataManager.Instance.GetEnemyDataById(enemyIds[0]);
        controller.SetEnemyName(enemyData.enemyName);
    }

    /// <summary>
    /// 敵キャラクターが出現したメッセージを表示します。
    /// </summary>
    void ShowEnemyAppearMessage()
    {
        int enemyId = BattleManager.Instance.EnemyIds[0];
        var enemyData = EnemyDataManager.Instance.GetEnemyDataById(enemyId);
        if (enemyData == null)
        {
            Logger.Instance.LogWarning($"敵データが取得できませんでした。 ID : {enemyId}");
            return;
        }

        // メッセージ表示後、BattleManagerに制御が戻ります。
        var controller = BattleManager.Instance.GetWindowManager().GetMessageWindowController();
        controller.ShowWindow();
        controller.GenerateEnemyAppearMessage(enemyData.enemyName, _startMessageTime);
    }
}