using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
/// <summary>
/// メニュー画面全体を管理するクラス
/// </summary>
public class MenuManager : DontDestroySingleton<MenuManager>
{
    [SerializeField] private Pause menuUIPause;
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
    [SerializeField] private MenuSkillTreeWindowController _menuSkillTreeWindowController;
    // マップ上のプレイヤーの動きを制限させるクラスへの参照
    private Pause playerPause;

    /// <summary>
    /// メニューのフェーズ
    /// </summary>
    public MenuPhase MenuPhase { get; private set; }

    /// <summary>
    /// 選択されたメニュー
    /// </summary>
    public MenuCommand SelectedMenu { get; private set; }
    public MenuUsePhase MenuUsePhase { get; private set; }

    // メニューを開いていいシーンかどうかを判別する（タイトルとエンディング以外は開ける）
    private bool isOpenMenu = true;
    private string[] notOpenMenuScenes = new string[] { "Title", "Ending" };

    private InputSetting _inputSetting;


    public override void Awake()
    {
        base.Awake(); // DontDestroySingletonのAwakeロジックを実行

        // メニューマネージャー固有の初期化処理をここに記述
        Debug.Log("[MenuManager] Specific initialization for IMenuManager completed.");
        HideMenuWindowAll();
    }
    void Start()
    {
        _inputSetting = InputSetting.Load();

        // 会話中にメニューを開けなくする
        ConversationTextManager.Instance.OnConversationStart += menuUIPause.PauseAll;
        ConversationTextManager.Instance.OnConversationEnd += menuUIPause.UnPauseAll;

        // 戦闘中にメニューを開けなくする
        BattleManager.Instance.OnBattleStart += menuUIPause.PauseAll;
        BattleManager.Instance.OnBattleEnd += menuUIPause.UnPauseAll;

        // アニメーション中にメニューを開けなくする
        AnimationManager.Instance.OnAnimationStart += menuUIPause.PauseAll;
        AnimationManager.Instance.OnAnimationEnd += menuUIPause.UnPauseAll;

        SceneManager.sceneLoaded += SceneLoaded;
        SetIsMenuOpen(SceneManager.GetActiveScene().name);

        //playerPause = GameObject.Find("Pause").GetComponent<Pause>();

    }

    // Update is called once per frame
    void Update()
    {
        CheckOpenMenuKey();
    }

    private void CheckOpenMenuKey()
    {
        // メニューが閉じている場合のみ、メニューを開くキーの入力を確認する
        if (MenuPhase != MenuPhase.Closed)
        {
            return;
        }

        if (!isOpenMenu)
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

        if (playerPause != null)
        {
            playerPause.PauseAll(); // マップ上のプレイヤーの動きを止める
        }
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
                ShowSkillTreeMenu();
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
        MenuPhase = MenuPhase.SkillTree;
        _menuSkillTreeWindowController.ShowWindow();
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
        if (playerPause != null)
        {
            playerPause.UnPauseAll();   // マップ上のキャラクターを動かせるようにする
        }
    }

    // アイテム/スキル選択ウィンドウを表示させる。引数にはSkillUseかItemUseの値を入れる
    public void OnOpenSelectWindow(MenuUsePhase m, int userId)
    {
        MenuUsePhase = m;
        if (m == MenuUsePhase.SkillUse)
        {
            _menuSelectWindowController.SetSkillData(_menuSkillWindowController.getSkillData(), userId);
        }
        else if (m == MenuUsePhase.ItemUse)
        {
            _menuSelectWindowController.SetItemData(_menuItemWindowController.getItemData());
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

    /// <summary>
    /// メニューの全てのウィンドウを閉じる
    /// </summary>
    private void HideMenuWindowAll()
    {
        _topMenuWindowController.HideWindow();
        _menuCharacterWindowController.HideWindow();
        _menuSkillWindowController.HideWindow();
        _menuItemWindowController.HideWindow();
        _menuSelectWindowController.HideWindow();
        _menuSkillTreeWindowController.HideWindow();
    }

    /// <summary>
    /// シーンの切り替え時にこの関数を呼び、メニュー画面が開けるシーンかどうかを判別する
    /// </summary>
    private void SetIsMenuOpen(string sceneName)
    {
        isOpenMenu = true;
        foreach (string scene in notOpenMenuScenes)
        {
            if (sceneName.Equals(scene))
            {
                isOpenMenu = false;
                return;
            }
        }
    }

    private void SceneLoaded(Scene nextScene, LoadSceneMode mode)
    {
        SetIsMenuOpen(nextScene.name);
        playerPause = GameObject.Find("Pause").GetComponent<Pause>();
    }
}