using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleActionProcessorSkill : MonoBehaviour
{
    BattleActionProcessor _actionProcessor;
    BattleManager _battleManager;
    MessageWindowController _messageWindowController;
    BattleSpriteController _battleSpriteController;

    // 追加: 状態異常マネージャー
    [SerializeField] private StatusEffectManager statusEffectManager;

    bool _pauseSkillEffect;

    // 追記: ターゲットIDリストを生成するヘルパーメソッド
    private List<int> GetEffectiveTargetIds(BattleAction action, SkillData skillData)
    {
        var skillEffect = skillData.skillEffects.FirstOrDefault(); // 最初の効果の範囲を使うと仮定
        if (skillEffect == null) return new List<int>();

        // ターゲット属性を判定し、対象リストを生成
        switch (skillEffect.EffectTarget)
        {
            case EffectTarget.Own:
                return new List<int> { action.actorId };

            case EffectTarget.FriendSolo:
                return action.targetIds; // ターゲット選択UIで選択されたID（単体）をそのまま使用

            case EffectTarget.FriendAll:
                // 全味方キャラクターのIDを返す
                return CharacterStatusManager.Instance.partyCharacter.Where(id =>
                    !CharacterStatusManager.Instance.IsCharacterDefeated(id)).ToList();

            case EffectTarget.EnemySolo:
                return action.targetIds; // ターゲット選択UIで選択された敵ID（単体）をそのまま使用

            case EffectTarget.EnemyAll:
                // 全ての敵キャラクターの戦闘中IDを返す
                return EnemyStatusManager.Instance.GetEnemyStatusList().Where(status =>
                    !status.isDefeated && !status.isRunaway).Select(status => status.enemyBattleId).ToList();

            default:
                return action.targetIds; // デフォルトで登録されたターゲットIDリストを返す
        }
    }

    // 追記: ターゲットIDが味方か敵か判定するヘルパーメソッド
    private bool IsTargetFriend(int targetId, bool isActorFriend, SkillEffect skillEffect)
    {
        // 味方がアクションを行った場合、回復/バフは味方、ダメージは敵
        if (isActorFriend)
        {
            return skillEffect.EffectTarget == EffectTarget.Own ||
                   skillEffect.EffectTarget == EffectTarget.FriendSolo ||
                   skillEffect.EffectTarget == EffectTarget.FriendAll;
        }
        // 敵がアクションを行った場合
        else
        {
            // 敵の回復/バフは敵、ダメージは味方
            return skillEffect.EffectTarget != EffectTarget.EnemySolo &&
                   skillEffect.EffectTarget != EffectTarget.EnemyAll;
        }
    }


    IEnumerator ProcessSkillActionCoroutine(BattleAction action, SkillData skillData)
    {
        // ... (既存の消費MP処理) ...

        _actionProcessor.SetPauseProcess(true);
        List<int> effectiveTargetIds = GetEffectiveTargetIds(action, skillData);

        // 魔法詠唱メッセージを一度だけ表示
        string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
        _actionProcessor.SetPauseMessage(true);
        _messageWindowController.GenerateSkillCastMessage(actorName, skillData.skillName);
        while (_actionProcessor.IsPausedMessage) yield return null;

        // ----------------------------------------------------
        // 💡 複数ターゲットへの効果適用ループ
        // ----------------------------------------------------
        foreach (int currentTargetId in effectiveTargetIds)
        {
            Logger.Instance.Log($"ターゲット ID:{currentTargetId} への処理を開始。");

            foreach (var skillEffect in skillData.skillEffects)
            {
                // ... (ターゲット判定、ダメージ計算、ステータス変更のロジックはそのまま) ...

                // --- ダメージ計算と適用 ---
                if (skillEffect.skillCategory == SkillCategory.Damage)
                {
                    int damageValue = 100; // ダメージ計算式を適用してください
                    int hpDelta = -damageValue;
                    int mpDelta = 0;
                    bool isTargetFriend = IsTargetFriend(currentTargetId, action.isActorFriend, skillEffect);
                    bool isTargetDefeated = false;

                    // ステータス変更
                    if (isTargetFriend)
                    {
                        CharacterStatusManager.Instance.ChangeCharacterStatus(currentTargetId, hpDelta, mpDelta);
                        isTargetDefeated = CharacterStatusManager.Instance.IsCharacterDefeated(currentTargetId);
                    }
                    else
                    {
                        EnemyStatusManager.Instance.ChangeEnemyStatus(currentTargetId, hpDelta, mpDelta);
                        isTargetDefeated = EnemyStatusManager.Instance.IsEnemyDefeated(currentTargetId);

                        if (isTargetDefeated)
                            EnemyStatusManager.Instance.OnDefeatEnemy(currentTargetId);
                    }

                    // ダメージメッセージ表示
                    _actionProcessor.SetPauseMessage(true); // 修正: メッセージポーズをセット
                    string targetName = _actionProcessor.GetCharacterName(currentTargetId, isTargetFriend);
                    _messageWindowController.GenerateDamageMessage(targetName, damageValue);
                    _battleManager.OnUpdateStatus();
                    while (_actionProcessor.IsPausedMessage) yield return null;

                    // 撃破メッセージ表示
                    if (isTargetDefeated)
                    {
                        _actionProcessor.SetPauseMessage(true);
                        if (isTargetFriend)
                        {
                            _messageWindowController.GenerateDefeateFriendMessage(targetName);
                        }
                        else
                        {
                            // 💡 修正: 敵スプライトの更新
                            _battleSpriteController.RefreshActiveEnemies();
                            _messageWindowController.GenerateDefeateEnemyMessage(targetName);
                        }
                        while (_actionProcessor.IsPausedMessage) yield return null;

                        // 戦闘終了判定
                        if (EnemyStatusManager.Instance.IsAllEnemyDefeated())
                            _battleManager.OnEnemyDefeated();
                        if (CharacterStatusManager.Instance.IsAllCharacterDefeated())
                            _battleManager.OnGameover();
                        if (!_battleManager.IsBattleFinished)
                        {
                            _actionProcessor.SetPauseProcess(false);
                        }
                    }
                }
                // --- 回復計算と適用 ---
                else if (skillEffect.skillCategory == SkillCategory.Recovery)
                {
                    // ... (回復ロジック、メッセージ表示はそのまま) ...
                    int healValue = DamageFormula.CalculateHealValue(skillEffect.value);
                    bool isTargetFriend = IsTargetFriend(currentTargetId, action.isActorFriend, skillEffect);

                    // ステータス変更
                    if (isTargetFriend)
                        CharacterStatusManager.Instance.ChangeCharacterStatus(currentTargetId, healValue, 0);
                    else
                        EnemyStatusManager.Instance.ChangeEnemyStatus(currentTargetId, healValue, 0);

                    // 回復メッセージ表示
                    _pauseSkillEffect = true;
                    string targetName = _actionProcessor.GetCharacterName(currentTargetId, isTargetFriend);
                    _messageWindowController.GenerateHpHealMessage(targetName, healValue);
                    _battleManager.OnUpdateStatus();
                    while (_pauseSkillEffect) yield return null;
                }
                // ... （他の効果も同様）
            }
            // ❌ 削除: _actionProcessor.SetPauseProcess(false);
            // ❌ 削除: yield break; 
        }

        // 💡 修正: ループがすべて完了した後、戦闘が終了していない場合にのみプロセスを再開
        if (!_battleManager.IsBattleFinished)
        {
            _actionProcessor.SetPauseProcess(false);
        }
    }

    public void SetReferences(BattleManager battleManager, BattleActionProcessor actionProcessor)
    {
        _battleManager = battleManager;
        _actionProcessor = actionProcessor;
        _messageWindowController = _battleManager.GetWindowManager().GetMessageWindowController();
        _battleSpriteController = _battleManager.GetBattleSpriteController();
        statusEffectManager = _battleManager.GetStatusEffectManager();
    }

    public void ProcessAction(BattleAction action)
    {
        var skillData = SkillDataManager.Instance.GetSkillDataById(action.itemId);

        // 消費MP処理
        int hpDelta = 0;
        int mpDelta = skillData.cost * -1;
        if (action.isActorFriend)
            CharacterStatusManager.Instance.ChangeCharacterStatus(action.actorId, hpDelta, mpDelta);
        else
            EnemyStatusManager.Instance.ChangeEnemyStatus(action.actorId, hpDelta, mpDelta);

        _actionProcessor.SetPauseProcess(true);
        StartCoroutine(ProcessSkillActionCoroutine(action, skillData));
    }

    IEnumerator ShowSkillHealMessage(BattleAction action, string skillName, int healValue)
    {
        string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);

        foreach (var targetId in action.targetIds)
        {
            string targetName = _actionProcessor.GetCharacterName(targetId, action.isTargetFriend);
            _actionProcessor.SetPauseMessage(true);
            _messageWindowController.GenerateSkillCastMessage(actorName, skillName);
            while (_actionProcessor.IsPausedMessage) yield return null;

            _actionProcessor.SetPauseMessage(true);
            _messageWindowController.GenerateHpHealMessage(targetName, healValue);
            _battleManager.OnUpdateStatus();
            while (_actionProcessor.IsPausedMessage) yield return null;
        }

        _pauseSkillEffect = false;
    }

    bool IsSkillTargetFriend(SkillEffect skillEffect)
    {
        return skillEffect.EffectTarget == EffectTarget.Own
            || skillEffect.EffectTarget == EffectTarget.FriendSolo
            || skillEffect.EffectTarget == EffectTarget.FriendAll;
    }
}
