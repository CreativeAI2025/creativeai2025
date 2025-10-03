using UnityEngine;
using System.Collections;

public class BattleActionProcessorSkill : MonoBehaviour
{
    BattleActionProcessor _actionProcessor;
    BattleManager _battleManager;
    MessageWindowController _messageWindowController;
    EnemyStatusManager _enemyStatusManager;
    BattleSpriteController _battleSpriteController;

    // 追加: 状態異常マネージャー
    [SerializeField] private StatusEffectManager statusEffectManager;

    bool _pauseSkillEffect;

    public void SetReferences(BattleManager battleManager, BattleActionProcessor actionProcessor)
    {
        _battleManager = battleManager;
        _actionProcessor = actionProcessor;
        _messageWindowController = _battleManager.GetWindowManager().GetMessageWindowController();
        _enemyStatusManager = _battleManager.GetEnemyStatusManager();
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
            _enemyStatusManager.ChangeEnemyStatus(action.actorId, hpDelta, mpDelta);

        _actionProcessor.SetPauseProcess(true);
        StartCoroutine(ProcessSkillActionCoroutine(action, skillData));
    }

    IEnumerator ProcessSkillActionCoroutine(BattleAction action, SkillData skillData)
    {
        var actorParam = _actionProcessor.GetCharacterParameter(action.actorId, action.isActorFriend);
        var targetParam = _actionProcessor.GetCharacterParameter(action.targetId, action.isTargetFriend);

        bool isTargetDefeated = false;

        foreach (var skillEffect in skillData.skillEffects)
        {
            BattleAction messageAction = new()
            {
                actorId = action.actorId,
                targetId = action.targetId,
                isActorFriend = action.isActorFriend,
                isTargetFriend = action.isTargetFriend
            };

            // ダメージ系
            if (skillEffect.skillCategory == SkillCategory.Damage)
            {
                int damageValue = DamageFormula.CalculateSkillDamage(actorParam.MagicAttack, targetParam.MagicDefence, skillEffect.value);
                int hpDelta = -damageValue;
                int mpDelta = 0;

                bool isSkillTargetFriend = IsSkillTargetFriend(skillEffect);
                // var charaStatus = CharacterStatusManager.Instance.GetCharacterStatusById(action.actorId);
                // if (charaStatus != null && charaStatus.Confusion && Random.value < 0.5f)
                // {
                //     action.targetId = action.actorId;
                // }
                // var enemyStatus = _enemyStatusManager.GetEnemyStatusByBattleId(action.actorId);
                // if (enemyStatus != null && enemyStatus.Confusion && Random.value < 0.5f)
                // {
                //     action.targetId = action.actorId;

                // }
                if (isSkillTargetFriend)
                {
                    messageAction.targetId = action.actorId;
                    messageAction.isTargetFriend = action.isActorFriend;
                }
                else
                {
                    messageAction.targetId = action.targetId;
                    messageAction.isTargetFriend = !action.isActorFriend;
                }

                if (messageAction.isTargetFriend)
                {
                    CharacterStatusManager.Instance.ChangeCharacterStatus(messageAction.targetId, hpDelta, mpDelta);
                }
                else
                {
                    _enemyStatusManager.ChangeEnemyStatus(messageAction.targetId, hpDelta, mpDelta);
                    isTargetDefeated = _enemyStatusManager.IsEnemyDefeated(action.targetId);

                    if (isTargetDefeated)
                        _enemyStatusManager.OnDefeatEnemy(action.targetId);
                }

                // 状態異常付与
                StatusEffectCategory? appliedEffectCategory = null;
                if (skillEffect.StatusEffectEnable && skillEffect.StatusEffect != null)
                {
                    foreach (var statusEffect in skillEffect.StatusEffect)
                    {
                        appliedEffectCategory = statusEffect.EffectCategory;
                        Logger.Instance.Log("状態異常付与");
                        if (isSkillTargetFriend)
                        {
                            statusEffectManager.ApplyStatusEffectToPlayer(action.targetId, statusEffect);
                        }
                        else
                        {
                            statusEffectManager.ApplyStatusEffectToEnemy(action.targetId, statusEffect);
                        }
                    }
                }

                _pauseSkillEffect = true;
                StartCoroutine(ShowSkillDamageMessage(messageAction, skillData.skillName, damageValue, isTargetDefeated, appliedEffectCategory));
            }
            // 回復系
            else if (skillEffect.skillCategory == SkillCategory.Recovery)
            {
                int hpDelta = DamageFormula.CalculateHealValue(skillEffect.value);
                int mpDelta = 0;

                bool isSkillTargetFriend = IsSkillTargetFriend(skillEffect);

                if (isSkillTargetFriend)
                {
                    messageAction.targetId = action.actorId;
                    messageAction.isTargetFriend = action.isActorFriend;
                }
                else
                {
                    messageAction.targetId = action.targetId;
                    messageAction.isTargetFriend = !action.isActorFriend;
                }

                if (messageAction.isTargetFriend)
                    CharacterStatusManager.Instance.ChangeCharacterStatus(messageAction.targetId, hpDelta, mpDelta);
                else
                    _enemyStatusManager.ChangeEnemyStatus(messageAction.targetId, hpDelta, mpDelta);

                _pauseSkillEffect = true;
                StartCoroutine(ShowSkillHealMessage(messageAction, skillData.skillName, hpDelta));
            }
            else
            {
                Debug.LogWarning($"未定義の魔法効果です。 ID: {skillData.skillId}");
            }

            while (_pauseSkillEffect) yield return null;
        }

        _actionProcessor.SetPauseProcess(false);
    }

