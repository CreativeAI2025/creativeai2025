// using UnityEngine;

// public class SkillAnimationManager
// {
//     public SkillData GetSkillDataById(int id)
//     {
//         return skillDataList.Find(s => s.skillId == id);
//     }
//     public void ExecuteSkill(Unit attacker, Unit target, int skillId)
//     {
//         var data = GetSkillDataById(skillId);

//         // エフェクト再生
//         if (data.effectPrefab != null)
//         {
//             GameObject effect = Instantiate(data.effectPrefab, target.transform.position, Quaternion.identity);

//             // エフェクトを一定時間後に削除（アニメ再生後削除）
//             Destroy(effect, 1.0f);
//         }
//     }

// }
