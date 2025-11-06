using UnityEngine;


/// <summary>
/// コマンドのUIを制御するクラスです。
/// </summary>
public class CommandUIController : MonoBehaviour, IBattleUIController
{
    /// <summary>
    /// 攻撃コマンドのカーソルオブジェクトです。
    /// </summary>
    [SerializeField]
    GameObject _cursorObjAttack;

    /// <summary>
    /// 魔法コマンドのカーソルオブジェクトです。
    /// </summary>
    [SerializeField]
    GameObject _cursorObjSkill;

    /// <summary>
    /// アイテムコマンドのカーソルオブジェクトです。
    /// </summary>
    [SerializeField]
    GameObject _cursorObjItem;

    /// <summary>
    /// 逃げるコマンドのカーソルオブジェクトです。
    /// </summary>
    [SerializeField]
    GameObject _cursorObjRun;

    /// <summary>
    /// コマンドのカーソルをすべて非表示にします。
    /// </summary>
    void HideAllCursor()
    {
        _cursorObjAttack.SetActive(false);
        _cursorObjSkill.SetActive(false);
        _cursorObjItem.SetActive(false);
        _cursorObjRun.SetActive(false);
    }

    /// <summary>
    /// 選択中のコマンドのカーソルを表示します。
    /// </summary>
    public void ShowSelectedCursor(BattleCommand command)
    {
        HideAllCursor();

        switch (command)
        {
            case BattleCommand.Attack:
                _cursorObjAttack.SetActive(true);
                break;
            case BattleCommand.Skill:
                _cursorObjSkill.SetActive(true);
                break;
            case BattleCommand.Item:
                _cursorObjItem.SetActive(true);
                break;
            case BattleCommand.Run:
                _cursorObjRun.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// UIを表示します。
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// UIを非表示にします。
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}