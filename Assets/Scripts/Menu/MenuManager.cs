using UnityEngine;
using System.Collections;
/// <summary>
/// メニュー画面全体を管理するクラス
/// </summary>
public class MenuManager : MonoBehaviour
{
    /// <summary>
    /// キャラクターの移動を行うクラスを管理するクラスへの参照
    /// </summary>
    //[SerializeField] CharacterMoverManager _characterMoverManager;

    /// <summary>
    /// メニューのトップ画面を制御するクラスへの参照
    /// </summary>
    [SerializeField] TopMenuWindowController _topMenuWindowController;

    /// <summary>
    /// メニュー画面のステータスウィンドウを制御するクラスへの参照
    /// </summary>
    [SerializeField] MenuCharacterWindowController _menuCharacterWindowController;

    /// <summary>
    /// メニューのフェーズ
    /// </summary>
    public MenuPhase MenuPhase { get; private set; }

    /// <summary>
    /// 選択されたメニュー
    /// </summary>
    public MenuCommand SelectedMenu { get; private set; }

    private InputSetting _inputSetting;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputSetting = InputSetting.Load();
    }

    // Update is called once per frame
    void Update()
    {
        CheckOpenMenuKey();
    }

    void CheckOpenMenuKey()
    {
        /*
        // 移動中以外の場合は、メニューを開けないようにする
        if (GameStateManager.CurrentState != GameState.Moving)
        {
            return;
        }
        */

        // メニューが閉じている場合のみ、メニューを開くキーの入力を確認する
        if (MenuPhase != MenuPhase.Closed)
        {
            return;
        }

        // メニューキーが押された時、メニューを開く
        if (_inputSetting.GetMenuKeyDown())
        {
            OpenMenu();
        }
    }

    /// <summary>
    /// メニュー画面の表示
    /// </summary>
    void OpenMenu()
    {
        StartCoroutine(OpenMenuProcess());
    }

    IEnumerator OpenMenuProcess()
    {
        /// <sumarry>
        /// １フレームだけ遅延させる（その際、OpenMenu関数内にあるコルーチンを用いる）
        /// </summary>
        yield return null;

        MenuPhase = MenuPhase.Top;
        _topMenuWindowController.SetUpController(this);
        _topMenuWindowController.InitializeCommand();
        _topMenuWindowController.ShowWindow();

        //_characterMoverManager.StopCharacterMover();
    }

    /// <summary>
    /// メニュー項目が選択された時のコールバック
    /// </summary>
    /// <param name="menuCOmmand"></param>
    public void OnSelectedMenu(MenuCommand menuCommand)
    {
        SelectedMenu = menuCommand;
        HandleMenu();
    }

    /// <summary>
    /// 選択されたメニューに応じた処理を呼び出す
    /// </summary>
    void HandleMenu()
    {
        switch (SelectedMenu)
        {
            case MenuCommand.Character:
                // キャラ詳細を開く処理
                ShowCharacterMenu();
                break;
            case MenuCommand.Skill:
                // スキルを開く処理
                break;
            case MenuCommand.Item:
                // アイテムを開く処理
                break;
            case MenuCommand.SkillBoard:
                // スキルボードを開く処理
                break;
        }
    }

    void ShowCharacterMenu()
    {
        MenuPhase = MenuPhase.Character;
        _menuCharacterWindowController.SetUpController(this);
        _menuCharacterWindowController.ShowWindow();
    }

    public void OnCharacterCanceled()
    {
        MenuPhase = MenuPhase.Top;
    }

    /// <summary>
    /// メニュー画面が閉じるときのコールバック
    /// </summary>
    public void OnCloseMenu()
    {
        MenuPhase = MenuPhase.Closed;
        //_characterMoverManager.ResumeCharacterMover();
    }
}
