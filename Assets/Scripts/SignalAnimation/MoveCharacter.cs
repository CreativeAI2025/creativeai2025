using UnityEngine;
using System.Collections;

public class MoveCharacter : MonoBehaviour
{
    // ===========================
    // 【体の揺れ設定】
    // ===========================
    [Header("Shake Settings (体の揺れ設定)")]

    [Tooltip("キャラクターが揺れている時間（秒）")]
    public float shakeDuration = 0.3f;

    [Tooltip("揺れの強さ（数値が大きいほど激しく揺れる）")]
    public float shakeAmount = 0.1f;

    [Tooltip("時間経過による揺れの減衰速度（大きいほど早く止まる）")]
    public float decreaseFactor = 1.5f;



    // ===========================
    // 【ジャンプ設定】
    // ===========================
    [Header("Jump Settings (ジャンプ設定)")]

    [Tooltip("ジャンプの高さ（Y軸方向）")]
    public float jumpHeight = 0.5f;

    [Tooltip("上昇から下降までの合計時間（秒）")]
    public float jumpDuration = 0.3f;



    // ===========================
    // 【点滅設定】
    // ===========================
    [Header("Blink Settings (点滅設定)")]

    [Tooltip("1回の点滅サイクルにかかる時間（秒）")]
    public float blinkInterval = 0.1f;

    [Tooltip("点滅を繰り返す回数")]
    public int blinkCount = 5;



    private Vector3 originalPos;
    private Renderer charRenderer; // キャラクターのマテリアル取得用
    private Coroutine blinkCoroutine; // 同時再生防止

    private void OnEnable()
    {
        originalPos = transform.localPosition;
        charRenderer = GetComponentInChildren<Renderer>();
    }

    // ===========================
    // --- 体を揺らす ---
    // ===========================
    public void Shake()
    {
        StopAllCoroutines();
        StartCoroutine(DoShake());
    }

    private IEnumerator DoShake()
    {
        float duration = shakeDuration;
        while (duration > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
            duration -= Time.deltaTime * decreaseFactor;
            yield return null;
        }
        transform.localPosition = originalPos;
    }

    // ===========================
    // --- ぴょんと跳ねる ---
    // ===========================
    public void JumpOnce()
    {
        StopAllCoroutines();
        StartCoroutine(DoJump());
    }

    private IEnumerator DoJump()
    {
        float halfTime = jumpDuration / 2f;
        float elapsed = 0f;

        // 上昇
        while (elapsed < halfTime)
        {
            float t = elapsed / halfTime;
            transform.localPosition = originalPos + Vector3.up * Mathf.Lerp(0, jumpHeight, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 下降
        elapsed = 0f;
        while (elapsed < halfTime)
        {
            float t = elapsed / halfTime;
            transform.localPosition = originalPos + Vector3.up * Mathf.Lerp(jumpHeight, 0, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }

    // ===========================
    // --- キャラクターを点滅させる ---
    // ===========================
    public void Blink()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
        blinkCoroutine = StartCoroutine(DoBlink());
    }

    private IEnumerator DoBlink()
    {
        if (charRenderer == null)
            yield break;

        for (int i = 0; i < blinkCount; i++)
        {
            charRenderer.enabled = false; // 非表示
            yield return new WaitForSeconds(blinkInterval);
            charRenderer.enabled = true;  // 表示
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}
