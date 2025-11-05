using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class MenuSelectWindowController : MonoBehaviour, IMenuWindowController
{
    [SerializeField] private MenuSelectUIController _uiController;
    private InputSetting _inputSetting;
    private int _selectPhase;
    private int _cursor;
    private int _cursorMax;
    private SkillData _skillData;
    private ItemData _itemData;

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
        if (MenuManager.Instance.MenuUsePhase == MenuUsePhase.Closed)
        {
            return;
        }
        if (_inputSetting.GetCancelKeyDown())
        {
            StartCoroutine(HideProcess());
        }
        else if (_inputSetting.GetRightKeyDown())
        {
            PointNext();
        }
        else if (_inputSetting.GetLeftKeyDown())
        {
            PointPrevious();
        }
        else if (_inputSetting.GetDecideInputDown())
        {
            if (_selectPhase == 0)
            {
                SetPhase2Text();
            }
            else
            {
                StartCoroutine(HideProcess());
            }
        }

    }

    /// <summary>
    /// このウィンドウに来た時に、カーソルを０にするなどの初期化を行う
    /// </summary>
    private void SetupSelectWindow()
    {
        List<int> ids = CharacterStatusManager.Instance.partyCharacter;
        _cursor = 0;
        _cursorMax = ids.Count;
        _uiController.HideAllCursor();  // 一度全てのキャラクターステータスウィンドウを閉じる（後から必要な箇所だけ追加する）
        while (_cursor < _cursorMax)
        {
            int id = ids[_cursor];
            var characterData = CharacterDataManager.Instance.GetCharacterData(id);
            Sprite sprite = characterData.sprite;   // キャラクタースプライトの取得
            var characterStatus = CharacterStatusManager.Instance.GetCharacterStatusById(id);
            int currentHp = characterStatus.currentHp;
            int maxHp = characterStatus.maxHp;
            int currentMp = characterStatus.currentMp;
            int maxMp = characterStatus.maxMp;
            switch (_cursor)
            {
                case 0:
                    _uiController.SetCharacterStatus1(sprite, currentHp, maxHp, currentMp, maxMp);
                    break;
                case 1:
                    _uiController.SetCharacterStatus2(sprite, currentHp, maxHp, currentMp, maxMp);
                    break;
                case 2:
                    _uiController.SetCharacterStatus3(sprite, currentHp, maxHp, currentMp, maxMp);
                    break;
            }
            _cursor++;
        }
        _cursor = 0;
    }

    public void OpenMenuUseSelect()
    {
        StartCoroutine(OpenMenuUseSelectProcess());
    }

    private IEnumerator OpenMenuUseSelectProcess()
    {
        yield return null;
    }

    private IEnumerator HideProcess()
    {
        yield return null;
        MenuManager.Instance.OnCloseSelectWindow();
        HideWindow();
    }

    public void ShowWindow()
    {
        InitializeCommand();
        SetupSelectWindow();
        _uiController.Show();

    }

    public void HideWindow()
    {
        _uiController.Hide();
        _selectPhase = 0;
    }

    //各キーへの入力
    private void PointNext()
    {
        _cursor++;
        if (_cursor >= _cursorMax)
        {
            _cursor = 0;
        }
        _uiController.ShowSelectedCursor(_cursor);
    }
    private void PointPrevious()
    {
        _cursor--;
        if (_cursor < 0)
        {
            _cursor = _cursorMax - 1;
        }
        _uiController.ShowSelectedCursor(_cursor);
    }

    private void SetPhase2Text()
    {
        string text = "使用した！";
        _uiController.InputText(text);
        _selectPhase++;
    }

    // 初期化をここで行う。
    private void InitializeCommand()
    {
        _cursor = 0;
        _selectPhase = 0;
        _uiController.ShowSelectedCursor(_cursor);
        if (MenuManager.Instance.MenuUsePhase == MenuUsePhase.SkillUse)
        {
            _uiController.InputText(_skillData.skillDesc);
        }
        else if (MenuManager.Instance.MenuUsePhase == MenuUsePhase.ItemUse)
        {
            _uiController.InputText(_itemData.itemDesc);
        }

    }

    // スキル一覧で選択されているスキルデータを手に入れるために使用
    public void SetSkillData(SkillData sd)
    {
        _skillData = sd;
    }

    // アイテム一覧で選択されているアイテムデータを手に入れるために使用
    public void SetItemData(ItemData itemdata)
    {
        _itemData = itemdata;
    }
}
