using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class MenuSelectWindowController : MonoBehaviour, IMenuWindowController
{
    [SerializeField] private MenuSelectUIController _uiController;
    private InputSetting _inputSetting;
    private int _selectPhase;
    private int _cursor;
    private SkillData _skillData;

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
        int member = 3;
        _cursor++;
        _cursor = _cursor % member;  // ここはパーティメンバーの数によって変える必要あり
        _uiController.ShowSelectedCursor(_cursor);
    }
    private void PointPrevious()
    {
        int member = 3;
        _cursor--;
        if (_cursor < 0)
        {
            _cursor = member - 1;
        }
        _uiController.ShowSelectedCursor(_cursor);
    }

    private void SetPhase2Text()
    {
        string text = "You Used Skill !!";
        _uiController.InputText(text);
        _selectPhase++;
    }

    // 初期化をここで行う。
    private void InitializeCommand()
    {
        _cursor = 0;
        _selectPhase = 0;
        _uiController.ShowSelectedCursor(_cursor);
        _uiController.InputText(_skillData.skillDesc);
    }

    // スキル一覧で選択されているスキルデータを手に入れるために使用
    public void SetSkillData(SkillData sd)
    {
        _skillData = sd;
    }
}
