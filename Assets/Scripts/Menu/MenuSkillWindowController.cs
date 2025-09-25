using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class MenuSkillWindowController : MonoBehaviour, IMenuWindowController
{
    [SerializeField] MenuSkillUIController _uiController;
    private bool _canClose;
    private InputSetting _inputSetting;
    private int _skillListCursor; // スキルリスト内のどの位置を指しているかを数字で表す
    private int _characterCursor;   // どのキャラクターのリストを表示しているかを数字（キャラクターId）で表す（Maxの値は、キャラクターの人数によって変動）
    private SkillData _selectedSkillData;    // _skillListCursorが示すスキルのデータ
    private List<int> _skillList; // スキルのリスト（１キャラクター）

    /// <summary>
    /// スキルの一覧を画面に表示する
    /// </summary>
    private void SetUpSkill()
    {
        _skillListCursor = 0;
        _characterCursor = 1;
        // 以下の部分はデバッグ用に変更。本来ならば、各キャラクターごとに習得しているスキルを代入する。今回は、データベースで実験。
        _skillList = new();
        foreach (var sd in SkillDataManager.Instance.GetAllData())
        {
            _skillList.Add(sd.skillId);
        }

        if (_skillList == null)
        {
            Debug.Log("[MenuSkillWindowController]skillListがnull");
            return;
        }
        _selectedSkillData = SkillDataManager.Instance.GetSkillDataById(_skillList[_skillListCursor]);
        SetText();
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
        if (MenuManager.Instance.MenuPhase != MenuPhase.Skill)
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
        else if (_inputSetting.GetBackKeyDown())
        {
            ShowNextSkill();
        }
        else if (_inputSetting.GetForwardKeyDown())
        {
            ShowPreviousSkill();
        }
        else if (_inputSetting.GetDecideInputDown())
        {
            MenuManager.Instance.OnOpenSelectWindow(MenuUsePhase.SkillUse);
        }
    }
    private IEnumerator HideProcess()
    {
        _canClose = false;
        yield return null;
        MenuManager.Instance.OnSkillCanceled();
        HideWindow();
    }

    public void ShowWindow()
    {
        _uiController.InitializeText(); // テキストの初期化
        SetUpSkill();
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
    private void ShowNextSkill()
    {
        _skillListCursor++;
        _skillListCursor = _skillListCursor % _skillList.Count;
        _selectedSkillData = SkillDataManager.Instance.GetSkillDataById(_skillList[_skillListCursor]);
        SetText();
    }

    private void ShowPreviousSkill()
    {
        _skillListCursor--;
        _skillListCursor = (_skillListCursor + _skillList.Count) % _skillList.Count;
        _selectedSkillData = SkillDataManager.Instance.GetSkillDataById(_skillList[_skillListCursor]);
        SetText();
    }

    /// <summary>
    /// 引数で与えられた値にあるスキルの名前を返す
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    private string GetSkillNameByCursor(int n)
    {
        if (_skillList == null)
        {
            Debug.Log("[MenuSkillWindowController]_skillListがnull");
            return string.Empty;
        }
        if (n < 0 || n > _skillList.Count - 1)
        {
            return "---";
        }
        SkillData sd = SkillDataManager.Instance.GetSkillDataById(_skillList[n]);
        return sd.skillName;
    }

    /// <summary>
    /// スキル画面のテキストを表示する
    /// </summary>
    private void SetText()
    {
        _uiController.SetSkill1NameText(GetSkillNameByCursor(_skillListCursor - 2));
        _uiController.SetSkill2NameText(GetSkillNameByCursor(_skillListCursor - 1));
        _uiController.SetSkill3NameText(GetSkillNameByCursor(_skillListCursor));
        _uiController.SetSkill4NameText(GetSkillNameByCursor(_skillListCursor + 1));
        _uiController.SetSkill5NameText(GetSkillNameByCursor(_skillListCursor + 2));
        _uiController.SetSkillSelectedDiscText(_selectedSkillData.skillDesc);
        _uiController.SetSkillSelectedNameText(_selectedSkillData.skillName);
        _uiController.SetSkillSelectedValueText(_selectedSkillData.cost);
    }

    // 現在選択されているスキルデータを返す
    public SkillData getSkillData()
    {
        return _selectedSkillData;
    }
}
