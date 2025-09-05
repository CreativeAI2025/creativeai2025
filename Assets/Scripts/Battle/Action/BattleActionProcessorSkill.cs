using UnityEngine;
using System.Collections;
public class BattleActionProcessorSkill : MonoBehaviour
{
    /// <summary>
    /// 戦闘中のアクションを処理するクラスへの参照です。
    /// </summary>
    BattleActionProcessor _actionProcessor;

    /// <summary>
    /// 戦闘に関する機能を管理するクラスへの参照です。
    /// </summary>
    BattleManager _battleManager;

    /// <summary>
    /// メッセージウィンドウを制御するクラスへの参照です。
    /// </summary>
    MessageWindowController _messageWindowController;

    /// <summary>
    /// 戦闘中の敵キャラクターの管理を行うクラスへの参照です。
    /// </summary>
    EnemyStatusManager _enemyStatusManager;
    /// <summary>
    /// 戦闘関連のスプライトを制御するクラスへの参照です。
    /// </summary>
    BattleSpriteController _battleSpriteController;

    /// <summary>
    /// 魔法効果をポーズするかどうかのフラグです。
    /// </summary>
    bool _pauseSkillEffect;

    /// <summary>
    /// 参照をセットします。
    /// </summary>
    public void SetReferences(BattleManager battleManager, BattleActionProcessor actionProcessor)
    {
        _battleManager = battleManager;
        _actionProcessor = actionProcessor;
        _messageWindowController = _battleManager.GetWindowManager().GetMessageWindowController();
        _enemyStatusManager = _battleManager.GetEnemyStatusManager();
        _battleSpriteController = _battleManager.GetBattleSpriteController();
    }

    /// <summary>
    /// 魔法のアクションを処理します。
    /// </summary>
    public void ProcessAction(BattleAction action)
    {
        var skillData = SkillDataManager.Instance.GetSkillDataById(action.itemId);

        // 消費MPの分だけMPを減らします。
        int hpDelta = 0;
        int mpDelta = skillData.cost * -1;
        bool isTargetDefeated = false;
        if (action.isActorFriend)
        {
            CharacterStatusManager.Instance.ChangeCharacterStatus(action.actorId, hpDelta, mpDelta);
        }
        else
        {
            _enemyStatusManager.ChangeEnemyStatus(action.actorId, hpDelta, mpDelta);

        }

        _actionProcessor.SetPauseProcess(true);
        StartCoroutine(ProcessSkillActionCoroutine(action, skillData));
    }

    /// <summary>
    /// 魔法のアクションを処理するコルーチンです。
    /// </summary>
    IEnumerator ProcessSkillActionCoroutine(BattleAction action, SkillData skillData)
    {
        var actorParam = _actionProcessor.GetCharacterParameter(action.actorId, action.isActorFriend);
        var targetParam = _actionProcessor.GetCharacterParameter(action.targetId, action.isTargetFriend);
        int damage = DamageFormula.CalculateDamage(actorParam.Attack, targetParam.Defence);

        bool isTargetDefeated = false;
        // 魔法の効果を処理します。
        foreach (var skillEffect in skillData.skillEffects)
        {
            // メッセージ表示用の行動を生成します。
            BattleAction messageAction = new()
            {
                actorId = action.actorId,
                targetId = action.targetId,
                isActorFriend = action.isActorFriend,
                isTargetFriend = action.isTargetFriend
            };
            if (skillEffect.skillCategory == SkillCategory.Damage)
            {


                // ダメージ量を計算（攻撃力 vs 防御力）
                int damageValue = DamageFormula.CalculateDamage(actorParam.MagicAttack, targetParam.MagicDefence);
                int hpDelta = -damageValue; // ダメージなのでマイナス
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

                // ステータスを変更（ダメージを与える）
                if (messageAction.isTargetFriend)
                {
                    CharacterStatusManager.Instance.ChangeCharacterStatus(messageAction.targetId, hpDelta, mpDelta);
                }
                else
                {
                    _enemyStatusManager.ChangeEnemyStatus(messageAction.targetId, hpDelta, mpDelta);
                    isTargetDefeated = _enemyStatusManager.IsEnemyDefeated(action.targetId);
                    if (isTargetDefeated)
                    {
                        _enemyStatusManager.OnDefeatEnemy(action.targetId);
                    }
                }

                // メッセージを表示
                _pauseSkillEffect = true;
                StartCoroutine(ShowSkillDamageMessage(messageAction, skillData.skillName, damageValue,isTargetDefeated));
            }
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
                {
                    CharacterStatusManager.Instance.ChangeCharacterStatus(messageAction.targetId, hpDelta, mpDelta);
                }
                else
                {
                    _enemyStatusManager.ChangeEnemyStatus(messageAction.targetId, hpDelta, mpDelta);
                }

                _pauseSkillEffect = true;
                StartCoroutine(ShowSkillHealMessage(messageAction, skillData.skillName, hpDelta));
            }
            else
            {
                Debug.LogWarning($"未定義の魔法効果です。 ID: {skillData.skillId}");
            }

            while (_pauseSkillEffect)
            {
                yield return null;
            }
        }

