using UnityEngine;

public class TopMenuUIController : MonoBehaviour, IMenuUIController
{
    /// <summary>
    /// メニューの各カーソルオブジェクト
    /// </summary>
    [SerializeField] GameObject _cursorObjCharacter;
    [SerializeField] GameObject _cursorObjSkill;
    [SerializeField] GameObject _cursorObjItem;
    [SerializeField] GameObject _cursorObjSkillBoard;

    /// <summary>
    /// コマンドのカーソルを全て非表示にする
    /// </summary>
    private void HideAllCursor()
    {
        _cursorObjCharacter.SetActive(false);
        _cursorObjSkill.SetActive(false);
        _cursorObjItem.SetActive(false);
        _cursorObjSkillBoard.SetActive(false);
    }

    /// <summary>
    /// 選択中の項目のカーソルを表示する
    /// </summary>
    /// <param name="command"></param>
    public void ShowSelectedCursor(MenuCommand command)
    {
        HideAllCursor();

        switch (command)
        {
            case MenuCommand.Character:
                _cursorObjCharacter.SetActive(true);
                //Debug.Log("キャラクター選択中");
                break;
            case MenuCommand.Skill:
                _cursorObjSkill.SetActive(true);
                //Debug.Log("スキル選択中");
                break;
            case MenuCommand.Item:
                _cursorObjItem.SetActive(true);
                //Debug.Log("アイテム選択中");
                break;
            case MenuCommand.SkillTree:
                _cursorObjSkillBoard.SetActive(true);
                //Debug.Log("スキルボード選択中");
                break;
        }
    }

    /// <summary>
    /// UIを表示する
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// UIを非表示にする
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
