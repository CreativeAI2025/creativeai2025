using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状態異常を管理するクラス
/// </summary>
public class StatusEffectManager : MonoBehaviour
{
    private BattleManager _battleManager;

    // 味方キャラごとの状態異常
    private Dictionary<int, List<StatusEffect>> _playerStatusEffects = new Dictionary<int, List<StatusEffect>>();
    // 敵キャラごとの状態異常
    private Dictionary<int, List<StatusEffect>> _enemyStatusEffects = new Dictionary<int, List<StatusEffect>>();

    public void SetBattleManager(BattleManager battleManager)
    {
        _battleManager = battleManager;
    }

    /// <summary>
    /// 味方に状態異常を付与
    /// </summary>
    public void ApplyStatusEffectToPlayer(int actorId, StatusEffect effect)
    {
        var characterStatus = CharacterStatusManager.Instance.GetCharacterStatusById(actorId);
        if (characterStatus == null)
        {
            Debug.LogWarning($"キャラクターのステータスが見つかりませんでした。 ID : {actorId}");
            return;
        }
        characterStatus.Duration += effect.Duration;
        if (effect.EffectCategory == StatusEffectCategory.Poison)
        {
            characterStatus.Poison = true;
        }
        if (effect.EffectCategory == StatusEffectCategory.Sleep)
        {
            characterStatus.Sleep = true;
            characterStatus.IsCharaStop = CanPlayerAct(actorId);
        }
        if (effect.EffectCategory == StatusEffectCategory.Paralysis)
        {
            characterStatus.Paralysis = true;
            characterStatus.IsCharaStop = CanPlayerAct(actorId);
        }
        if (effect.EffectCategory == StatusEffectCategory.Confusion)
        {
            characterStatus.Confusion = true;
        }
        if (!_playerStatusEffects.ContainsKey(actorId))
        {
            _playerStatusEffects[actorId] = new List<StatusEffect>();
        }
        _playerStatusEffects[actorId].Add(effect);

    }

    /// <summary>
    /// 敵に状態異常を付与
    /// </summary>
    public void ApplyStatusEffectToEnemy(int enemyId, StatusEffect effect)
    {
        var enemyStatus = EnemyStatusManager.Instance.GetEnemyStatusByBattleId(enemyId);
        if (enemyStatus == null)
        {
            Logger.Instance.LogWarning($"敵キャラクターのステータスが見つかりませんでした。 戦闘中ID : {enemyId}");
            return;
        }

        enemyStatus.Duration += effect.Duration;
        if (effect.EffectCategory == StatusEffectCategory.Poison)
        {
            enemyStatus.Poison = true;
        }

        if (effect.EffectCategory == StatusEffectCategory.Sleep)
        {
            enemyStatus.Sleep = true;
            enemyStatus.IsEnemyStop = CanPlayerAct(enemyId);
        }

        if (effect.EffectCategory == StatusEffectCategory.Paralysis)
        {
            enemyStatus.Paralysis = true;
            enemyStatus.IsEnemyStop = CanPlayerAct(enemyId);
        }

        if (effect.EffectCategory == StatusEffectCategory.Confusion)
        {
            enemyStatus.Confusion = true;
        }

        if (!_enemyStatusEffects.ContainsKey(enemyId))
        {
            _enemyStatusEffects[enemyId] = new List<StatusEffect>();
        }

        _enemyStatusEffects[enemyId].Add(effect);
    }

