using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// 戦闘中のアクションを処理するクラスです。
/// </summary>
public class BattleActionProcessor : MonoBehaviour
{
    /// <summary>
    /// 戦闘中のアクション間の実行間隔です。
    /// </summary>
    [SerializeField]
    float _actionInterval = 0.5f;

    /// <summary>
    /// 戦闘に関する機能を管理するクラスへの参照です。
    /// </summary>
    BattleManager _battleManager;

    /// <summary>
    /// 戦闘中の味方キャラクターの管理を行うクラスへの参照です。
    /// </summary>
    CharacterStatusManager _characterStatusManager;
    /// <summary>
    /// 戦闘中の攻撃アクションを処理するクラスへの参照です。
    /// </summary>
    [SerializeField]
    BattleActionProcessorAttack _battleActionProcessorAttack;

    /// <summary>
    /// 戦闘中の魔法アクションを処理するクラスへの参照です。
    /// </summary>
    [SerializeField]
    BattleActionProcessorSkill _battleActionProcessorSkill;

    /// <summary>
    /// 戦闘中のアイテムアクションを処理するクラスへの参照です。
    /// </summary>
    [SerializeField]
    BattleActionProcessorItem _battleActionProcessorItem;

    /// <summary>
    /// 戦闘中の逃げるアクションを処理するクラスへの参照です。
    /// </summary>
    [SerializeField]
    BattleActionProcessorRun _battleActionProcessorRun;
    /// <summary>
    /// ターン内のアクションのリストです。
    /// </summary>
    List<BattleAction> _actions = new();

    /// <summary>
    /// アクションを処理するコルーチンへの参照です。
    /// </summary>
    Coroutine _processActionCoroutine;

    /// <summary>
    /// プロセスを一時停止するかどうかのフラグです。
    /// </summary>
    public bool IsPausedProcess { get; private set; }

    /// <summary>
    /// メッセージをポーズするかどうかのフラグです。
    /// </summary>
    public bool IsPausedMessage { get; private set; }

    /// <summary>
    /// このクラスを初期化します。
    /// </summary>
    /// <param name="battleManager">戦闘に関する機能を管理するクラスへの参照</param>
    public void InitializeProcessor(BattleManager battleManager)
    {
        _battleManager = battleManager;

        _battleActionProcessorAttack.SetReferences(_battleManager, this);
        _battleActionProcessorSkill.SetReferences(_battleManager, this);
        _battleActionProcessorItem.SetReferences(_battleManager, this);
        _battleActionProcessorRun.SetReferences(_battleManager, this);
    }

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
    /// <param name="action">追加するアクション</param>
    public void RegisterAction(BattleAction action)
    {
        _actions.Add(action);
    }

    /// <summary>
    /// アクションリストの要素の優先度を設定します。
    /// </summary>
    public void SetPriorities()
    {
        // アクションコマンドの素早さの値に20%の乱数を乗じます。
        foreach (var action in _actions)
        {
            action.actorSpeed = (int)(action.actorSpeed * (1 + Random.Range(-0.2f, 0.2f)));
        }

        // 遅い順に並べ替え、優先度を1から順に設定します。
        // 優先度が大きい方を先に処理します。
        var query = _actions.OrderBy(a => a.actorSpeed);
        int priority = 1;
        int runPriority = 100;
        foreach (var action in query)
        {
            if (action.battleCommand == BattleCommand.Run)
            {
                action.priority = runPriority;
                continue;
            }
            action.priority = priority;
            priority++;
        }
    }

    /// <summary>
    /// アクションリストの内容を優先度に応じて処理していきます。
    /// </summary>
    public void StartActions()
    {
        OutputActionOrder();
        IsPausedMessage = false;
        IsPausedProcess = false;
        _processActionCoroutine = StartCoroutine(ProcessAction());
    }

    /// <summary>
    /// アクションリストの内容を優先度に応じて処理していきます。
    /// </summary>
    IEnumerator ProcessAction()
    {
        var query = _actions.OrderByDescending(a => a.priority).ToList();
        foreach (var action in query)
        {
            Logger.Instance.Log($"キャラクターの行動を開始します。action.priority : {action.priority}");
            if (_battleManager.IsBattleFinished)
            {
                Logger.Instance.Log("戦闘が終了しているため、処理を中断します。");
                yield break;
            }
            // ここで行動不能チェック
            if (action.isActorFriend)
            {
                var charaStatus = CharacterStatusManager.Instance.GetCharacterStatusById(action.actorId);
                if (charaStatus != null && (charaStatus.IsCharaStop || charaStatus.isDefeated))
                {
                    Logger.Instance.Log($"味方 {action.actorId} は行動不能（倒された/状態異常）で動けなかった！");
                    continue;
                }
            }
            else
            {
                var enemyStatus = EnemyStatusManager.Instance.GetEnemyStatusByBattleId(action.actorId);
                if (enemyStatus != null && (enemyStatus.IsEnemyStop || enemyStatus.isDefeated))
                {
                    Logger.Instance.Log($"敵 {action.actorId} は行動不能（倒された/状態異常）で動けなかった！");
                    continue;
                }
            }
            Logger.Instance.Log($"コマンドに応じた行動を行います。 コマンド : {action.battleCommand}");

            switch (action.battleCommand)
            {
                case BattleCommand.Attack:
                    _battleActionProcessorAttack.ProcessAction(action);
                    break;
                case BattleCommand.Skill:
                    _battleActionProcessorSkill.ProcessAction(action);
                    break;
                case BattleCommand.Item:
                    _battleActionProcessorItem.ProcessAction(action);
                    break;
                case BattleCommand.Run:
                    _battleActionProcessorRun.ProcessAction(action);
                    break;
            }


            while (IsPausedProcess)
            {
                yield return null;
            }

            yield return new WaitForSeconds(_actionInterval);
        }

        // ターン内の行動が完了したことを通知します。
        _battleManager.OnFinishedActions();
    }

