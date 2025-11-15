using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class MenuSkillTreeWindowController : MonoBehaviour, IMenuWindowController
{
    MenuManager _menuManager;
    [SerializeField] private GameObject _uiController;
    [SerializeField] private List<GameObject> _skillTreeController;
    [SerializeField] private MenuHeaderMiniUIController _headerUIController;
    private int _characterIndex;
    private int _characterIndexMax;
    private InputSetting _inputSetting;
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
        if (MenuManager.Instance.MenuPhase != MenuPhase.SkillTree)
        {
            return;
        }
        if (_inputSetting.GetCancelKeyDown())
        {
            StartCoroutine(HideProcess());
        }
        else if (_inputSetting.GetRightKeyDown())
        {
            StartCoroutine(NextPage());
        }
        else if (_inputSetting.GetLeftKeyDown())
        {
            StartCoroutine(previousPage());
        }
    }

    private IEnumerator HideProcess()
    {
        yield return null;
        MenuManager.Instance.OnSkillTreeCanceled();
        HideWindow();
    }
    public void ShowWindow()
    {
        _uiController.SetActive(true);

        SetupSkillTree();

        StartCoroutine(SetCloseStateDelay());
    }

    public void SkillSetActive(int currentID)
    {
        switch (currentID)
        {
            case 1:
                _skillTreeController[0].SetActive(true);
                _skillTreeController[1].SetActive(false);
                _skillTreeController[2].SetActive(false);
                break;
            case 3:
                _skillTreeController[0].SetActive(false);
                _skillTreeController[1].SetActive(true);
                _skillTreeController[2].SetActive(false);
                break;
            case 2:
                _skillTreeController[0].SetActive(false);
                _skillTreeController[1].SetActive(false);
                _skillTreeController[2].SetActive(true);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// ヘッダーの初期化などを行う
    /// </summary>
    private void SetupSkillTree()
    {
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
        // ここに_characterIndexの添え字に応じたスキルツリーを表示する
        int currentID = CharacterStatusManager.Instance.partyCharacter[_characterIndex];

        SkillSetActive(currentID);

    }

    private IEnumerator previousPage()
    {
        _characterIndex--;
        if (_characterIndex < 0)
        {
            _characterIndex = _characterIndexMax - 1;
        }
        yield return null;

        _headerUIController.SetSameHeight();
        _headerUIController.SetHeight(_characterIndex);
        // ここに_characterIndexの添え字に応じたスキルツリーを表示する
        int currentID = CharacterStatusManager.Instance.partyCharacter[_characterIndex];

        SkillSetActive(currentID);
    }

    private IEnumerator SetCloseStateDelay()
    {
        yield return null;
    }

    public void HideWindow()
    {
        _uiController.SetActive(false);
    }
}
