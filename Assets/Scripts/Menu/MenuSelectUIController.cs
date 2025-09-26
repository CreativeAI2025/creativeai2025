using UnityEngine;
using TMPro;

public class MenuSelectUIController : MonoBehaviour, IMenuUIController
{
    [SerializeField] TextMeshProUGUI _textField;
    [SerializeField] GameObject _cursorMainCharacter;
    [SerializeField] GameObject _cursorFriend1;
    [SerializeField] GameObject _cursorFriend2;

    private void HideAllCursor()
    {
        _cursorMainCharacter.SetActive(false);
        _cursorFriend1.SetActive(false);
        _cursorFriend2.SetActive(false);
    }

    public void InputText(string text)
    {
        _textField.text = text;
    }

    public void ShowSelectedCursor(int cursor)
    {
        HideAllCursor();

        if (cursor < 0 || cursor >= 3)
        {
            Debug.Log("そんなにパーティメンバーがいるわけないでしょうが");
            return;
        }

        switch (cursor)
        {
            case 0:
                _cursorMainCharacter.SetActive(true);
                break;
            case 1:
                _cursorFriend1.SetActive(true);
                break;
            case 2:
                _cursorFriend2.SetActive(true);
                break;
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