        _actionProcessor.SetPauseProcess(false);
    }
    /// <summary>
    /// 攻撃魔法のメッセージを表示します。
    /// </summary>
    IEnumerator ShowSkillDamageMessage(BattleAction action, string skillName, int damageValue,bool isTargetDefeated)
    {
        string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
        string targetName = _actionProcessor.GetCharacterName(action.targetId, action.isTargetFriend);

        _actionProcessor.SetPauseMessage(true);
        _messageWindowController.GenerateSkillCastMessage(actorName, skillName);

        while (_actionProcessor.IsPausedMessage)
        {
            yield return null;
        }

        _actionProcessor.SetPauseMessage(true);
        _messageWindowController.GenerateSkillCastMessage(actorName, skillName);
        _messageWindowController.GenerateDamageMessage(targetName, damageValue);
        _battleManager.OnUpdateStatus();
        while (_actionProcessor.IsPausedMessage)
        {
            yield return null;
        }
// 撃破チェック
    if (isTargetDefeated)
    {
        if (action.isTargetFriend)
        {
            _actionProcessor.SetPauseMessage(true);
            _messageWindowController.GenerateDefeateFriendMessage(targetName);
            while (_actionProcessor.IsPausedMessage) yield return null;

            if (CharacterStatusManager.Instance.IsAllCharacterDefeated())
            {
                _battleManager.OnGameover();
            }
        }
        else
        {
            _actionProcessor.SetPauseMessage(true);
            _battleSpriteController.HideEnemy();
            _messageWindowController.GenerateDefeateEnemyMessage(targetName);
            while (_actionProcessor.IsPausedMessage) yield return null;

            if (_enemyStatusManager.IsAllEnemyDefeated())
            {
                _battleManager.OnEnemyDefeated();
            }
        }
    }
        _pauseSkillEffect = false;
    }
    /// <summary>
    /// 回復魔法のメッセージを表示します。
    /// </summary>
    IEnumerator ShowSkillHealMessage(BattleAction action, string skillName, int healValue)
    {
        string actorName = _actionProcessor.GetCharacterName(action.actorId, action.isActorFriend);
        string targetName = _actionProcessor.GetCharacterName(action.targetId, action.isTargetFriend);

        _actionProcessor.SetPauseMessage(true);
        _messageWindowController.GenerateSkillCastMessage(actorName, skillName);
        while (_actionProcessor.IsPausedMessage)
        {
            yield return null;
        }

        _actionProcessor.SetPauseMessage(true);
        _messageWindowController.GenerateHpHealMessage(targetName, healValue);
        _battleManager.OnUpdateStatus();
        while (_actionProcessor.IsPausedMessage)
        {
            yield return null;
        }

        _pauseSkillEffect = false;
    }

    /// <summary>
    /// 魔法の対象が行動者の味方かどうかを判定します。
    /// </summary>
    /// <param name="skillEffect">魔法効果</param>
    bool IsSkillTargetFriend(SkillEffect skillEffect)
    {
        bool isFriend = false;
        if (skillEffect.EffectTarget == EffectTarget.Own
            || skillEffect.EffectTarget == EffectTarget.FriendSolo
            || skillEffect.EffectTarget == EffectTarget.FriendAll)
        {
            isFriend = true;
        }
        return isFriend;
    }
}
