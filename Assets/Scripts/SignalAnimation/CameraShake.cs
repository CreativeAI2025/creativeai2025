using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPos;

    void Awake()
    {
        originalPos = transform.localPosition;
    }

    // Timelineから呼び出される（引数なし）
    public void Shake()
    {
        // デフォルト値で揺らす
        StartCoroutine(ShakeRoutine(0.3f, 0.2f));
    }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;
    }

    // ← ここに初期化メソッドを追加
    public void ResetPosition()
    {
       originalPos = transform.localPosition;
    }

    // もしTimelineで任意のTransformにリセットしたい場合
    public void ResetPositionTo(Vector3 newPos)
    {
        originalPos = newPos;
        transform.localPosition = originalPos;
    }
}
