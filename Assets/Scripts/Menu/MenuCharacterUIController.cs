using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuCharacterUIController : MonoBehaviour, IMenuUIController
{
    /// <summary>
    /// キャラクターの名前テキストへの参照
    /// </summary>
    [SerializeField] TextMeshProUGUI _characterNameText;

    /// <summary>
    /// レベルの値テキストへの参照
    /// </summary>
    [SerializeField] TextMeshProUGUI _levelValueText;

    [SerializeField] MenuBarUIController _hpbarField;

    [SerializeField] MenuBarUIController _mpbarField;
    [SerializeField] TextMeshProUGUI _attackValueText;
    [SerializeField] TextMeshProUGUI _defenceValueText;
    [SerializeField] TextMeshProUGUI _magicAtkValueText;
    [SerializeField] TextMeshProUGUI _magicDefValueText;
    [SerializeField] TextMeshProUGUI _speedValueText;
    //[SerializeField] TextMeshProUGUI _dodgeValueText;
    [SerializeField] Image _characterImage;
    private Color32 hpColor = new Color32(180, 250, 145, 192);  // HPバーの色
    private Color32 mpColor = new Color32(145, 190, 250, 192);  // MPバーの色

    public void SetCharacterNameText(string name)
    {
        _characterNameText.text = name;
    }
    public void SetLevelValueText(int level)
    {
        _levelValueText.text = $"Lv. {level.ToString()}";
    }
    public void SetHPValueText(int currentHP, int maxHP)
    {
        string text = $"HP: {currentHP} / {maxHP}";
        float rate = (float)currentHP / (float)maxHP;
        _hpbarField.SetTextField(text);
        _hpbarField.SetBarElementSize(rate);
    }
    public void SetMPValueText(int currentMP, int maxMP)
    {
        string text = $"MP: {currentMP} / {maxMP}";
        float rate = (float)currentMP / (float)maxMP;
        _mpbarField.SetTextField(text);
        _mpbarField.SetBarElementSize(rate);
    }
    public void SetAttackValueText(int attack)
    {
        _attackValueText.text = $"攻撃：{attack.ToString()}";
    }
    public void SetDefenceValueText(int defence)
    {
        _defenceValueText.text = $"防御：{defence.ToString()}";
    }
    public void SetMagicAtkValueText(int MagicAtk)
    {
        _magicAtkValueText.text = $"魔法攻撃：{MagicAtk.ToString()}";
    }
    public void SetMagicDefValueText(int MagicDef)
    {
        _magicDefValueText.text = $"魔法防御：{MagicDef.ToString()}";
    }
    public void SetSpeedValueText(int speed)
    {
        _speedValueText.text = "$素早さ：{speed.ToString()}";
    }
    /*public void SetDodgeValueText(int dodge)
    {
        _dodgeValueText.text = dodge.ToString();
    }*/
    public void SetCharacterSprite(Sprite sprite)
    {
        _characterImage.sprite = sprite;
    }

    /// <summary>
    /// テキストの表示の初期化
    /// </summary>
    public void InitializeText()
    {
        _characterNameText.text = string.Empty;
        _levelValueText.text = string.Empty;
        _hpbarField.SetElementBarColor(hpColor);
        _mpbarField.SetElementBarColor(mpColor);
        _attackValueText.text = string.Empty;
        _defenceValueText.text = string.Empty;
        _magicAtkValueText.text = string.Empty;
        _magicDefValueText.text = string.Empty;
        _speedValueText.text = string.Empty;
        //_dodgeValueText.text = string.Empty;
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
