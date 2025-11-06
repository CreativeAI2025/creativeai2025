using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuBarUIController : MonoBehaviour
{
    // HPやMPの値を表示するテキストフィールド
    [SerializeField] private TextMeshProUGUI textField;
    // バーの全体を表示するゲームオブジェクト
    [SerializeField] private GameObject barParent;
    // バーの値を割合で表示するゲームオブジェクト（色がついている方）
    [SerializeField] private GameObject barElement;

    /// <summary>
    /// テキスト（HP/MPの値）をセットする
    /// </summary>
    /// <param name="text"></param>
    public void SetTextField(string text)
    {
        textField.text = text;
    }

    /// <summary>
    /// 引数で与えられた割合を元に、バーの長さを変える
    /// </summary>
    /// <param name="rate"></param>
    public void SetBarElementSize(float rate)
    {
        var totalWidth = barParent.GetComponent<RectTransform>().rect.width;
        var marginWidth = totalWidth * (1.0f - rate);  // 余白の横幅を求めている
        barElement.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0); // こちらはLeftとTopを設定しているらしい
        barElement.GetComponent<RectTransform>().offsetMax = new Vector2(-1.0f * marginWidth, 0);   // こっちはRightとBottomを設定しているみたい
    }

    /// <summary>
    /// 引数で与えられた「Color32」（＝色）をbarElementに適応する
    /// HPだったら緑、MPだったら青とか
    /// </summary>
    /// <param name="color"></param>
    public void SetElementBarColor(Color32 color)
    {
        barElement.GetComponent<Image>().color = color;
    }
}
