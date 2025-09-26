using UnityEngine;
/// <summary>
/// メニュー画面ウィンドウ内のUIを制御するクラス向けのインタフェース
/// </summary>
public interface IMenuUIController
{
    /// <summary>
    /// UIを表示する
    /// </summary>
    void Show();

    /// <summary>
    /// UIを非表示にする
    /// </summary>
    void Hide();
}
