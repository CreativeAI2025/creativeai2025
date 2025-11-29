using UnityEngine;
using TMPro;

public class MenuItemUIController : MonoBehaviour, IMenuUIController
{
    /// <summary>
    /// スキルリストのn番目の名前テキストへの参照
    /// </summary>
    [SerializeField] TextMeshProUGUI _item1NameText;
    [SerializeField] TextMeshProUGUI _item2NameText;
    [SerializeField] TextMeshProUGUI _item3NameText;
    [SerializeField] TextMeshProUGUI _item4NameText;
    [SerializeField] TextMeshProUGUI _item5NameText;

    /// <summary>
    /// 選択されているスキルの名前、個数、説明テキストへの参照
    /// </summary>
    [SerializeField] TextMeshProUGUI _itemSelectedNameText;
    [SerializeField] TextMeshProUGUI _itemSelectedValueText;
    [SerializeField] TextMeshProUGUI _itemSelectedDiscText;

    /// <summary>
    /// 各テキストへ代入する
    /// </summary>
    public void SetItem1NameText(string name)
    {
        _item1NameText.text = name;
    }
    public void SetItem2NameText(string name)
    {
        _item2NameText.text = name;
    }
    public void SetItem3NameText(string name)
    {
        _item3NameText.text = name;
    }
    public void SetItem4NameText(string name)
    {
        _item4NameText.text = name;
    }
    public void SetItem5NameText(string name)
    {
        _item5NameText.text = name;
    }
    public void SetItemSelectedNameText(string name)
    {
        _itemSelectedNameText.text = name;
    }
    public void SetItemSelectedValueText(int value)
    {
        _itemSelectedValueText.text = value.ToString();
    }
    public void SetItemSelectedDiscText(string text)
    {
        _itemSelectedDiscText.text = text;
    }

    /// <summary>
    /// テキストの表示の初期化
    /// </summary>
    public void InitializeText()
    {
        _item1NameText.text = string.Empty;
        _item2NameText.text = string.Empty;
        _item3NameText.text = string.Empty;
        _item4NameText.text = string.Empty;
        _item5NameText.text = string.Empty;
        _itemSelectedNameText.text = string.Empty;
        _itemSelectedValueText.text = string.Empty;
        _itemSelectedDiscText.text = string.Empty;
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
