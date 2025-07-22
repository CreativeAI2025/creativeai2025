using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// 選択ウィンドウにて魔法に関する処理を制御するクラスです。
/// </summary>
public class SelectionWindowSkillController : MonoBehaviour
{
    /// <summary>
    /// 項目オブジェクトと魔法IDの対応辞書です。
    /// </summary>
    Dictionary<int, int> _skillIdDictionary = new();

    /// <summary>
    /// キャラクターが覚えている魔法の一覧です。
    /// </summary>
    List<SkillData> _characterSkillList = new();

    /// <summary>
    /// インデックスが有効な範囲か確認します。
    /// </summary>
    /// <param name="index">確認するインデックス</param>
    public bool IsValidIndex(int index)
    {
        bool isValid = index >= 0 && index < _characterSkillList.Count;
        return isValid;
    }

    /// <summary>
    /// 選択中の項目が実行できるか確認します。
    /// 魔法の場合は消費MPを確認、アイテムの場合は所持数を確認します。
    /// </summary>
    /// <param name="selectedIndex">選択中のインデックス</param>
    public bool IsValidSelection(int selectedIndex)
    {
        bool isValid = false;
        int indexInPage = selectedIndex % 4;

        // インデックスが辞書に存在しない場合は有効でないと判断します。
        if (!_skillIdDictionary.ContainsKey(indexInPage))
        {
            return isValid;
        }

        var skillId = _skillIdDictionary[indexInPage];
        var skillData = SkillDataManager.GetSkillDataById(skillId);
        isValid = CanSelectSkill(skillData);
        return isValid;
    }

    /// <summary>
    /// 最大ページ数を取得します。
    /// </summary>
    public int GetMaxPageNum()
    {
        int maxPage = Mathf.CeilToInt(_characterSkillList.Count * 1.0f / 4.0f);
        return maxPage;
    }

    /// <summary>
    /// キャラクターが覚えている魔法をリストにセットします。
    /// </summary>
    public void SetCharacterSkill()
    {
        _characterSkillList.Clear();

        // 指定したキャラクターのステータスを取得します。
        var currentSelectingCharacter = CharacterStatusManager.partyCharacter[0];
        var characterStatus = CharacterStatusManager.GetCharacterStatusById(currentSelectingCharacter);
        foreach (var skillId in characterStatus.skillList)
        {
            var skillData = SkillDataManager.GetSkillDataById(skillId);
            _characterSkillList.Add(skillData);
        }
    }

    /// <summary>
    /// ページ内の魔法の項目をセットします。
    /// </summary>
    /// <param name="page">ページ番号</param>
    /// <param name="uiController">UIの制御クラス</param>
    public void SetPageSkill(int page, SelectionUIController uiController)
    {
        _skillIdDictionary.Clear();
        int startIndex = page * 4;
        for (int i = startIndex; i < startIndex + 4; i++)
        {
            int positionIndex = i - startIndex;
            if (i < _characterSkillList.Count)
            {
                var skillData = _characterSkillList[i];
                string skillName = skillData.skillName;
                int skillCost = skillData.cost;
                bool canSelect = CanSelectSkill(skillData);
                uiController.SetItemText(positionIndex, skillName, skillCost, canSelect);
                uiController.SetDescriptionText(skillData.skillDesc);
                _skillIdDictionary.Add(positionIndex, skillData.skillId);
            }
            else
            {
                uiController.ClearItemText(positionIndex);
            }
        }

        if (_skillIdDictionary.Count == 0)
        {
            string noSkillText = "* 選択できる魔法がありません！ *";
            uiController.SetDescriptionText(noSkillText);
        }
    }

    /// <summary>
    /// 魔法を使えるか確認します。
    /// </summary>
    /// <param name="skillData">魔法データ</param>
    bool CanSelectSkill(SkillData skillData)
    {
        if (skillData == null)
        {
            return false;
        }

        var currentSelectingCharacter = CharacterStatusManager.partyCharacter[0];
        var characterStatus = CharacterStatusManager.GetCharacterStatusById(currentSelectingCharacter);
        return characterStatus.currentMp >= skillData.cost;
    }

    /// <summary>
    /// 項目が選択された時の処理です。
    /// </summary>
    /// <param name="selectedIndex">選択されたインデックス</param>
    public SkillData GetSkillData(int selectedIndex)
    {
        SkillData skillData = null;
        if (selectedIndex >= 0 && selectedIndex < _characterSkillList.Count)
        {
            skillData = _characterSkillList[selectedIndex];
        }

        return skillData;
    }
}