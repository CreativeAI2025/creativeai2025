using UnityEngine;
using UnityEngine.UI;

public class MenuCharacterStatusUI : MonoBehaviour
{
    // キャラクターのイメージ
    [SerializeField] private Image characterImage;
    // HPバーのUIを管理するスクリプトへの参照
    [SerializeField] private MenuBarUIController hpbarController;
    // MPバー
    [SerializeField] private MenuBarUIController mpbarController;
    private Color32 hpColor = new Color32(180, 250, 145, 192);  // HPバーの色
    private Color32 mpColor = new Color32(145, 190, 250, 192);  // MPバーの色
    private Color32 nonselectedColor = new Color32(159, 159, 159, 255);    // このオブジェクトが選択されていないときの、キャラクターの色（じゃっかん暗く）
    private Color32 selectedColor = new Color32(255, 255, 255, 255);    // このオブジェクトが選択されているときの、キャラクターの色（明るく）

    /// <summary>
    /// キャラクターのスプライトをセットする
    /// </summary>
    /// <param name="sprite"></param>
    public void SetCharacterSprite(Sprite sprite)
    {
        characterImage.sprite = sprite;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize()
    {
        // バーの色をセットする
        hpbarController.SetElementBarColor(hpColor);
        mpbarController.SetElementBarColor(mpColor);
    }

    /// <summary>
    /// HPバーの長さを、バー全体の任意の割合（引数で設定）にする
    /// </summary>
    /// <param name="rate"></param>
    public void SetHpbarSize(float rate)
    {
        hpbarController.SetBarElementSize(rate);
    }

    /// <summary>
    /// MPバーの長さを、バー全体の任意の割合（引数で設定）にする
    /// </summary>
    /// <param name="rate"></param>
    public void SetMpbarSize(float rate)
    {
        mpbarController.SetBarElementSize(rate);
    }

    /// <summary>
    /// バーの上に表示されるHPの値のテキストを設定する。
    /// 引数には、現在のHPとその最大値を与える
    /// </summary>
    /// <param name="currentValue"></param>
    /// <param name="maxValue"></param>
    public void SetHPTextFromValue(int currentValue, int maxValue)
    {
        string text = $"HP: {currentValue} / {maxValue}";
        hpbarController.SetTextField(text);
    }

    /// <summary>
    /// バーの上に表示されるMPの値のテキストを設定する。
    /// 引数には、現在のMPとその最大値を与える
    /// </summary>
    /// <param name="currentValue"></param>
    /// <param name="maxValue"></param>
    public void SetMPTextFromValue(int currentValue, int maxValue)
    {
        string text = $"MP: {currentValue} / {maxValue}";
        mpbarController.SetTextField(text);
    }

    /// <summary>
    /// このゲームオブジェクトが選択されていないときに、キャラクターを暗くする
    /// </summary>
    public void MakeCharacterImageDark()
    {
        characterImage.color = nonselectedColor;
    }

    /// <summary>
    /// このゲームオブジェクトが選択されているときに、キャラクターを明るくする
    /// </summary>
    public void MakeCharacterImageBright()
    {
        characterImage.color = selectedColor;
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
