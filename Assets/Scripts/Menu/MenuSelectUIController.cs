using UnityEngine;
using TMPro;

public class MenuSelectUIController : MonoBehaviour, IMenuUIController
{
    [SerializeField] TextMeshProUGUI _textField;
    [SerializeField] MenuCharacterStatusUI _characterStatusField1;
    [SerializeField] MenuCharacterStatusUI _characterStatusField2;
    [SerializeField] MenuCharacterStatusUI _characterStatusField3;

    private void SetupSelectWindow()
    {
        _characterStatusField1.Initialize();
        _characterStatusField2.Initialize();
        _characterStatusField3.Initialize();
    }

    public void HideAllCursor()
    {
        _characterStatusField1.Hide();
        _characterStatusField2.Hide();
        _characterStatusField3.Hide();
    }

    private void GetDarkAll()
    {
        _characterStatusField1.MakeCharacterImageDark();
        _characterStatusField2.MakeCharacterImageDark();
        _characterStatusField3.MakeCharacterImageDark();
    }

    public void GetBrightAll()
    {
        _characterStatusField1.MakeCharacterImageBright();
        _characterStatusField2.MakeCharacterImageBright();
        _characterStatusField3.MakeCharacterImageBright();
    }

    public void InputText(string text)
    {
        _textField.text = text;
    }

    public void ShowSelectedCursor(int cursor)
    {
        GetDarkAll();

        if (cursor < 0 || cursor > 2)
        {
            Debug.Log("そんなにパーティメンバーがいるわけないでしょうが");
            return;
        }

        switch (cursor)
        {
            case 0:
                _characterStatusField1.MakeCharacterImageBright();
                break;
            case 1:
                _characterStatusField2.MakeCharacterImageBright();
                break;
            case 2:
                _characterStatusField3.MakeCharacterImageBright();
                break;
        }
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
    public void SetCharacterStatus1(Sprite sprite, int currentHP, int maxHP, int currentMP, int maxMP)
    {
        _characterStatusField1.SetCharacterSprite(sprite);

        float hpRate = (float)currentHP / (float)maxHP;
        _characterStatusField1.SetHPTextFromValue(currentHP, maxHP);
        _characterStatusField1.SetHpbarSize(hpRate);

        float mpRate = (float)currentMP / (float)maxMP;
        _characterStatusField1.SetMPTextFromValue(currentMP, maxMP);
        _characterStatusField1.SetMpbarSize(mpRate);

        _characterStatusField1.Show();
    }
    public void SetCharacterStatus2(Sprite sprite, int currentHP, int maxHP, int currentMP, int maxMP)
    {
        _characterStatusField2.SetCharacterSprite(sprite);

        float hpRate = (float)currentHP / (float)maxHP;
        _characterStatusField2.SetHPTextFromValue(currentHP, maxHP);
        _characterStatusField2.SetHpbarSize(hpRate);

        float mpRate = (float)currentMP / (float)maxMP;
        _characterStatusField2.SetMPTextFromValue(currentMP, maxMP);
        _characterStatusField2.SetMpbarSize(mpRate);

        _characterStatusField2.Show();
    }
    public void SetCharacterStatus3(Sprite sprite, int currentHP, int maxHP, int currentMP, int maxMP)
    {
        _characterStatusField3.SetCharacterSprite(sprite);

        float hpRate = (float)currentHP / (float)maxHP;
        _characterStatusField3.SetHPTextFromValue(currentHP, maxHP);
        _characterStatusField3.SetHpbarSize(hpRate);

        float mpRate = (float)currentMP / (float)maxMP;
        _characterStatusField3.SetMPTextFromValue(currentMP, maxMP);
        _characterStatusField3.SetMpbarSize(mpRate);

        _characterStatusField3.Show();
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
