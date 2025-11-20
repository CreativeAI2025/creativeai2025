using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class MenuSkillWindowController : MonoBehaviour, IMenuWindowController
{
    [SerializeField] MenuSkillUIController _uiController;
    [SerializeField] private MenuHeaderMiniUIController _headerUIController;
    private bool _canClose;
    private InputSetting _inputSetting;
    private int _skillListCursor; // スキルリスト内のどの位置を指しているかを数字で表す
    private SkillData _selectedSkillData;    // _skillListCursorが示すスキルのデータ
    private List<int> _skillList; // スキルのリスト（１キャラクター）
    private int _characterIndex;
    private int _characterIndexMax;
    private bool stop;

    /// <summary>
    /// スキルの一覧を画面に表示する
    /// </summary>
    private void SetUpSkill()
    {
        stop = false;
        _skillListCursor = 0;
        SetSkillList(); // スキルリストをセットする
        if (_skillList == null)
        {
            stop = true;
            Debug.Log("[MenuSkillWindowController]skillListがnull");
            return;
        }
        if (_skillList.Count == 0)
        {
            stop = true;
            Debug.Log("[MenuSkillWindowController]skillListの要素数が０");
        }
        else
        {
            _selectedSkillData = SkillDataManager.Instance.GetSkillDataById(_skillList[_skillListCursor]);
            SetText();
        }

        // ヘッダーの設定
        _headerUIController.Initialize();
        _headerUIController.SetHeight(1);
        List<int> characterIds = CharacterStatusManager.Instance.partyCharacter;
        int count = characterIds.Count;
        for (int i = 0; i < 3; i++)
        {
            string text = string.Empty;
            if (i == 0)
            {
                if (i < count)
                {
                    int id = characterIds[i];
                    var characterdata = CharacterDataManager.Instance.GetCharacterData(id);
                    text = characterdata.characterName;
                }
                _headerUIController.SetHeaderObject1(text);
            }
            else if (i == 1)
            {
                if (i < count)
                {
                    int id = characterIds[i];
                    var characterdata = CharacterDataManager.Instance.GetCharacterData(id);
                    text = characterdata.characterName;
                }
                _headerUIController.SetHeaderObject2(text);
            }
            else if (i == 2)
            {
                if (i < count)
                {
                    int id = characterIds[i];
                    var characterdata = CharacterDataManager.Instance.GetCharacterData(id);
                    text = characterdata.characterName;
                }
                _headerUIController.SetHeaderObject3(text);
            }
        }
        _characterIndex = 0;
        _characterIndexMax = characterIds.Count;    // パーティメンバーが二人なら「２」を返すよ
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
            if (stop)
            {
                return;
            }
            ShowNextSkill();
        }
        else if (_inputSetting.GetForwardKeyDown())
        {
            if (stop)
            {
                return;
            }
            ShowPreviousSkill();
        }
        else if (_inputSetting.GetDecideInputDown())
        {
            if (stop)
            {
                return;
            }
            int userId = CharacterStatusManager.Instance.partyCharacter[_characterIndex];
            MenuManager.Instance.OnOpenSelectWindow(MenuUsePhase.SkillUse, userId);
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
        MenuManager.Instance.OnSkillCanceled();
        SoundManager.Instance.PlaySE(3);
        HideWindow();
    }

    public void ShowWindow()
    {
        _uiController.InitializeText(); // テキストの初期化
        _headerUIController.Initialize();
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
        SoundManager.Instance.PlaySE(1);
    }

    private void ShowPreviousSkill()
    {
        _skillListCursor--;
        _skillListCursor = (_skillListCursor + _skillList.Count) % _skillList.Count;
        _selectedSkillData = SkillDataManager.Instance.GetSkillDataById(_skillList[_skillListCursor]);
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

        SetSkillList();
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

        SetSkillList();
        InitializePage();
        SoundManager.Instance.PlaySE(1);
    }

    /// <summary>
    /// スキルリストのカーソルを一番上にするなどの、
    /// ページの切り替えを行った際に行われる初期化
    /// </summary>
    private void InitializePage()
    {
        if (stop)
        {
            return;
        }
        _skillListCursor = 0;
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

    /// <summary>
    /// 現在の_characterIndexを元に、キャラクターのIDを取得し、
    /// そのキャラクターのステータスを取得し、
    /// 覚えているスキルIDのリストを、_skillListに渡す
    /// </summary>
    private void SetSkillList()
    {
        int id = CharacterStatusManager.Instance.partyCharacter[_characterIndex];
        CharacterStatus currentCharacterStatus = CharacterStatusManager.Instance.GetCharacterStatusById(id);
        _skillList = currentCharacterStatus.skillList;
        if (_skillList.Count == 0)
        {
            stop = true;
        }
    }
}
