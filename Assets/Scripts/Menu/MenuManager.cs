using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
/// <summary>
/// メニュー画面全体を管理するクラス
/// </summary>
public class MenuManager : DontDestroySingleton<MenuManager>
{
    /// <summary>
    /// キャラクターの移動を行うクラスを管理するクラスへの参照
    /// </summary>
    //[SerializeField] CharacterMoverManager _characterMoverManager;

    /// <summary>
    /// メニューのトップ画面を制御するクラスへの参照
    /// </summary>
    [SerializeField] private TopMenuWindowController _topMenuWindowController;

    /// <summary>
    /// メニュー画面のステータスウィンドウを制御するクラスへの参照
    /// </summary>
    [SerializeField] private MenuCharacterWindowController _menuCharacterWindowController;

    [SerializeField] private MenuSkillWindowController _menuSkillWindowController;
    [SerializeField] private MenuItemWindowController _menuItemWindowController;
    [SerializeField] private MenuSelectWindowController _menuSelectWindowController;

    /// <summary>
    /// メニューのフェーズ
    /// </summary>
    public MenuPhase MenuPhase { get; private set; }

    /// <summary>
    /// 選択されたメニュー
    /// </summary>
    public MenuCommand SelectedMenu { get; private set; }
    public MenuUsePhase MenuUsePhase { get; private set; }

    private InputSetting _inputSetting;


    public override void Awake()
    {
        base.Awake(); // DontDestroySingletonのAwakeロジックを実行

        // メニューマネージャー固有の初期化処理をここに記述
        Debug.Log("[MenuManager] Specific initialization for IMenuManager completed.");
    }
    void Start()
    {
        _inputSetting = InputSetting.Load();
    }

    // Update is called once per frame
    void Update()
    {
        CheckOpenMenuKey();
    }

    private void CheckOpenMenuKey()
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
        // ここは将来いらないだろう
        if (_inputSetting.GetMenuKeyDown())
        {
            OpenMenu();
        }
    }

    /// <summary>
    /// メニュー画面の表示
    /// </summary>
    public void OpenMenu()
    {
        StartCoroutine(OpenMenuProcess());
    }

    private IEnumerator OpenMenuProcess()
    {
        /// <sumarry>
        /// １フレームだけ遅延させる（その際、OpenMenu関数内にあるコルーチンを用いる）
        /// </summary>
        yield return null;

        MenuPhase = MenuPhase.Top;
        MenuUsePhase = MenuUsePhase.Closed;
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
                ShowSkillMenu();
                break;
            case MenuCommand.Item:
                // アイテムを開く処理
                ShowItemMenu();
                break;
            case MenuCommand.SkillTree:
                // スキルツリーを開く処理
                break;
        }
    }

    private void ShowCharacterMenu()
    {
        MenuPhase = MenuPhase.Character;
        _menuCharacterWindowController.ShowWindow();
    }

    private void ShowSkillMenu()
    {
        MenuPhase = MenuPhase.Skill;
        _menuSkillWindowController.ShowWindow();
    }

    private void ShowItemMenu()
    {
        MenuPhase = MenuPhase.Item;
        _menuItemWindowController.ShowWindow();
    }

    private void ShowSkillTreeMenu()
    {
        //上と似たような感じに、スキルツリーを開くコードを記入
    }

    public void OnCharacterCanceled()
    {
        MenuPhase = MenuPhase.Top;
    }

    public void OnSkillCanceled()
    {
        MenuPhase = MenuPhase.Top;
    }

    public void OnItemCanceled()
    {
        MenuPhase = MenuPhase.Top;
    }

    public void OnSkillTreeCanceled()
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

    // アイテム/スキル選択ウィンドウを表示させる。引数にはSkillUseかItemUseの値を入れる
    public void OnOpenSelectWindow(MenuUsePhase m)
    {
        MenuUsePhase = m;
        if (m == MenuUsePhase.SkillUse)
        {
            _menuSelectWindowController.SetSkillData(_menuSkillWindowController.getSkillData());
        }
        else if (m == MenuUsePhase.ItemUse)
        {
            //
        }
        _menuSelectWindowController.ShowWindow();
    }
    /// <summary>
    /// 選択ウィンドウを閉じる
    /// </summary>
    public void OnCloseSelectWindow()
    {
        MenuUsePhase = MenuUsePhase.Closed;
        _menuSelectWindowController.HideWindow();
    }
}
