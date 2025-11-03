using UnityEngine;
using UnityEngine.UI;

public class TopMenuUIController : MonoBehaviour, IMenuUIController
{
    /*
    以下、「カーソル」と表記されていますが、imageカラーを変えるように変更されているので、ご注意を
    */
    /// <summary>
    /// メニューの各カーソルオブジェクト
    /// </summary>
    [SerializeField] Image _cursorObjCharacter;
    [SerializeField] Image _cursorObjSkill;
    [SerializeField] Image _cursorObjItem;
    [SerializeField] Image _cursorObjSkillBoard;

    private Color white = Color.white;
    private Color selectedColor = Color.red;

    /// <summary>
    /// コマンドのカーソルを全て非表示にする
    /// </summary>
    private void HideAllCursor()
    {
        _cursorObjCharacter.color = white;
        _cursorObjSkill.color = white;
        _cursorObjItem.color = white;
        _cursorObjSkillBoard.color = white;
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
                _cursorObjCharacter.color = selectedColor;
                //Debug.Log("キャラクター選択中");
                break;
            case MenuCommand.Skill:
                _cursorObjSkill.color = selectedColor;
                //Debug.Log("スキル選択中");
                break;
            case MenuCommand.Item:
                _cursorObjItem.color = selectedColor;
                //Debug.Log("アイテム選択中");
                break;
            case MenuCommand.SkillTree:
                _cursorObjSkillBoard.color = selectedColor;
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
