using UnityEngine;
using TMPro;

    /// <summary>
    /// 選択ウィンドウの各項目を制御するクラスです。
    /// </summary>
    public class SelectionItemController : MonoBehaviour
    {
        /// <summary>
        /// 項目のカーソルオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObj;

        /// <summary>
        /// 選択項目の名前テキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _itemNameText;

        /// <summary>
        /// 選択項目の数値テキストです。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _numberText;

        /// <summary>
        /// 項目が選択可能なときの色です。
        /// </summary>
        [SerializeField]
        Color _selectableColor = Color.white;

        /// <summary>
        /// 項目が選択不可能なときの色です。
        /// </summary>
        [SerializeField]
        Color _unselectableColor = Color.gray;

        /// <summary>
        /// 選択項目のテキストをセットします。
        /// </summary>
        /// <param name="itemName">項目名</param>
        /// <param name="itemNum">項目数</param>
        public void SetItemText(string itemName, int itemNum)
        {
            _itemNameText.text = itemName;
            _numberText.text = itemNum.ToString();
        }

        /// <summary>
        /// 選択項目のテキストを初期化します。
        /// </summary>
        public void ClearItemText()
        {
            _itemNameText.text = "";
            _numberText.text = "";
        }

        /// <summary>
        /// 選択項目のテキストの色をセットします。
        /// </summary>
        /// <param name="isSelectable">選択可能かどうか</param>
        public void SetItemTextColors(bool isSelectable)
        {
            SetTextColor(_itemNameText, isSelectable);
            SetTextColor(_numberText, isSelectable);
        }

        /// <summary>
        /// 選択項目のテキストの色をセットします。
        /// </summary>
        /// <param name="text">対象のテキスト</param>
        /// <param name="isSelectable">選択可能かどうか</param>
        void SetTextColor(TextMeshProUGUI text, bool isSelectable)
        {
            text.color = isSelectable ? _selectableColor : _unselectableColor;
        }

        /// <summary>
        /// 選択項目のカーソルを表示します。
        /// </summary>
        public void ShowCursor()
        {
            _cursorObj.SetActive(true);
        }

        /// <summary>
        /// 選択項目のカーソルを非表示にします。
        /// </summary>
        public void HideCursor()
        {
            _cursorObj.SetActive(false);
        }
    }