    /// <summary>
    /// ターン終了時の状態異常処理（毒ダメージ・行動不能など）
    /// </summary>
    public void ProcessTurnEffects()
    {
        Logger.Instance.Log("=== 状態異常のターン終了処理を開始します ===");

        // 味方の状態異常処理
        foreach (var kvp in _playerStatusEffects)
        {
            int actorId = kvp.Key;
            var effects = kvp.Value;
            ProcessEffectsForActor(actorId, effects, isPlayer: true);
            UpdateBuffs(actorId, isPlayer: true);
        }

        // 敵の状態異常処理
        foreach (var kvp in _enemyStatusEffects)
        {
            int enemyId = kvp.Key;
            var effects = kvp.Value;
            ProcessEffectsForActor(enemyId, effects, isPlayer: false);
            UpdateBuffs(enemyId, isPlayer: false);
        }
    }
    /// <summary>
    /// ターン処理
    /// </summary>
    public void ReduceDuration(int characterId, StatusEffect effect)
    {
        var characterStatus = CharacterStatusManager.Instance.GetCharacterStatusById(characterId);
        if (characterStatus == null)
        {
            Debug.LogWarning($"キャラクターのステータスが見つかりませんでした。 ID : {characterId}");
            return;
        }
        characterStatus.Duration += effect.Duration;

        if (characterStatus.Duration == 0)
        {
            characterStatus.isDefeated = true;
            return;
        }
    }
    /// <summary>
    /// 個別のキャラクターに対する状態異常処理
    /// </summary>
    private void ProcessEffectsForActor(int id, List<StatusEffect> effects, bool isPlayer)
    {
        for (int i = effects.Count - 1; i >= 0; i--)
        {
            StatusEffect effect = effects[i];
            var characterStatus = CharacterStatusManager.Instance.GetCharacterStatusById(id);
            var enemyStatus = EnemyStatusManager.Instance.GetEnemyStatusByBattleId(id);
            switch (effect.EffectCategory)
            {
                case StatusEffectCategory.Poison:
                    int poisonDamage = effect.Power;
                    if (isPlayer)
                    {
                        CharacterStatusManager.Instance.ChangeCharacterStatus(id, -poisonDamage, 0);
                        Logger.Instance.Log($"味方 {id} は毒で {poisonDamage} ダメージを受けた！");
                    }
                    else
                    {
                        _battleManager.GetEnemyStatusManager().ChangeEnemyStatus(id, -poisonDamage, 0);
                        Logger.Instance.Log($"敵 {id} は毒で {poisonDamage} ダメージを受けた！");
                    }
                    break;

                case StatusEffectCategory.Paralysis:
                    if (isPlayer)
                    {
                        characterStatus.IsCharaStop = CanPlayerAct(id);
                        Logger.Instance.Log($"{"味方"} {id} は麻痺している…（次の行動制御で参照予定）");
                    }
                    else
                    {
                        enemyStatus.IsEnemyStop = CanEnemyAct(id);
                        Logger.Instance.Log($"{"味方"} {id} は麻痺している…（次の行動制御で参照予定）");
                    }
                    break;

                case StatusEffectCategory.Sleep:
                    Logger.Instance.Log($"{(isPlayer ? "味方" : "敵")} {id} は眠っている…");
                    break;

                case StatusEffectCategory.Confusion:
                    Logger.Instance.Log($"{(isPlayer ? "味方" : "敵")} {id} は混乱している！");
                    break;
            }

            // ターン数を減らす

            if (isPlayer)
            {

                Debug.LogWarning("ターン数。 : {characterStatus.Duration}");
                --characterStatus.Duration;
                if (characterStatus.Duration <= 0)
                {
                    characterStatus.IsCharaStop = true;
                    Logger.Instance.Log($"{"味方"} {id} の {effect.EffectCategory} が解除された");
                    effects.RemoveAt(i);
                }
                return;
            }
            else
            {

                Logger.Instance.LogWarning($"ターン数。 : {enemyStatus.Duration}");
                --enemyStatus.Duration;
                if (enemyStatus.Duration <= 0)
                {
                    enemyStatus.IsEnemyStop = false;
                    Logger.Instance.Log($"{"敵"} {id} の {effect.EffectCategory} が解除された");
                    effects.RemoveAt(i);
                }
                return;
            }

        }
    }
    /// <summary>
    /// 味方が行動可能かどうかを判定する
    /// </summary>
    public bool CanPlayerAct(int actorId)
    {
        if (!_playerStatusEffects.ContainsKey(actorId)) return true;
        var characterStatus = CharacterStatusManager.Instance.GetCharacterStatusById(actorId);
        var effects = _playerStatusEffects[actorId];
        foreach (var effect in effects)
        {
            if (effect.EffectCategory == StatusEffectCategory.Sleep)
            {
                Logger.Instance.Log($"味方 {actorId} は眠っていて行動できない！");
                return true;
            }
            if (effect.EffectCategory == StatusEffectCategory.Paralysis)
            {
                if (Random.value < effect.Power) // 50%の確率
                {
                    Logger.Instance.Log($"味方 {actorId} は麻痺で体が動かない！");
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 敵が行動可能かどうかを判定する
    /// </summary>
    public bool CanEnemyAct(int enemyId)
    {
        if (!_enemyStatusEffects.ContainsKey(enemyId)) return true;
        var enemyStatus = EnemyStatusManager.Instance.GetEnemyStatusByBattleId(enemyId);
        var effects = _enemyStatusEffects[enemyId];
        foreach (var effect in effects)
        {
            if (effect.EffectCategory == StatusEffectCategory.Sleep)
            {

                Logger.Instance.Log($"敵 {enemyId} は眠っていて行動できない！");
                return true;
            }
            if (effect.EffectCategory == StatusEffectCategory.Paralysis)
            {
                if (Random.value < 0.5f)
                {

                    Logger.Instance.Log($"敵 {enemyId} は麻痺で動けない！");
                    return true;
                }
            }
        }
        return false;
    }
    /// <summary>
    /// 味方にバフを付与
    /// </summary>
    public void PlayerApplyBuff(int actorId, Buff buff)
    {
        var characterStatus = CharacterStatusManager.Instance.GetCharacterStatusById(actorId);
        if (characterStatus == null)
        {
            Debug.LogWarning($"キャラクターのステータスが見つかりませんでした。 ID : {actorId}");
            return;
        }
        switch (buff.BuffCategory)
        {
            case BuffStatusCategory.Attack:
                characterStatus.attackBuffMultiplier = buff.Power;
                characterStatus.attackBuffDuration = buff.Duration;
                break;

            case BuffStatusCategory.Defence:
                characterStatus.defenceBuffMultiplier = buff.Power;
                characterStatus.defenceBuffDuration = buff.Duration;
                break;

            case BuffStatusCategory.MagicAttack:
                characterStatus.magicAttackBuffMultiplier = buff.Power;
                characterStatus.magicAttackBuffDuration = buff.Duration;
                break;

            case BuffStatusCategory.MagicDefence:
                characterStatus.magicDefenceBuffMultiplier = buff.Power;
                characterStatus.magicDefenceBuffDuration = buff.Duration;
                break;

            case BuffStatusCategory.Speed:
                characterStatus.speedBuffMultiplier = buff.Power;
                characterStatus.speedBuffDuration = buff.Duration;
                break;

            case BuffStatusCategory.Evasion:
                characterStatus.evasionBuffMultiplier = buff.Power;
                characterStatus.evasionBuffDuration = buff.Duration;
                break;
        }

        Debug.Log($"{buff.BuffCategory} に {buff.Power} 倍のバフを付与！（{buff.Duration}ターン）");
    }
    /// <summary>
    /// バフのターン経過
    /// </summary>
    public void UpdateBuffs(int actorId, bool isPlayer)
    {
        if (isPlayer)
        {
            var characterStatus = CharacterStatusManager.Instance.GetCharacterStatusById(actorId);
            if (characterStatus == null)
            {
                Debug.LogWarning($"キャラクターのステータスが見つかりませんでした。 ID : {actorId}");
                return;
            }
            if (characterStatus.attackBuffDuration > 0 && --characterStatus.attackBuffDuration == 0)
                characterStatus.attackBuffMultiplier = 1.0f;

            if (characterStatus.defenceBuffDuration > 0 && --characterStatus.defenceBuffDuration == 0)
                characterStatus.defenceBuffMultiplier = 1.0f;

            if (characterStatus.magicAttackBuffDuration > 0 && --characterStatus.magicAttackBuffDuration == 0)
                characterStatus.magicAttackBuffMultiplier = 1.0f;

            if (characterStatus.magicDefenceBuffDuration > 0 && --characterStatus.magicDefenceBuffDuration == 0)
                characterStatus.magicDefenceBuffMultiplier = 1.0f;

            if (characterStatus.speedBuffDuration > 0 && --characterStatus.speedBuffDuration == 0)
                characterStatus.speedBuffMultiplier = 1.0f;

            if (characterStatus.evasionBuffDuration > 0 && --characterStatus.evasionBuffDuration == 0)
                characterStatus.evasionBuffMultiplier = 1.0f;
        }
        else
        {

            var characterStatus = EnemyStatusManager.Instance.GetEnemyStatusByBattleId(actorId);
            if (characterStatus == null)
            {
                Logger.Instance.LogWarning($"敵キャラクターのステータスが見つかりませんでした。 戦闘中ID : {actorId}");
                return;
            }
            if (characterStatus.attackBuffDuration > 0 && --characterStatus.attackBuffDuration == 0)
                characterStatus.attackBuffMultiplier = 1.0f;

            if (characterStatus.defenceBuffDuration > 0 && --characterStatus.defenceBuffDuration == 0)
                characterStatus.defenceBuffMultiplier = 1.0f;

            if (characterStatus.magicAttackBuffDuration > 0 && --characterStatus.magicAttackBuffDuration == 0)
                characterStatus.magicAttackBuffMultiplier = 1.0f;

            if (characterStatus.magicDefenceBuffDuration > 0 && --characterStatus.magicDefenceBuffDuration == 0)
                characterStatus.magicDefenceBuffMultiplier = 1.0f;

            if (characterStatus.speedBuffDuration > 0 && --characterStatus.speedBuffDuration == 0)
                characterStatus.speedBuffMultiplier = 1.0f;

            if (characterStatus.evasionBuffDuration > 0 && --characterStatus.evasionBuffDuration == 0)
                characterStatus.evasionBuffMultiplier = 1.0f;
        }

    }
    /// <summary>
    /// 敵にバフを付与
    /// </summary>
    public void EnemyApplyBuff(int enemyId, Buff buff)
    {
        var enemyStatus = EnemyStatusManager.Instance.GetEnemyStatusByBattleId(enemyId);
        if (enemyStatus == null)
        {
            Logger.Instance.LogWarning($"敵キャラクターのステータスが見つかりませんでした。 戦闘中ID : {enemyId}");
            return;
        }
        switch (buff.BuffCategory)
        {
            case BuffStatusCategory.Attack:
                enemyStatus.attackBuffMultiplier = buff.Power;
                enemyStatus.attackBuffDuration = buff.Duration;
                break;

            case BuffStatusCategory.Defence:
                enemyStatus.defenceBuffMultiplier = buff.Power;
                enemyStatus.defenceBuffDuration = buff.Duration;
                break;

            case BuffStatusCategory.MagicAttack:
                enemyStatus.magicAttackBuffMultiplier = buff.Power;
                enemyStatus.magicAttackBuffDuration = buff.Duration;
                break;

            case BuffStatusCategory.MagicDefence:
                enemyStatus.magicDefenceBuffMultiplier = buff.Power;
                enemyStatus.magicDefenceBuffDuration = buff.Duration;
                break;

            case BuffStatusCategory.Speed:
                enemyStatus.speedBuffMultiplier = buff.Power;
                enemyStatus.speedBuffDuration = buff.Duration;
                break;

            case BuffStatusCategory.Evasion:
                enemyStatus.evasionBuffMultiplier = buff.Power;
                enemyStatus.evasionBuffDuration = buff.Duration;
                break;
        }

        Debug.Log($"{buff.BuffCategory} に {buff.Power} 倍のバフを付与！（{buff.Duration}ターン）");
    }
    /// <summary>
    /// クリア処理（戦闘終了時など）
    /// </summary>
    public void ClearAllEffects()
    {
        _playerStatusEffects.Clear();
        _enemyStatusEffects.Clear();
    }
}
