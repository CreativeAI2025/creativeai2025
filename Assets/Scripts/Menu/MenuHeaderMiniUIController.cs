using UnityEngine;
using UnityEngine.UI;

public class MenuHeaderMiniUIController : MonoBehaviour, IMenuUIController
{
    // ミニヘッダーのゲームオブジェクトへの参照
    [SerializeField] private MenuHeaderMiniField headerObject1;
    [SerializeField] private MenuHeaderMiniField headerObject2;
    [SerializeField] private MenuHeaderMiniField headerObject3;
    private const int NORMAL_HEIGHT = 50;   // 通常のヘッダーの高さは80
    private const int SELECTED_HEIGHT = 80; // 選択されているヘッダーの高さは50

    // ヘッダーの高さを50(=NORMAL_HEIGHT)に設定する
    public void SetSameHeight()
    {
        headerObject1.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(headerObject1.gameObject.GetComponent<RectTransform>().sizeDelta.x, NORMAL_HEIGHT);
        headerObject2.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(headerObject2.gameObject.GetComponent<RectTransform>().sizeDelta.x, NORMAL_HEIGHT);
        headerObject3.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(headerObject3.gameObject.GetComponent<RectTransform>().sizeDelta.x, NORMAL_HEIGHT);
    }

    public void SetHeight(int cursor)
    {
        if (cursor < 0 || cursor > 2)
        {
            Debug.Log("[MenuHeaderMiniUIController]SetHeight関数の引数が適切ではありません。");
            return;
        }

        switch (cursor)
        {
            case 0:
                headerObject1.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(headerObject1.gameObject.GetComponent<RectTransform>().sizeDelta.x, SELECTED_HEIGHT);
                break;
            case 1:
                headerObject2.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(headerObject2.gameObject.GetComponent<RectTransform>().sizeDelta.x, SELECTED_HEIGHT);
                break;
            case 2:
                headerObject3.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(headerObject3.gameObject.GetComponent<RectTransform>().sizeDelta.x, SELECTED_HEIGHT);
                break;
        }
    }

    /// <summary>
    /// タブをセットする
    /// </summary>
    /// <param name="text"></param> <summary>
    /// タブの項目に、引数で与えられた文字列を表示させる
    /// なお、string.Emptyのときは、該当するタブが非表示になる
    /// </summary>
    /// <param name="text"></param>
    public void SetHeaderObject1(string text)
    {
        headerObject1.SetHeaderMiniField(text);
    }
    public void SetHeaderObject2(string text)
    {
        headerObject2.SetHeaderMiniField(text);
    }
    public void SetHeaderObject3(string text)
    {
        headerObject3.SetHeaderMiniField(text);
    }

    /// <summary>
    /// 初期化させる（タブにはなにも表示されない）
    /// </summary>
    public void Initialize()
    {
        SetHeaderObject1(string.Empty);
        SetHeaderObject2(string.Empty);
        SetHeaderObject3(string.Empty);
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
