using UnityEngine;
using UnityEngine.UI;

public class MenuHeaderMiniUIController : MonoBehaviour, IMenuUIController
{
    // ミニヘッダーのゲームオブジェクトへの参照
    [SerializeField] private GameObject headerObject1;
    [SerializeField] private GameObject headerObject2;
    [SerializeField] private GameObject headerObject3;
    private const int NORMAL_HEIGHT = 50;   // 通常のヘッダーの高さは80
    private const int SELECTED_HEIGHT = 80; // 選択されているヘッダーの高さは50

    // ヘッダーの高さを50(=NORMAL_HEIGHT)に設定する
    private void SetSameHeight()
    {
        headerObject1.GetComponent<RectTransform>().sizeDelta = new Vector2(headerObject1.GetComponent<RectTransform>().sizeDelta.x, NORMAL_HEIGHT);
        headerObject2.GetComponent<RectTransform>().sizeDelta = new Vector2(headerObject2.GetComponent<RectTransform>().sizeDelta.x, NORMAL_HEIGHT);
        headerObject3.GetComponent<RectTransform>().sizeDelta = new Vector2(headerObject3.GetComponent<RectTransform>().sizeDelta.x, NORMAL_HEIGHT);
    }

    public void InitializeHeader(int size)
    {
        switch (size)
        {
            case 1:
                headerObject1.SetActive(true);
                break;
        }
    }

    public void SetSelectedHeaderMini(int cursor)
    {

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
