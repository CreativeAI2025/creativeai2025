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
    /// キー入力状態を保存するフラグ
    /// </summary>
    public bool _waitKeyInput;

    private InputSetting _inputSetting;

    void Start()
    {
        _inputSetting = InputSetting.Load();
    }

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
    /// 敵出現時のメッセージを生成します。
    /// </summary>
    public void GenerateEnemyAppearMessage(string enemyName, float appearInterval)
    {
        uiController.ClearMessage();
        string message = $"{enemyName}{BattleMessage.EnemyAppearSuffix}";
        StartCoroutine(ShowMessageAutoProcess(message, appearInterval));
    }

    /// <summary>
    /// 敵出現時のメッセージを、引数で与えられたメッセージをそのまま表示する。
    /// </summary>
    /// <param name="message"></param>
    /// <param name="appearInterval"></param>
    public void GenerateEnemyAppearMessageDirect(string message, float appearInterval)
    {
        uiController.ClearMessage();
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
        StartCoroutine(ShowMessageWaitInputProcess(message));
    }

    /// <summary>
    /// 敵を撃破した時のメッセージを生成します。
    /// </summary>
    public void GenerateDefeateEnemyMessage(string targetName)
    {
        string message = $"{targetName}{BattleMessage.DefeatEnemySuffix}";
        StartCoroutine(ShowMessageWaitInputProcess(message));
    }

    /// <summary>
    /// 味方がやられた時のメッセージを生成します。
    /// </summary>
    public void GenerateDefeateFriendMessage(string targetName)
    {
        string message = $"{targetName}{BattleMessage.DefeatFriendSuffix}";
        StartCoroutine(ShowMessageWaitInputProcess(message));
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
    public void GenerateRecoverStatusMessage(string targetName, string buffMessage, float buffValue)
    {
        string message = "";
        if (1.0 < buffValue && buffValue <= 1.2)
        {
            message = $"{targetName}{buffMessage}{BattleMessage.FewStatusUpSuffix}";
        }
        else if (1.2<buffValue && buffValue <= 1.7)
        {
            message = $"{targetName}{buffMessage}{BattleMessage.StatusUpSuffix}";
        }
        else if (buffValue > 1.7)
        {
            message = $"{targetName}{buffMessage}{BattleMessage.VeryStatusUpSuffix}";
        }
        else if (1.0>buffValue && buffValue>=0.8)
        {
            message = $"{targetName}{buffMessage}{BattleMessage.FewStatusDownSuffix}";
        }
        else if (0.8>buffValue && buffValue>= 0.6)
        {
            message = $"{targetName}{buffMessage}{BattleMessage.StatusDownSuffix}";
        }
        else if (0.6>buffValue )
        {
            message = $"{targetName}{buffMessage}{BattleMessage.VeryStatusDownSuffix}";
        }
        StartCoroutine(ShowMessageAutoProcess(message));
    }

    /// <summary>
    /// HPが回復する時のメッセージを生成します。
    /// </summary>
    public void GenerateHpHealMessage(string targetName, int healNum)
    {
        string message = $"{targetName}{BattleMessage.HealTargetSuffix} {healNum} {BattleMessage.HealNumSuffix}";
        StartCoroutine(ShowMessageWaitInputProcess(message));
    }
    /// <summary>
    /// MPが回復する時のメッセージを生成します。
    /// </summary>
    public void GenerateMpHealMessage(string targetName, int healNum)
    {
        string message = $"{targetName}{BattleMessage.HealTargetSuffix} {healNum} {BattleMessage.HealNumSuffix}";
        StartCoroutine(ShowMessageWaitInputProcess(message));
    }

    /// <summary>
    /// 生き返ったときのメッセージを表示
    /// </summary>
    /// <param name="targetName"></param>
    /// <param name="hpValue"></param>
    public void GenerateReviveMessage(string targetName, int hpValue)
    {
        string message = $"{targetName} は蘇生した！ HPが {hpValue} 回復した！";
        StartCoroutine(ShowMessageWaitInputProcess(message));
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
        StartCoroutine(ShowMessageWaitInputProcess(message));
    }

    /// <summary>
    /// 逃走が失敗した時のメッセージを生成します。
    /// </summary>
    public void GenerateRunFailedMessage()
    {
        string message = BattleMessage.RunFailed;
        StartCoroutine(ShowMessageWaitInputProcess(message));
    }

    /// <summary>
    /// ゲームオーバー時のメッセージを生成します。
    /// </summary>
    public void GenerateGameoverMessage(string characterName)
    {
        uiController.ClearMessage();
        string message = $"{characterName}{BattleMessage.GameoverSuffix}";
        StartCoroutine(ShowMessageWaitInputProcess(message));
    }

    /// <summary>
    /// 戦闘勝利時のメッセージを生成します。
    /// </summary>
    public void GenerateWinMessage(string characterName)
    {
        uiController.ClearMessage();
        string message = $"{characterName}{BattleMessage.WinSuffix}";
        StartCoroutine(ShowMessageWaitInputProcess(message));
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

    public void ClearMessage()
    {
        uiController.ClearMessage();
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

    /// <summary>
    /// 決定キーを入力するまで、次のメッセージを表示しないコルーチン
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    IEnumerator ShowMessageWaitInputProcess(string message)
    {
        // メッセージの追記
        uiController.AppendMessage(message);

        yield return new WaitForSeconds(_messageInterval);

        // 1. ページャーを表示してキー入力を促す
        ShowPager();

        // 2. ユーザー入力を待つ
        _waitKeyInput = true;
        while (_waitKeyInput)
        {
            // InputGameKey.cs を利用して決定ボタンが押されたかチェック
            if (_inputSetting.GetDecideInputDown())
            {
                _waitKeyInput = false; // 待機状態を解除
            }
            yield return null; // 毎フレーム待機
        }

        // 3. 待機解除後、クリーンアップ
        HidePager();

        // 4. BattleManagerに処理完了を通知（BattleActionProcessor.SetPauseMessage(false)へ繋がる）
        BattleManager.Instance.OnFinishedShowMessage();
    }
}