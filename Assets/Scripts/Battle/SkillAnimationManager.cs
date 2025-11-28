using UnityEngine;
using System.Collections.Generic;

public class SkillAnimationManager : MonoBehaviour
{
    [SerializeField] private List<SkillEfectChange> enemyEffects = new List<SkillEfectChange>();
    private BattleManager _battleManager;
    public void SetReferences(BattleManager battleManager)
    {
        _battleManager = battleManager;
    }
    public void PlayEffectAtEnemy(int targetIndex, int skillId, bool isFriend)
    {
        if (targetIndex < 0 || targetIndex >= enemyEffects.Count)
        {
            Debug.LogWarning($"targetIndexが不正です : {targetIndex}");
            return;
        }
        if (isFriend)
        {
            return;
        }
        //Debug.LogWarning($"targetIndex: {targetIndex}skillId: {skillId}");
        enemyEffects[targetIndex].PlaySkillAnimation(skillId);

    }
}
