using UnityEngine;
using TMPro;
using System.Collections.Generic;

    /// <summary>
    /// 選択ウィンドウのUIを制御するクラスです。
    /// </summary>
    public class SelectionUIController : MonoBehaviour, IBattleUIController
    {
        /// <summary>
        /// 説明テキストへの参照です。
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _descriptionText;

        /// <summary>
        /// 左上の項目のコントローラへの参照です。
        /// </summary>
        [SerializeField]
        SelectionItemController _controllerLeftTop;

        /// <summary>
        /// 右上の項目のコントローラへの参照です。
        /// </summary>
        [SerializeField]
        SelectionItemController _controllerRightTop;

        /// <summary>
        /// 左下の項目のコントローラへの参照です。
        /// </summary>
        [SerializeField]
        SelectionItemController _controllerLeftBottom;

        /// <summary>
        /// 右下の項目のコントローラへの参照です。
        /// </summary>
        [SerializeField]
        SelectionItemController _controllerRightBottom;

        /// <summary>
        /// 前のページがあることを示すカーソルのオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjPrev;

        /// <summary>
        /// 次のページがあることを示すカーソルのオブジェクトです。
        /// </summary>
        [SerializeField]
        GameObject _cursorObjNext;

        /// <summary>
        /// 位置とコントローラの対応辞書です。
        /// </summary>
        Dictionary<int, SelectionItemController> _controllerDictionary = new();

        /// <summary>
        /// 位置とコントローラの対応辞書をセットアップします。
        /// </summary>
        public void SetUpControllerDictionary()
        {
            _controllerDictionary = new Dictionary<int, SelectionItemController>
            {
                {SelectionItemPosition.LeftTop, _controllerLeftTop},
                {SelectionItemPosition.RightTop, _controllerRightTop},
                {SelectionItemPosition.LeftBottom, _controllerLeftBottom},
                {SelectionItemPosition.RightBottom, _controllerRightBottom},
            };
        }

        /// <summary>
        /// 項目のカーソルをすべて非表示にします。
        /// </summary>
        void HideAllCursor()
        {
            _controllerLeftTop.HideCursor();
            _controllerRightTop.HideCursor();
            _controllerLeftBottom.HideCursor();
            _controllerRightBottom.HideCursor();
        }

        /// <summary>
        /// 選択中の項目のカーソルを表示します。
        /// </summary>
        /// <param name="selectedPosition">選択中の位置</param>
        public void ShowSelectedCursor(int selectedPosition)
        {
            HideAllCursor();

            _controllerDictionary.TryGetValue(selectedPosition, out SelectionItemController selectedController);
            if (selectedController != null)
            {
                selectedController.ShowCursor();
            }
        }

        /// <summary>
        /// 指定した位置の項目のテキストをセットします。
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="itemName">項目名</param>
        /// <param name="itemNum">項目数</param>
        /// <param name="canSelect">選択可能かどうか</param>
        public void SetItemText(int position, string itemName, int itemNum, bool canSelect)
        {
            _controllerDictionary.TryGetValue(position, out SelectionItemController controller);
            if (controller != null)
            {
                controller.SetItemText(itemName, itemNum);
                controller.SetItemTextColors(canSelect);
            }
        }

        /// <summary>
        /// 指定した位置の項目のテキストを初期化します。
        /// </summary>
        /// <param name="position">位置</param>
        public void ClearItemText(int position)
        {
            _controllerDictionary.TryGetValue(position, out SelectionItemController controller);
            if (controller != null)
            {
                controller.ClearItemText();
            }
        }

        /// <summary>
        /// 全ての項目のテキストを初期化します。
        /// </summary>
        public void ClearAllItemText()
        {
            foreach (var controller in _controllerDictionary.Values)
            {
                controller.ClearItemText();
            }
        }

        /// <summary>
        /// 説明テキストをセットします。
        /// </summary>
        /// <param name="description">説明</param>
        public void SetDescriptionText(string description)
        {
            _descriptionText.text = description;
        }

        /// <summary>
        /// 説明テキストを初期化します。
        /// </summary>
        public void ClearDescriptionText()
        {
            _descriptionText.text = string.Empty;
        }

        /// <summary>
        /// 前のページがあることを示すカーソルを表示します。
        /// </summary>
        /// <param name="isVisible">表示するかどうか</param>
        public void SetPrevCursorVisibility(bool isVisible)
        {
            _cursorObjPrev.SetActive(isVisible);
        }

        /// <summary>
        /// 次のページがあることを示すカーソルを表示します。
        /// </summary>
        /// <param name="isVisible">表示するかどうか</param>
        public void SetNextCursorVisibility(bool isVisible)
        {
            _cursorObjNext.SetActive(isVisible);
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