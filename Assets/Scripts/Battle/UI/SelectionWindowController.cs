using UnityEngine;
using System.Collections.Generic;


    /// <summary>
    /// 選択ウィンドウを制御するクラスです。
    /// </summary>
    public class SelectionWindowController : MonoBehaviour, IBattleWindowController
    {
        /// <summary>
        /// 戦闘に関する機能を管理するクラスへの参照です。
        /// </summary>
        BattleManager _battleManager;

        /// <summary>
        /// 選択ウィンドウのUIを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        SelectionUIController _uiController;

        /// <summary>
        /// 選択ウィンドウにてアイテムに関する処理を制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        SelectionWindowSkillController _skillController;

        /// <summary>
        /// 選択ウィンドウにて魔法に関する処理を制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        SelectionWindowItemController _itemController;

        /// <summary>
        /// 現在選択中の項目のインデックスです。
        /// </summary>
        int _selectedIndex;

        /// <summary>
        /// 現在のページ数です。
        /// </summary>
        int _page;

        /// <summary>
        /// 項目選択が可能かどうかのフラグです。
        /// </summary>
        bool _canSelect;

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        /// <param name="battleManager">戦闘に関する機能を管理するクラス</param>
        public void SetUpController(BattleManager battleManager)
        {
            _battleManager = battleManager;
        }

        void Update()
        {
            SelectItem();
        }

        /// <summary>
        /// コマンドを選択します。
        /// </summary>
        void SelectItem()
        {
            if (_battleManager == null)
            {
                return;
            }

            if (_battleManager.BattlePhase != BattlePhase.SelectItem)
            {
                return;
            }

            if (!_canSelect)
            {
                return;
            }

            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                SelectUpperItem();
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                SelectLowerItem();
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                SelectRightItem();
            }
            else if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                SelectLeftItem();
            }
            else if (InputGameKey.ConfirmButton())
            {
                OnPressedConfirmButton();
            }
            else if (InputGameKey.CancelButton())
            {
                OnPressedCancelButton();
            }
        }

        /// <summary>
        /// インデックスが有効な範囲か確認します。
        /// </summary>
        /// <param name="index">確認するインデックス</param>
        bool IsValidIndex(int index)
        {
            bool isValid = false;
            if (_battleManager.SelectedCommand == BattleCommand.Skill)
            {
                isValid = _skillController.IsValidIndex(index);
            }
            else if (_battleManager.SelectedCommand == BattleCommand.Item)
            {
                isValid = _itemController.IsValidIndex(index);
            }
            return isValid;
        }

        /// <summary>
        /// 選択中の項目が実行できるか確認します。
        /// 魔法の場合は消費MPを確認、アイテムの場合は所持数を確認します。
        /// </summary>
        bool IsValidSelection()
        {
            bool isValid = false;
            if (_battleManager.SelectedCommand == BattleCommand.Skill)
            {
                isValid = _skillController.IsValidSelection(_selectedIndex);
            }
            else if (_battleManager.SelectedCommand == BattleCommand.Item)
            {
                isValid = _itemController.IsValidSelection(_selectedIndex);
            }
            return isValid;
        }

        /// <summary>
        /// 右側の項目を選択します。
        /// </summary>
        void SelectRightItem()
        {
            // 左側の項目を選択していない場合は何もしません。
            if (!IsLeftColumn())
            {
                return;
            }

            // 移動先のインデックスに魔法が存在する場合はカーソルを移動します。
            if (IsValidIndex(_selectedIndex + 1))
            {
                _selectedIndex += 1;
            }

            ShowSelectionCursor();
        }

        /// <summary>
        /// 左側の項目を選択します。
        /// </summary>
        void SelectLeftItem()
        {
           // 右側の項目を選択していない場合は何もしません。
            if (IsLeftColumn())
            {
                return;
            }

            // 移動先のインデックスに魔法が存在する場合はカーソルを移動します。
            if (IsValidIndex(_selectedIndex - 1))
            {
                _selectedIndex -= 1;
            }

            ShowSelectionCursor();
        }

        /// <summary>
        /// 現在の選択位置が左側のカラムかどうかを確認します。
        /// </summary>
        bool IsLeftColumn()
        {
            int currentColmun = _selectedIndex % 2;
            int leftColumn = 0;
            return currentColmun == leftColumn;
        }

        /// <summary>
        /// ひとつ上の項目を選択します。
        /// </summary>
        void SelectUpperItem()
        {
            if (IsUpperRow())
            {
                if (_page > 0)
                {
                    _page -= 1;
                    SetPageElement();
                    _selectedIndex = _page * 4;
                }
            }
            else
            {
                if (IsValidIndex(_selectedIndex - 2))
                {
                    _selectedIndex -= 2;
                }
            }

            ShowSelectionCursor();
        }

        /// <summary>
        /// ひとつ下の項目を選択します。
        /// </summary>
        void SelectLowerItem()
        {
            if (IsUpperRow())
            {
                if (IsValidIndex(_selectedIndex + 2))
                {
                    _selectedIndex += 2;
                }
            }
            else
            {
                if (_page < GetMaxPageNum() - 1)
                {
                    _page += 1;
                    SetPageElement();
                    _selectedIndex = _page * 4;
                }
            }

            ShowSelectionCursor();
        }

        /// <summary>
        /// 現在の選択位置が上側の行かどうかを確認します。
        /// </summary>
        bool IsUpperRow()
        {
            int currentRow = _selectedIndex % 4;
            int upperRowMax = 1;
            return currentRow <= upperRowMax;
        }

        /// <summary>
        /// 最大ページ数を取得します。
        /// </summary>
        int GetMaxPageNum()
        {
            int maxPage = 0;
            if (_battleManager.SelectedCommand == BattleCommand.Skill)
            {
                maxPage = _skillController.GetMaxPageNum();
            }
            else if (_battleManager.SelectedCommand == BattleCommand.Item)
            {
                maxPage = _itemController.GetMaxPageNum();
            }
            return maxPage;
        }

        /// <summary>
        /// 選択中の位置に応じたカーソルを表示します。
        /// </summary>
        void ShowSelectionCursor()
        {
            int index = _selectedIndex % 4;
            _uiController.ShowSelectedCursor(index);
        }

        /// <summary>
        /// ウィンドウの状態をセットアップします。
        /// </summary>
        public void SetUpWindow()
        {
            _uiController.SetUpControllerDictionary();
            _uiController.ClearAllItemText();
            _uiController.ClearDescriptionText();
            InitializeSelect();
            _skillController.SetCharacterSkill();
        }

        /// <summary>
        /// 項目選択を初期化します。
        /// </summary>
        public void InitializeSelect()
        {
            _page = 0;
            _selectedIndex = SelectionItemPosition.LeftTop;
            _uiController.ShowSelectedCursor(_selectedIndex);
        }

        /// <summary>
        /// ページ内の項目をセットします。
        /// </summary>
        public void SetPageElement()
        {
            if (_battleManager.SelectedCommand == BattleCommand.Skill)
            {
                _skillController.SetPageSkill(_page, _uiController);
            }
            else if (_battleManager.SelectedCommand == BattleCommand.Item)
            {
                _itemController.SetPageItem(_page, _uiController);
            }

            // ページ送りのカーソルの表示状態を確認します。
            bool isVisiblePrevCursor = _page > 0;
            bool isVisibleNextCursor = _page < GetMaxPageNum() - 1;
            _uiController.SetPrevCursorVisibility(isVisiblePrevCursor);
            _uiController.SetNextCursorVisibility(isVisibleNextCursor);
        }

        /// <summary>
        /// 項目が選択された時の処理です。
        /// </summary>
        void OnPressedConfirmButton()
        {
            if (!IsValidSelection())
            {
                return;
            }

            if (_battleManager.SelectedCommand == BattleCommand.Skill)
            {
                var skillData = _skillController.GetSkillData(_selectedIndex);
                if (skillData != null)
                {
                    _battleManager.OnItemSelected(skillData.skillId);
                    HideWindow();
                    SetCanSelectState(false);
                }
            }
            else if (_battleManager.SelectedCommand == BattleCommand.Item)
            {
                var itemInfo = _itemController.GetItemInfo(_selectedIndex);
                if (itemInfo != null)
                {
                    _battleManager.OnItemSelected(itemInfo.itemId);
                    HideWindow();
                    SetCanSelectState(false);
                }
            }
        }

        /// <summary>
        /// キャンセルボタンが押された時の処理です。
        /// </summary>
        void OnPressedCancelButton()
        {
            _battleManager.OnItemCanceled();
            HideWindow();
            SetCanSelectState(false);
        }

        /// <summary>
        /// 選択ウィンドウを表示します。
        /// </summary>
        public void ShowWindow()
        {
            _uiController.Show();
        }

        /// <summary>
        /// 選択ウィンドウを非表示にします。
        /// </summary>
        public void HideWindow()
        {
            _uiController.Hide();
        }

        /// <summary>
        /// 項目選択が可能かどうかの状態を設定します。
        /// </summary>
        /// <param name="canSelect">項目選択が可能かどうか</param>
        public void SetCanSelectState(bool canSelect)
        {
            _canSelect = canSelect;
        }
    }