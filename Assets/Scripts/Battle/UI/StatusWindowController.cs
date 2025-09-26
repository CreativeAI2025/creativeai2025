// using UnityEngine;

// public class StatusWindowController : MonoBehaviour
// {
//     /// <summary>
//     /// ステータス表示のUIを制御するクラスへの参照です。
//     /// </summary>
//     [SerializeField]
//     StatusUIController _uiController;

//     /// <summary>
//     /// コントローラの状態をセットアップします。
//     /// </summary>
//     /// <param name="battleManager">戦闘に関する機能を管理するクラス</param>
//     public void SetUpController(BattleManager battleManager)
//     {

//     }

//     /// <summary>
//     /// キャラクターのステータスを全てセットします。
//     /// </summary>
//     /// <param name="characterStatus">キャラクターのステータス</param>
//     public void SetCharacterStatus(CharacterStatus characterStatus)
//     {
//         if (characterStatus == null)
//         {
//             SimpleLogger.Instance.LogWarning("キャラクターステータスがnullです。");
//             return;
//         }

//         var characterName = CharacterDataManager.GetCharacterName(characterStatus.characterId);
//         _uiController.SetCharacterName(characterName);

//         var level = characterStatus.level;
//         var parameterTable = CharacterDataManager.GetParameterTable(characterStatus.characterId);
//         var record = parameterTable.parameterRecords.Find(r => r.level == level);

//         _uiController.SetCurrentHp(characterStatus.currentHp);
//         _uiController.SetMaxHp(record.hp);
//         _uiController.SetCurrentMp(characterStatus.currentMp);
//         _uiController.SetMaxMp(record.mp);
//     }

//     /// <summary>
//     /// 全キャラクターのステータスを更新します。
//     /// </summary>
//     public void UpdateAllCharacterStatus()
//     {
//         foreach (var characterId in CharacterStatusManager.partyCharacter)
//         {
//             var characterStatus = CharacterStatusManager.GetCharacterStatusById(characterId);
//             SetCharacterStatus(characterStatus);
//         }
//     }

//     /// <summary>
//     /// ステータス表示のウィンドウを表示します。
//     /// </summary>
//     public void ShowWindow()
//     {
//         _uiController.Show();
//     }

//     /// <summary>
//     /// ステータス表示のウィンドウを非表示にします。
//     /// </summary>
//     public void HideWindow()
//     {
//         _uiController.Hide();
//     }
// }
