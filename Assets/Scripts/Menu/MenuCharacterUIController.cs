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

    [SerializeField] TextMeshProUGUI _hpValueText;

    [SerializeField] TextMeshProUGUI _mpValueText;
    [SerializeField] TextMeshProUGUI _attackValueText;
    [SerializeField] TextMeshProUGUI _defenceValueText;
    [SerializeField] TextMeshProUGUI _magicAtkValueText;
    [SerializeField] TextMeshProUGUI _magicDefValueText;
    [SerializeField] TextMeshProUGUI _speedValueText;
    [SerializeField] TextMeshProUGUI _dodgeValueText;
    [SerializeField] Image _characterImage;

    public void SetCharacterNameText(string name)
    {
        _characterNameText.text = name;
    }
    public void SetLevelValueText(int level)
    {
        _levelValueText.text = level.ToString();
    }
    public void SetHPValueText(int currentHP, int maxHP)
    {
        _hpValueText.text = $"HP: {currentHP}/{maxHP}";
    }
    public void SetMPValueText(int currentMP, int maxMP)
    {
        _mpValueText.text = $"MP: {currentMP}/{maxMP}";
    }
    public void SetAttackValueText(int attack)
    {
        _attackValueText.text = attack.ToString();
    }
    public void SetDefenceValueText(int defence)
    {
        _defenceValueText.text = defence.ToString();
    }
    public void SetMagicAtkValueText(int MagicAtk)
    {
        _magicAtkValueText.text = MagicAtk.ToString();
    }
    public void SetMagicDefValueText(int MagicDef)
    {
        _magicDefValueText.text = MagicDef.ToString();
    }
    public void SetSpeedValueText(int speed)
    {
        _speedValueText.text = speed.ToString();
    }
    public void SetDodgeValueText(int dodge)
    {
        _dodgeValueText.text = dodge.ToString();
    }
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
        _hpValueText.text = string.Empty;
        _mpValueText.text = string.Empty;
        _attackValueText.text = string.Empty;
        _defenceValueText.text = string.Empty;
        _magicAtkValueText.text = string.Empty;
        _magicDefValueText.text = string.Empty;
        _speedValueText.text = string.Empty;
        _dodgeValueText.text = string.Empty;
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
