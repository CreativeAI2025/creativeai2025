using UnityEngine;
using TMPro;

public class MenuSelectUIController : MonoBehaviour, IMenuUIController
{
    [SerializeField] private TextMeshProUGUI _textField;
    [SerializeField] private MenuCharacterStatusUI[] _characterStatusUIs;

    private void SetupSelectWindow()
    {
        foreach (var ui in _characterStatusUIs)
        {
            ui.Initialize();
        }
    }

    public void HideAllCursor()
    {
        foreach (var ui in _characterStatusUIs)
        {
            ui.Hide();
        }
    }

    private void GetDarkAll()
    {
        foreach (var ui in _characterStatusUIs)
        {
            ui.MakeCharacterImageDark();
        }
    }

    public void GetBrightAll()
    {
        foreach (var ui in _characterStatusUIs)
        {
            ui.MakeCharacterImageBright();
        }
    }

    public void InputText(string text)
    {
        _textField.text = text;
    }

    public void ShowSelectedCursor(int cursor)
    {
        GetDarkAll();
        _characterStatusUIs[cursor].MakeCharacterImageBright();
    }

    /// <summary>
    /// 一つ目のキャラクターステータスをセットする。
    /// Show()関数の前に呼ぶ
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="currentHP"></param>
    /// <param name="maxHP"></param>
    /// <param name="currentMP"></param>
    /// <param name="maxMP"></param>
    public void SetCharacterStatus(int cursor, Sprite sprite, int currentHP, int maxHP, int currentMP, int maxMP)
    {
        _characterStatusUIs[cursor].SetCharacterSprite(sprite);

        float hpRate = (float)currentHP / (float)maxHP;
        _characterStatusUIs[cursor].SetHPTextFromValue(currentHP, maxHP);
        _characterStatusUIs[cursor].SetHpbarSize(hpRate);

        float mpRate = (float)currentMP / (float)maxMP;
        _characterStatusUIs[cursor].SetMPTextFromValue(currentMP, maxMP);
        _characterStatusUIs[cursor].SetMpbarSize(mpRate);

        _characterStatusUIs[cursor].Show();
    }

    public void Show()
    {
        SetupSelectWindow();
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
