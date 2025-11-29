using UnityEngine;
using TMPro;

// メニューで使われるキャラクター切り替えタブを参照するクラス
public class MenuHeaderMiniField : MonoBehaviour
{
    [SerializeField] private GameObject headerMiniField;
    [SerializeField] private TextMeshProUGUI textField;

    public void SetHeaderMiniField(string text)
    {
        if (text == string.Empty)
        {
            headerMiniField.SetActive(false);
        }
        else
        {
            headerMiniField.SetActive(true);
        }
        textField.text = text;
        return;
    }
}
