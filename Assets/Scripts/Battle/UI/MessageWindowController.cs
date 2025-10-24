using System.Collections;
using UnityEngine;

/// <summary>
/// メッセージウィンドウの動作を制御するクラスです。
/// </summary>
public class MessageWindowController : MonoBehaviour, IBattleWindowController
{
    /// <summary>
    /// メッセージウィンドウのUIを制御するクラスへの参照です。
    /// </summary>
    [SerializeField]
    MessageUIController uiController;

    /// <summary>
    /// メッセージの表示間隔です。
    /// </summary>
    [SerializeField]
    float _messageInterval = 1.0f;

    /// <summary>
    /// ウィンドウを表示します。
    /// </summary>
    public void ShowWindow()
    {
        uiController.ClearMessage();
        uiController.Show();
    }

    /// <summary>
    /// ウィンドウを非表示にします。
    /// </summary>
    public void HideWindow()
    {
        uiController.ClearMessage();
        uiController.Hide();
    }

    /// <summary>
    /// 攻撃時のメッセージを生成します。
    /// </summary>
    public void GenerateEnemyAppearMessage(string enemyName, float appearInterval)
    {
        uiController.ClearMessage();
        string message = $"{enemyName}{BattleMessage.EnemyAppearSuffix}";
        StartCoroutine(ShowMessageAutoProcess(message, appearInterval));
    }

    /// <summary>
    /// 攻撃時のメッセージを生成します。
    /// </summary>
    public void GenerateAttackMessage(string attackerName)
    {
        uiController.ClearMessage();
        string message = $"{attackerName}{BattleMessage.AttackSuffix}";
        StartCoroutine(ShowMessageAutoProcess(message));
    }

    /// <summary>
    /// ダメージ発生時のメッセージを生成します。
    /// </summary>
    public void GenerateDamageMessage(string targetName, int damage)
    {
        string message = $"{targetName}{BattleMessage.DefendSuffix} {damage} {BattleMessage.DamageSuffix}";
        StartCoroutine(ShowMessageAutoProcess(message));
    }

    /// <summary>
    /// 敵を撃破した時のメッセージを生成します。
    /// </summary>
    public void GenerateDefeateEnemyMessage(string targetName)
    {
        string message = $"{targetName}{BattleMessage.DefeatEnemySuffix}";
        StartCoroutine(ShowMessageAutoProcess(message));
    }

    /// <summary>
    /// 味方がやられた時のメッセージを生成します。
    /// </summary>
    public void GenerateDefeateFriendMessage(string targetName)
    {
        string message = $"{targetName}{BattleMessage.DefeatFriendSuffix}";
        StartCoroutine(ShowMessageAutoProcess(message));
    }

    /// <summary>
    /// 魔法を唱えた時のメッセージを生成します。
    /// </summary>
    public void GenerateSkillCastMessage(string magicUserName, string magicName)
    {
        uiController.ClearMessage();
        string message = $"{magicUserName}{BattleMessage.MagicUserSuffix} {magicName} {BattleMessage.MagicNameSuffix}";
        StartCoroutine(ShowMessageAutoProcess(message));
    }

    /// <summary>
    /// 状態異常が発生したときのメッセージを表示します。
    /// </summary>
    public void GenerateStatusAilmentMessage(string targetName, string statusMessage)
    {
        string message = $"{targetName}{BattleMessage.MagicUserSuffix}{statusMessage}";
        StartCoroutine(ShowMessageAutoProcess(message));
    }

    /// <summary>
    /// 状態異常が回復したときのメッセージを表示します。
    /// </summary>
    public void GenerateRecoverStatusMessage(string targetName)
    {
        string message = $"{targetName}{BattleMessage.MagicUserSuffix}{BattleMessage.RecoverStatusSuffix}";
        StartCoroutine(ShowMessageAutoProcess(message));
    }

    /// <summary>
    ///バフデバフを受けたときのメッセージを表示します。
    /// </summary>
    public void GenerateRecoverStatusMessage(string targetName, string buffMessage, int buffValue)
    {
        string message = "";
        if (buffValue < 1.2)
        {
            message = $"{targetName}{buffMessage}{BattleMessage.FewStatusUpSuffix}";
        }
        else if (buffValue < 1.7)
        {
            message = $"{targetName}{buffMessage}{BattleMessage.StatusUpSuffix}";
        }
        else if (buffValue > 1.7)
        {
            message = $"{targetName}{buffMessage}{BattleMessage.VeryStatusUpSuffix}";
        }
        StartCoroutine(ShowMessageAutoProcess(message));
    }

