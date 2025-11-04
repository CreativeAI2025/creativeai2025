using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;

public class MenuCharacterWindowController : MonoBehaviour, IMenuWindowController
{
    [SerializeField] private MenuCharacterUIController _uiController;
    [SerializeField] private MenuHeaderMiniUIController _headerUIController;
    /// <summary>
    /// ステータス画面を閉じられるかどうかのフラグ
    /// </summary>
    bool _canClose;
    private bool stop = false;
    private int _characterIndex;
    private int _characterIndexMax;
    private InputSetting _inputSetting;

    /// <summary>
    /// キャラクターのステータスを画面に表示する
    /// </summary>
    private void SetUpCharacter()
    {
        if (CharacterStatusManager.Instance == null)
        {
            Debug.Log("[Menu]CharacterStatusManager が初期化されていません。");
            stop = true;
            return;
        }
        stop = false;

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
        if (stop)
            return;
        CheckKeyInput();
    }

    void CheckKeyInput()
    {
        if (MenuManager.Instance == null)
        {
            return;
        }
        if (MenuManager.Instance.MenuPhase != MenuPhase.Character)
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
            StartCoroutine(NextPage());
        }
        else if (_inputSetting.GetLeftKeyDown())
        {
            StartCoroutine(previousPage());
        }
    }
    private IEnumerator HideProcess()
    {
        _canClose = false;
        yield return null;
        MenuManager.Instance.OnCharacterCanceled();
        HideWindow();
    }

    public void ShowWindow()
    {
        // _uiController.InitializeText(); // テキストの初期化
        SetUpCharacter();
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
    }
}