    IEnumerator ShowSkillDamageMessage(BattleAction action, string skillName, int damageValue, bool isTargetDefeated, StatusEffectCategory? effectCategory)
    {
        string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
        string targetName = _actionProcessor.GetCharacterName(action.targetId, action.isTargetFriend);

        string statusMessage = "";
        if (effectCategory != null)
        {
            switch (effectCategory)
            {
                case StatusEffectCategory.Poison:
                    statusMessage = BattleMessage.PoisonSuffix;
                    break;
                case StatusEffectCategory.Paralysis:
                    statusMessage = BattleMessage.ParalysisSuffix;
                    break;
                case StatusEffectCategory.Sleep:
                    statusMessage = BattleMessage.SleepSuffix;
                    break;
                case StatusEffectCategory.Confusion:
                    statusMessage = BattleMessage.ConfusionSuffix;
                    break;
            }
        }

        _actionProcessor.SetPauseMessage(true);
        _messageWindowController.GenerateSkillCastMessage(actorName, skillName);
        while (_actionProcessor.IsPausedMessage) yield return null;

        _actionProcessor.SetPauseMessage(true);
        _messageWindowController.GenerateSkillCastMessage(actorName, skillName);
        _messageWindowController.GenerateDamageMessage(targetName, damageValue);

        if (!string.IsNullOrEmpty(statusMessage))
            _messageWindowController.GenerateStatusAilmentMessage(targetName, statusMessage);

        _battleManager.OnUpdateStatus();
        while (_actionProcessor.IsPausedMessage) yield return null;

        // 撃破チェック
        if (isTargetDefeated)
        {
            if (action.isTargetFriend)
            {
                _actionProcessor.SetPauseMessage(true);
                _messageWindowController.GenerateDefeateFriendMessage(targetName);
                while (_actionProcessor.IsPausedMessage) yield return null;

                if (CharacterStatusManager.Instance.IsAllCharacterDefeated())
                    _battleManager.OnGameover();
            }
            else
            {
                _actionProcessor.SetPauseMessage(true);
                _battleSpriteController.HideEnemy();
                _messageWindowController.GenerateDefeateEnemyMessage(targetName);
                while (_actionProcessor.IsPausedMessage) yield return null;

                if (_enemyStatusManager.IsAllEnemyDefeated())
                    _battleManager.OnEnemyDefeated();
            }
        }

        _pauseSkillEffect = false;
    }

    IEnumerator ShowSkillHealMessage(BattleAction action, string skillName, int healValue)
    {
        string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
        string targetName = _actionProcessor.GetCharacterName(action.targetId, action.isTargetFriend);

        _actionProcessor.SetPauseMessage(true);
        _messageWindowController.GenerateSkillCastMessage(actorName, skillName);
        while (_actionProcessor.IsPausedMessage) yield return null;

        _actionProcessor.SetPauseMessage(true);
        _messageWindowController.GenerateHpHealMessage(targetName, healValue);
        _battleManager.OnUpdateStatus();
        while (_actionProcessor.IsPausedMessage) yield return null;

        _pauseSkillEffect = false;
    }

    bool IsSkillTargetFriend(SkillEffect skillEffect)
    {
        return skillEffect.EffectTarget == EffectTarget.Own
            || skillEffect.EffectTarget == EffectTarget.FriendSolo
            || skillEffect.EffectTarget == EffectTarget.FriendAll;
    }
}