    /// <summary>
    /// アクションの処理を停止します。
    /// </summary>
    public void StopActions()
    {
        if (_processActionCoroutine != null)
        {
            StopCoroutine(_processActionCoroutine);
        }
    }

    /// <summary>
    /// キャラクターの戦闘用パラメータを取得します。
    /// </summary>
    /// <param name="charcterId">キャラクターのID</param>
    /// <param name="isFriend">味方かどうか</param>
    public BattleParameter GetCharacterParameter(int charcterId, bool isFriend)
    {
        BattleParameter battleParameter = new();

        if (isFriend)
        {
            battleParameter = CharacterStatusManager.Instance.GetCharacterBattleParameterById(charcterId);
        }
        else
        {
            var enemyStatus = EnemyStatusManager.Instance.GetEnemyStatusByBattleId(charcterId);
            var enemyData = enemyStatus.enemyData;
            battleParameter.Attack = enemyData.Attack;
            battleParameter.Defence = enemyData.Defence;
            battleParameter.Speed = enemyData.Speed;
            battleParameter.MagicAttack = enemyData.MagicAttack;
            battleParameter.MagicDefence = enemyData.MagicDefence;
            battleParameter.Evasion = enemyData.Evasion;

        }
        return battleParameter;
    }

    /// <summary>
    /// キャラクターの名前を取得します。
    /// </summary>
    /// <param name="charcterId">キャラクターのID</param>
    /// <param name="isFriend">味方かどうか</param>
    public string GetCharacterName(int charcterId, bool isFriend)
    {
        string name;
        if (isFriend)
        {
            var characterData = CharacterDataManager.Instance.GetCharacterData(charcterId);
            name = characterData.characterName;
        }
        else
        {
            var enemyStatus = EnemyStatusManager.Instance.GetEnemyStatusByBattleId(charcterId);
            name = enemyStatus.enemyData.enemyName;
        }
        return name;
    }

    /// <summary>
    /// 次のメッセージを表示します。
    /// もしメッセージの表示か完了していたら、次のアクションを処理します。
    /// </summary>
    public void ShowNextMessage()
    {
        SetPauseMessage(false);
    }

    /// <summary>
    /// プロセスを一時停止するかどうかを設定します。
    /// Trueで一時停止します。
    /// </summary>
    /// <param name="puase">一時停止するかどうか</param>
    public void SetPauseProcess(bool puase)
    {
        IsPausedProcess = puase;
    }

    /// <summary>
    /// メッセージを一時停止するかどうかを設定します。
    /// Trueで一時停止します。
    /// </summary>
    /// <param name="puase">一時停止するかどうか</param>
    public void SetPauseMessage(bool puase)
    {
        IsPausedMessage = puase;
    }

    /// <summary>
    /// デバッグ用機能
    /// キャラクターの行動順をコンソールに出力します。
    /// </summary>
    void OutputActionOrder()
    {
        Logger.Instance.Log("行動順を出力します。");
        var query = _actions.OrderBy(a => a.priority);
        foreach (var action in query)
        {
            Logger.Instance.Log($"キャラクターID : {action.actorId}, 味方かどうか : {action.isActorFriend}, Priority : {action.priority}, Command : {action.battleCommand}");
        }
    }

    /// <summary>
    /// アクション実行前に、ターゲットリストから倒れているキャラクターを除外します。
    /// </summary>
    /// <param name="targetIds">ターゲットのIDリスト</param>
    /// <param name="isTargetFriend">ターゲットが味方かどうか</param>
    /// <returns>有効なターゲットIDのみを含む新しいリスト</returns>
    public List<int> GetValidTargets(List<int> targetIds, bool isTargetFriend)
    {
        if (isTargetFriend)
        {
            // 味方リストから、倒れていないキャラクターのみを抽出
            return targetIds.Where(id => !CharacterStatusManager.Instance.IsCharacterDefeated(id)).ToList();
        }
        else
        {
            // 敵リストから、倒れていない（isDefeated/isRunawayではない）敵のみを抽出
            return targetIds.Where(id =>
                !EnemyStatusManager.Instance.IsEnemyDefeated(id) &&
                !EnemyStatusManager.Instance.IsEnemyRunaway(id)).ToList();
        }
    }
}