    /// <summary>
    /// HPが回復する時のメッセージを生成します。
    /// </summary>
    public void GenerateHpHealMessage(string targetName, int healNum)
    {
        string message = $"{targetName}{BattleMessage.HealTargetSuffix} {healNum} {BattleMessage.HealNumSuffix}";
        StartCoroutine(ShowMessageAutoProcess(message));
    }
    /// <summary>
    /// MPが回復する時のメッセージを生成します。
    /// </summary>
    public void GenerateMpHealMessage(string targetName, int healNum)
    {
        string message = $"{targetName}{BattleMessage.HealTargetSuffix} {healNum} {BattleMessage.HealNumSuffix}";
        StartCoroutine(ShowMessageAutoProcess(message));
    }
    public void GenerateReviveMessage(string targetName, int hpValue)
    {
        string message = $"{targetName} は蘇生した！ HPが {hpValue} 回復した！";
        StartCoroutine(ShowMessageAutoProcess(message));
    }
    /// <summary>
    /// アイテムを使用した時のメッセージを生成します。
    /// </summary>
    public void GenerateUseItemMessage(string itemUserName, string itemName)
    {
        uiController.ClearMessage();
        string message = $"{itemUserName}{BattleMessage.ItemUserSuffix} {itemName} {BattleMessage.ItemNameSuffix}";
        StartCoroutine(ShowMessageAutoProcess(message));
    }

    /// <summary>
    /// 逃走した時のメッセージを生成します。
    /// </summary>
    public void GenerateRunMessage(string characterName)
    {
        uiController.ClearMessage();
        string message = $"{characterName}{BattleMessage.RunnerSuffix}";
        StartCoroutine(ShowMessageAutoProcess(message));
    }

    /// <summary>
    /// 逃走が失敗した時のメッセージを生成します。
    /// </summary>
    public void GenerateRunFailedMessage()
    {
        string message = BattleMessage.RunFailed;
        StartCoroutine(ShowMessageAutoProcess(message));
    }

    /// <summary>
    /// ゲームオーバー時のメッセージを生成します。
    /// </summary>
    public void GenerateGameoverMessage(string characterName)
    {
        uiController.ClearMessage();
        string message = $"{characterName}{BattleMessage.GameoverSuffix}";
        StartCoroutine(ShowMessageAutoProcess(message));
    }

    /// <summary>
    /// 戦闘勝利時のメッセージを生成します。
    /// </summary>
    public void GenerateWinMessage(string characterName)
    {
        uiController.ClearMessage();
        string message = $"{characterName}{BattleMessage.WinSuffix}";
        StartCoroutine(ShowMessageAutoProcess(message));
    }

    /// <summary>
    /// Exp獲得時のメッセージを生成します。
    /// </summary>
    public void GenerateGetExpMessage(int exp)
    {
        string message = $"{exp} {BattleMessage.GetExpSuffixSuffix}";
        StartCoroutine(ShowMessageAutoProcess(message));
    }

    /// <summary>
    /// ゴールド獲得時のメッセージを生成します。
    /// </summary>
    public void GenerateGetGoldMessage(int gold)
    {
        string message = $"{gold} {BattleMessage.GetGoldSuffixSuffix}";
        StartCoroutine(ShowMessageAutoProcess(message));
    }

    /// <summary>
    /// レベルアップ時のメッセージを生成します。
    /// </summary>
    public void GenerateLevelUpMessage(string characterName, int level)
    {
        uiController.ClearMessage();
        string message = $"{characterName}{BattleMessage.LevelUpNameSuffix} {level} {BattleMessage.LevelUpNumberSuffix}";
        StartCoroutine(ShowMessageAutoProcess(message));
    }

    /// <summary>
    /// ページ送りのカーソルを表示します。
    /// </summary>
    public void ShowPager()
    {
        uiController.ShowCursor();
    }

    /// <summary>
    /// ページ送りのカーソルを非表示にします。
    /// </summary>
    public void HidePager()
    {
        uiController.HideCursor();
    }

    /// <summary>
    /// メッセージを順番に表示するコルーチンです。
    /// </summary>
    IEnumerator ShowMessageAutoProcess(string message)
    {
        uiController.AppendMessage(message);
        yield return new WaitForSeconds(_messageInterval);
        BattleManager.Instance.OnFinishedShowMessage();
    }

    /// <summary>
    /// メッセージを順番に表示するコルーチンです。
    /// </summary>
    /// <param name="message">表示するメッセージ</param>
    /// <param name="interval">表示間隔</param>
    IEnumerator ShowMessageAutoProcess(string message, float interval)
    {
        uiController.AppendMessage(message);
        yield return new WaitForSeconds(interval);
        BattleManager.Instance.OnFinishedShowMessage();
    }
}