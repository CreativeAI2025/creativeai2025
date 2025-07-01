using UnityEngine;
using TMPro;


/// <summary>
/// ステータス表示のUIを制御するクラスです。
/// </summary>
public class StatusUIController : MonoBehaviour, IBattleUIController
{
    /// <summary>
    /// キャラクターの名前を表示するテキストです。
    /// </summary>
    [SerializeField]
    TextMeshProUGUI _characterNameText;

    /// <summary>
    /// 現在のHPを表示するテキストです。
    /// </summary>
    [SerializeField]
    TextMeshProUGUI _currentHpText;

    /// <summary>
    /// 最大HPを表示するテキストです。
    /// </summary>
    [SerializeField]
    TextMeshProUGUI _maxHpText;

    /// <summary>
    /// 現在のMPを表示するテキストです。
    /// </summary>
    [SerializeField]
    TextMeshProUGUI _currentMpText;

    /// <summary>
    /// 最大MPを表示するテキストです。
    /// </summary>
    [SerializeField]
    TextMeshProUGUI _maxMpText;

    /// <summary>
    /// キャラクターの名前をセットします。
    /// </summary>
    /// <param name="characterName">キャラクターの名前</param>
    public void SetCharacterName(string characterName)
    {
        _characterNameText.text = characterName;
    }

    /// <summary>
    /// 現在のHPをセットします。
    /// </summary>
    /// <param name="currentHp">現在のHP</param>
    public void SetCurrentHp(int currentHp)
    {
        _currentHpText.text = currentHp.ToString();
    }

    /// <summary>
    /// 最大HPをセットします。
    /// </summary>
    /// <param name="maxHp">最大HP</param>
    public void SetMaxHp(int maxHp)
    {
        _maxHpText.text = maxHp.ToString();
    }

    /// <summary>
    /// 現在のMPをセットします。
    /// </summary>
    /// <param name="currentMp">現在のMP</param>
    public void SetCurrentMp(int currentMp)
    {
        _currentMpText.text = currentMp.ToString();
    }

    /// <summary>
    /// 最大MPをセットします。
    /// </summary>
    /// <param name="maxMp">最大MP</param>
    public void SetMaxMp(int maxMp)
    {
        _maxMpText.text = maxMp.ToString();
    }

    /// <summary>
    /// UIを表示します。
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// UIを非表示にします。
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
