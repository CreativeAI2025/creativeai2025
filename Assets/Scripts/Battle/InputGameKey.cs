using UnityEngine;

public class InputGameKey
{
    /// <summary>
    /// 決定ボタンが押されたかどうかを取得します。
    /// </summary>
    public static bool ConfirmButton()
    {
        return Input.GetKeyDown(KeyCode.Return)
            || Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(KeyCode.Z);
    }

    /// <summary>
    /// キャンセルボタンが押されたかどうかを取得します。
    /// </summary>
    public static bool CancelButton()
    {
        return Input.GetKeyDown(KeyCode.Escape)
            || Input.GetKeyDown(KeyCode.X);
    }
}
