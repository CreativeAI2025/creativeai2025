
/// <summary>
/// 戦闘中に表示するメッセージの定義クラスです。
/// </summary>
public class BattleMessage
{
    /// <summary>
    /// 敵キャラクター出現のメッセージのフォーマットです。
    /// </summary>
    public static readonly string EnemyAppearSuffix = "があらわれた！";

    /// <summary>
    /// 敵が複数出てきた際の出現メッセージのフォーマット
    /// </summary>
    public static readonly string EnemiesAppearSuffix = "たちが行く手を阻む！";

    /// <summary>
    /// 敵が出現数Maxの５体出てきたときの、特別メッセージのフォーマット
    /// </summary>
    public static readonly string EnemyMaxAppearText = "これは悪夢だ...！";

    /// <summary>
    /// 攻撃時のメッセージのフォーマットです。
    /// </summary>
    public static readonly string AttackSuffix = "の攻撃！";

    /// <summary>
    /// 攻撃を受ける側のメッセージのフォーマットです。
    /// </summary>
    public static readonly string DefendSuffix = "に";

    /// <summary>
    /// ダメージ量のメッセージのフォーマットです。
    /// </summary>
    public static readonly string DamageSuffix = "のダメージ！";

    /// <summary>
    /// 魔法使用時の使用者のメッセージのフォーマットです。
    /// </summary>
    public static readonly string MagicUserSuffix = "は";

    /// <summary>
    /// 魔法使用時の魔法名のメッセージのフォーマットです。
    /// </summary>
    public static readonly string MagicNameSuffix = "を唱えた！";

    /// <summary>
    /// 状態異常：毒にかかったときのメッセージ
    /// </summary>
    public static readonly string PoisonSuffix = "は毒に侵された！";

    /// <summary>
    /// 状態異常：麻痺にかかったときのメッセージ
    /// </summary>
    public static readonly string ParalysisSuffix = "は体がしびれて動けない！";

    /// <summary>
    /// 状態異常：睡眠にかかったときのメッセージ
    /// </summary>
    public static readonly string SleepSuffix = "は眠りについた！";

    /// <summary>
    /// 状態異常：火傷にかかったときのメッセージ
    /// </summary>
    public static readonly string ConfusionSuffix = "は混乱した！";


    /// <summary>
    /// 状態異常が治ったときのメッセージ
    /// </summary>
    public static readonly string RecoverStatusSuffix = "の状態異常が治った！";

    /// <summary>
    /// バフ用のメッセージ
    /// </summary>
    public static readonly string FewStatusUpSuffix = "が少し上がった";
    public static readonly string StatusUpSuffix = "が上がった";
    public static readonly string VeryStatusUpSuffix = "がとても上がった";
    /// <summary>
    /// デバフ用のメッセージ
    /// </summary>
    public static readonly string FewStatusDownSuffix = "が少し下がった";
    public static readonly string StatusDownSuffix = "が下がった";
    public static readonly string VeryStatusDownSuffix = "がとても下がった";
    /// <summary>
    /// ステータス変化用のメッセージ
    /// </summary>
    public static readonly string AttackStatusSuffix = "の攻撃力";
    public static readonly string DefenceStatusSuffix = "の防御力";
    public static readonly string MagicAttackStatusSuffix = "のスキル攻撃力";
    public static readonly string MagicDefenceStatusSuffix = "のスキル防御力";
    public static readonly string SpeedStatusSuffix = "の素早さ";
    public static readonly string EvasionStatusSuffix = "の回避率";

    /// <summary>
    /// アイテム使用時の使用者のメッセージのフォーマットです。
    /// </summary>
    public static readonly string ItemUserSuffix = "は";

    /// <summary>
    /// アイテム使用時のアイテム名のメッセージのフォーマットです。
    /// </summary>
    public static readonly string ItemNameSuffix = "を使った！";

    /// <summary>
    /// 逃走時の代表者のメッセージのフォーマットです。
    /// </summary>
    public static readonly string RunnerSuffix = "は逃げ出した！";

    /// <summary>
    /// 逃走失敗時のメッセージのフォーマットです。
    /// </summary>
    public static readonly string RunFailed = "しかし回り込まれてしまった！";

    /// <summary>
    /// HP回復のターゲットのメッセージのフォーマットです。
    /// </summary>
    public static readonly string HealTargetSuffix = "のHPが";

    /// <summary>
    /// HP回復の効果量のメッセージのフォーマットです。
    /// </summary>
    public static readonly string HealNumSuffix = "回復した！";

    /// <summary>
    /// 敵を倒した時のメッセージのフォーマットです。
    /// </summary>
    public static readonly string DefeatEnemySuffix = "をやっつけた！";

    /// <summary>
    /// 敵に倒された時のメッセージのフォーマットです。
    /// </summary>
    public static readonly string DefeatFriendSuffix = "はやられてしまった！";

    /// <summary>
    /// 全員が敵に倒された時のメッセージのフォーマットです。
    /// </summary>
    public static readonly string GameoverSuffix = "は目の前が真っ暗になった……。";

    /// <summary>
    /// 敵に勝利した時のメッセージのフォーマットです。
    /// </summary>
    public static readonly string WinSuffix = "は戦いに勝利した！";

    /// <summary>
    /// 経験値を得た時のメッセージのフォーマットです。
    /// </summary>
    public static readonly string GetExpSuffixSuffix = "の経験値を獲得。";

    /// <summary>
    /// ゴールドを得た時のメッセージのフォーマットです。
    /// </summary>
    public static readonly string GetGoldSuffixSuffix = "ゴールドを手に入れた。";

    /// <summary>
    /// レベルが上がった時の名前の後ろに表示するメッセージのフォーマットです。
    /// </summary>
    public static readonly string LevelUpNameSuffix = "のレベルが";

    /// <summary>
    /// レベルが上がった時の数値の後ろに表示するメッセージのフォーマットです。
    /// </summary>
    public static readonly string LevelUpNumberSuffix = "に上がった！";
}