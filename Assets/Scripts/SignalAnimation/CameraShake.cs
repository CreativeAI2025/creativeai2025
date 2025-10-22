using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPos;

    [Header("Shake Settings")]
    [Tooltip("1回の位置変動の待機時間（秒）")]
    public float shakeDuration = 0.1f;

    [Tooltip("揺れの回数（往復動作を1回と数えます）")]
    public int shakeCount = 3;

    [Tooltip("X方向の揺れ幅（正方向と負方向を往復）")]
    public float shakeAmplitudeX = 0.3f;

    [Tooltip("Y方向の揺れ幅（オプション、未使用なら0でもOK）")]
    public float shakeAmplitudeY = 0f;

    [Tooltip("最初の揺れ方向（1で右、-1で左）")]
    public int shakeDirection = 1;

    private bool isShaking = false;

    void Awake()
    {
        originalPos = transform.localPosition;
    }

    /// <summary>
    /// Timelineやスクリプトなどから呼び出す
    /// </summary>
    public void Shake()
    {
        if (!isShaking)
            StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        isShaking = true;
        int direction = shakeDirection;

        for (int i = 0; i < shakeCount; i++)
        {
            // 1. 正方向に移動
            Vector3 offset = new Vector3(shakeAmplitudeX * direction, 0f, 0f);
            transform.localPosition = originalPos + offset;

            // 2. 指定時間待機
            yield return new WaitForSeconds(shakeDuration);

            // 3. 元の位置に戻す
            transform.localPosition = originalPos;

            // 4. 指定時間待機
            yield return new WaitForSeconds(shakeDuration);

            // 5. 逆方向に移動
            transform.localPosition = originalPos - offset;

            // 6. 指定時間待機
            yield return new WaitForSeconds(shakeDuration);

            // 7. 元の位置に戻す
            transform.localPosition = originalPos;

            // 最後にもう一度待機しても自然
            yield return new WaitForSeconds(shakeDuration);

            // 方向を反転して次へ
            direction *= -1;
        }

        // 最終的にカメラ位置をリセット
        transform.localPosition = originalPos;
        isShaking = false;
    }

    /// <summary>
    /// 現在位置を新しい基準点として記録
    /// </summary>
    public void ResetPosition()
    {
        originalPos = transform.localPosition;
    }

    /// <summary>
    /// 明示的に新しい位置を設定して記録
    /// </summary>
    public void ResetPositionTo(Vector3 newPos)
    {
        originalPos = newPos;
        transform.localPosition = originalPos;
    }
}
