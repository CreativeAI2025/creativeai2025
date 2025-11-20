using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class MenuItemWindowController : MonoBehaviour, IMenuWindowController
{
    [SerializeField] MenuItemUIController _uiController;
    [SerializeField] private MenuHeaderMiniUIController _headerUIController;
    private bool _canClose;
    private InputSetting _inputSetting;
    private int _itemListCursor; // スキルリスト内のどの位置を指しているかを数字で表す
    private ItemData _selectedItemData;    // _itemListCursorが示すスキルのデータ
    private List<int> _itemList; // スキルのリスト（１キャラクター）
    private int _characterIndex;
    private int _characterIndexMax;
    private const string NORMAL_ITEM_TEXT = "消費アイテム";
    private const string IMPORTANT_ITEM_TEXT = "大切なアイテム";
    private bool stop;  // アイテムリストがnullのときは、キャンセルのみ受け付けるようにする

    /// <summary>
    /// スキルの一覧を画面に表示する
    /// </summary>
    private void SetUpItem()
    {
        stop = false;
        _itemListCursor = 0;
        SetItemList(); // スキルリストをセットする
        if (_itemList == null)
        {
            stop = true;
            Debug.Log("[MenuItemWindowController]itemListがnull");
            return;
        }
        if (_itemList.Count == 0)
        {
            stop = true;
            Debug.Log("[MenuItemWindowController]itemListの要素が０");
            return;
        }
        _selectedItemData = ItemDataManager.Instance.GetItemDataById(_itemList[_itemListCursor]);
        SetText();

        // ヘッダーの設定
        _headerUIController.Initialize();
        _headerUIController.SetHeight(1);

        _headerUIController.SetHeaderObject1(NORMAL_ITEM_TEXT);
        _headerUIController.SetHeaderObject2(IMPORTANT_ITEM_TEXT);
        _headerUIController.SetHeaderObject3(string.Empty);

        _characterIndex = 0;
        _characterIndexMax = 2;    // タブが「消費アイテム」と「大切なもの」の２種類のため、２を代入する。
        _headerUIController.SetSameHeight();
        _headerUIController.SetHeight(_characterIndex); // キャラクターの添え字にあるタブを大きくする
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputSetting = InputSetting.Load();
    }

    // Update is called once per frame
    void Update()
    {
        CheckKeyInput();
    }

    void CheckKeyInput()
    {
        if (MenuManager.Instance == null)
        {
            return;
        }
        if (MenuManager.Instance.MenuPhase != MenuPhase.Item)
        {
            return;
        }
        if (!_canClose)
        {
            return;
        }
        // アイテム/スキル使用ウィンドウが開いていたら、操作を受け付けない
        if (MenuManager.Instance.MenuUsePhase != MenuUsePhase.Closed)
        {
            return;
        }
        if (_inputSetting.GetCancelKeyDown())
        {
            StartCoroutine(HideProcess());
        }
        else if (stop)
        {
            return;
        }
        else if (_inputSetting.GetBackKeyDown())
        {
            ShowNextItem();
        }
        else if (_inputSetting.GetForwardKeyDown())
        {
            ShowPreviousItem();
        }
        else if (_inputSetting.GetDecideInputDown())
        {
            MenuManager.Instance.OnOpenSelectWindow(MenuUsePhase.ItemUse, -1);
            SoundManager.Instance.PlaySE(3);
        }
        else if (_inputSetting.GetRightKeyDown())
        {
            StartCoroutine(NextPage());
        }
        else if (_inputSetting.GetLeftKeyDown())
        {
            StartCoroutine(PreviousPage());
        }
    }
    private IEnumerator HideProcess()
    {
        _canClose = false;
        yield return null;
        MenuManager.Instance.OnItemCanceled();
        SoundManager.Instance.PlaySE(3);
        HideWindow();
    }

    public void ShowWindow()
    {
        _uiController.InitializeText(); // テキストの初期化
        _headerUIController.Initialize();
        SetUpItem();
        _uiController.Show();
        _canClose = false;

        StartCoroutine(SetCloseStateDelay());
    }

    private IEnumerator SetCloseStateDelay()
    {
        yield return null;
        _canClose = true;
    }

    private IEnumerator SetOpenStateDelay()
    {
        yield return null;
        _canClose = false;
    }

    public void HideWindow()
    {
        _uiController.Hide();
        _canClose = true;
        StartCoroutine(SetOpenStateDelay());
    }

    /// <summary>
    /// 次のスキルを表示する
    /// </summary>
    private void ShowNextItem()
    {
        _itemListCursor++;
        _itemListCursor = _itemListCursor % _itemList.Count;
        _selectedItemData = ItemDataManager.Instance.GetItemDataById(_itemList[_itemListCursor]);
        SetText();
        SoundManager.Instance.PlaySE(1);
    }

    private void ShowPreviousItem()
    {
        _itemListCursor--;
        _itemListCursor = (_itemListCursor + _itemList.Count) % _itemList.Count;
        _selectedItemData = ItemDataManager.Instance.GetItemDataById(_itemList[_itemListCursor]);
        SetText();
        SoundManager.Instance.PlaySE(1);
    }

    /// <summary>
    /// キャラクタータブを次（右）に移動する
    /// </summary>
    /// <returns></returns>
    private IEnumerator NextPage()
    {
        _characterIndex++;
        if (_characterIndex >= _characterIndexMax)
        {
            _characterIndex = 0;
        }
        yield return null;

        _headerUIController.SetSameHeight();
        _headerUIController.SetHeight(_characterIndex);

        SetItemList();
        InitializePage();
        SoundManager.Instance.PlaySE(1);
    }

    /// <summary>
    /// キャラクタータブを前（左）に移動する
    /// </summary>
    /// <returns></returns>
    private IEnumerator PreviousPage()
    {
        _characterIndex--;
        if (_characterIndex < 0)
        {
            _characterIndex = _characterIndexMax - 1;
        }
        yield return null;

        _headerUIController.SetSameHeight();
        _headerUIController.SetHeight(_characterIndex);

        SetItemList();
        InitializePage();
        SoundManager.Instance.PlaySE(1);
    }

    /// <summary>
    /// スキルリストのカーソルを一番上にするなどの、
    /// ページの切り替えを行った際に行われる初期化
    /// </summary>
    private void InitializePage()
    {
        _itemListCursor = 0;
        _selectedItemData = ItemDataManager.Instance.GetItemDataById(_itemList[_itemListCursor]);
        SetText();
    }

    /// <summary>
    /// 引数で与えられた値にあるスキルの名前を返す
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    private string GetItemNameByCursor(int n)
    {
        if (_itemList == null)
        {
            Debug.Log("[MenuItemWindowController]_itemListがnull");
            return string.Empty;
        }
        if (n < 0 || n > _itemList.Count - 1)
        {
            return "---";
        }
        ItemData sd = ItemDataManager.Instance.GetItemDataById(_itemList[n]);
        return sd.itemName;
    }

    /// <summary>
    /// スキル画面のテキストを表示する
    /// </summary>
    private void SetText()
    {
        _uiController.SetItem1NameText(GetItemNameByCursor(_itemListCursor - 2));
        _uiController.SetItem2NameText(GetItemNameByCursor(_itemListCursor - 1));
        _uiController.SetItem3NameText(GetItemNameByCursor(_itemListCursor));
        _uiController.SetItem4NameText(GetItemNameByCursor(_itemListCursor + 1));
        _uiController.SetItem5NameText(GetItemNameByCursor(_itemListCursor + 2));
        _uiController.SetItemSelectedDiscText(_selectedItemData.itemDesc);
        _uiController.SetItemSelectedNameText(_selectedItemData.itemName);
        _uiController.SetItemSelectedValueText(10);
    }

    // 現在選択されているスキルデータを返す
    public ItemData getItemData()
    {
        return _selectedItemData;
    }

    /// <summary>
    /// アイテムの種類により、表示するアイテムリストを変える
    /// </summary>
    private void SetItemList()
    {
        _itemList = new();
        foreach (var iteminfo in CharacterStatusManager.Instance.partyItemInfoList)
        {
            _itemList.Add(iteminfo.itemId);
        }
    }
}
