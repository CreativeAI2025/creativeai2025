using UnityEngine;

 public class BattleStarter : MonoBehaviour
    {
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
        }

        /// <summary>
        /// 戦闘関連のUIを全て非表示にします。
        /// </summary>
        void HideAllUI()
        {
 _battleManager.GetWindowManager().HideAllWindow();
        }

        /// <summary>
        /// 戦闘関連のスプライトを表示します。
        /// </summary>
        void ShowSprites()
        {
 var battleSpriteController = _battleManager.GetBattleSpriteController();
            battleSpriteController.SetSpritePosition();
            battleSpriteController.ShowBackground();
            battleSpriteController.ShowEnemy(_battleManager.EnemyId);
        }

        /// <summary>
        /// 現在のステータスを表示します。
        /// </summary>
        void ShowStatus()
        {
 int characterId = 1;
            var characterStatus = CharacterStatusManager.GetCharacterStatusById(characterId);
            if (characterStatus == null)
            {
                SimpleLogger.Instance.LogWarning($"キャラクターステータスが取得できませんでした。 ID : {characterId}");
                return;
            }

            var controller = _battleManager.GetWindowManager().GetStatusWindowController();
            controller.SetCharacterStatus(characterStatus);
            controller.ShowWindow();
        }

        /// <summary>
        /// コマンド入力のUIを表示します。
        /// </summary>
        void ShowCommand()
        {

        }

        /// <summary>
        /// 敵キャラクターの名前表示ウィンドウを表示します。
        /// </summary>
        void ShowEnemyNameWindow()
        {
  var controller = _battleManager.GetWindowManager().GetEnemyNameWindowController();
            controller.ShowWindow();

            int enemyId = _battleManager.EnemyId;
            var enemyData = EnemyDataManager.GetEnemyDataById(enemyId);
            controller.SetEnemyName(enemyData.enemyName);
        }

        /// <summary>
        /// 敵キャラクターが出現したメッセージを表示します。
        /// </summary>
        void ShowEnemyAppearMessage()
        {

        }
    }