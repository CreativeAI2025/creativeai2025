using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class MenuItemWindowController : MonoBehaviour, IMenuWindowController
{
    MenuManager _menuManager;
    [SerializeField] private MenuItemUIController _uiController;
    private bool _canClose;
    private InputSetting _inputSetting;
    private int _itemClassification;    // アイテムの分類

    public void SetUpController(MenuManager menuManager)
    {
        _menuManager = menuManager;
    }

    /// <summary>
    /// スキルの一覧を画面に表示する
    /// </summary>
    public void SetUpItem()
    {
        _itemClassification = 0;
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

    private void CheckKeyInput()
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
        if (_inputSetting.GetCancelKeyDown())
        {
            StartCoroutine(HideProcess());
        }
        else if (_inputSetting.GetRightKeyDown())
        {
            StartCoroutine(SwitchItemClassification());
        }
        else if (_inputSetting.GetLeftKeyDown())
        {
            StartCoroutine(SwitchItemClassification());
        }
    }
    private IEnumerator HideProcess()
    {
        _canClose = false;
        yield return null;
        MenuManager.Instance.OnItemCanceled();
        HideWindow();
    }

    public void ShowWindow()
    {
        // _uiController.InitializeText(); // テキストの初期化
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

    public void HideWindow()
    {
        _uiController.Hide();
    }

    /// <summary>
    /// アイテムの分類を切り替える
    /// 通常アイテム←→重要アイテム
    /// </summary>
    private IEnumerator SwitchItemClassification()
    {
        yield return null;
        _itemClassification++;
        string log = "Switch ItemClassification to ";
        if (_itemClassification % 2 == 1)
        {
            log += "IMPORTANT";
        }
        else
        {
            log += "NORMAL";
        }
        Debug.Log(log);
    }
}
