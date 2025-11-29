using UnityEngine;
using TMPro;

public class MenuSkillUIController : MonoBehaviour, IMenuUIController
{
    /// <summary>
    /// スキルリストのn番目の名前テキストへの参照
    /// </summary>
    [SerializeField] TextMeshProUGUI _skill1NameText;
    [SerializeField] TextMeshProUGUI _skill2NameText;
    [SerializeField] TextMeshProUGUI _skill3NameText;
    [SerializeField] TextMeshProUGUI _skill4NameText;
    [SerializeField] TextMeshProUGUI _skill5NameText;

    /// <summary>
    /// 選択されているスキルの名前、個数、説明テキストへの参照
    /// </summary>
    [SerializeField] TextMeshProUGUI _skillSelectedNameText;
    [SerializeField] TextMeshProUGUI _skillSelectedValueText;
    [SerializeField] TextMeshProUGUI _skillSelectedDiscText;

    /// <summary>
    /// 各テキストへ代入する
    /// </summary>
    public void SetSkill1NameText(string name)
    {
        _skill1NameText.text = name;
    }
    public void SetSkill2NameText(string name)
    {
        _skill2NameText.text = name;
    }
    public void SetSkill3NameText(string name)
    {
        _skill3NameText.text = name;
    }
    public void SetSkill4NameText(string name)
    {
        _skill4NameText.text = name;
    }
    public void SetSkill5NameText(string name)
    {
        _skill5NameText.text = name;
    }
    public void SetSkillSelectedNameText(string name)
    {
        _skillSelectedNameText.text = name;
    }
    public void SetSkillSelectedValueText(int value)
    {
        _skillSelectedValueText.text = value.ToString();
    }
    public void SetSkillSelectedDiscText(string text)
    {
        _skillSelectedDiscText.text = text;
    }

    /// <summary>
    /// テキストの表示の初期化
    /// </summary>
    public void InitializeText()
    {
        _skill1NameText.text = string.Empty;
        _skill2NameText.text = string.Empty;
        _skill3NameText.text = string.Empty;
        _skill4NameText.text = string.Empty;
        _skill5NameText.text = string.Empty;
        _skillSelectedNameText.text = string.Empty;
        _skillSelectedValueText.text = string.Empty;
        _skillSelectedDiscText.text = string.Empty;
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
