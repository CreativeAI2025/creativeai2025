using UnityEngine;

public interface IMenuWindowController
{
    /// <summary>
    /// コントローラの状態をセットアップする
    /// </summary>
    /// <param name="menuManager"></param>
    void SetUpController(MenuManager menuManager);

    /// <summary>
    /// ウィンドウを表示する
    /// </summary>
    void ShowWindow();

    /// <summary>
    /// ウィンドウを非表示にする
    /// </summary>
    void HideWindow();
}
