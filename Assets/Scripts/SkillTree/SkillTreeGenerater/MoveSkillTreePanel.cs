using UnityEngine;

public class MoveSkillTreePanel : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector3 previousMousePos;
    private Vector3 startPos;

    [SerializeField, Range(0.01f, 1f)]
    private float sensitivity = 0.5f; // マウス移動の感度

    // スケール制御用
    [SerializeField] private float scaleSpeed = 0.01f; // スクロール感度
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 2f;

    // 基準となる移動範囲（scale=1.0 のときの範囲）
    [SerializeField]
    private Vector2 baseMinPosition = new Vector2(-200, -200);
    [SerializeField]
    private Vector2 baseMaxPosition = new Vector2(200, 200);

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.localPosition;
    }

    void Update()
    {
        // ------------------
        // ドラッグ移動
        // ------------------
        if (Input.GetMouseButtonDown(0))
        {
            previousMousePos = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 mouseDelta = Input.mousePosition - previousMousePos;

            Vector3 newPos = rectTransform.localPosition +
                             new Vector3(-mouseDelta.y, mouseDelta.x, 0) *
                             sensitivity / rectTransform.lossyScale.x;

            // 現在のスケールを取得
            float scaleFactor = rectTransform.localScale.x;

            // スケールに応じて移動範囲を拡大/縮小
            Vector2 minPos = baseMinPosition * scaleFactor;
            Vector2 maxPos = baseMaxPosition * scaleFactor;

            newPos.x = Mathf.Clamp(newPos.x, minPos.x, maxPos.x);
            newPos.y = Mathf.Clamp(newPos.y, minPos.y, maxPos.y);

            rectTransform.localPosition = newPos;
            previousMousePos = Input.mousePosition;
        }

        // ------------------
        // スクロールでサイズ変更
        // ------------------
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0f)
        {
            Vector3 newScale = rectTransform.localScale + Vector3.one * scroll * scaleSpeed;

            newScale.x = Mathf.Clamp(newScale.x, minScale, maxScale);
            newScale.y = Mathf.Clamp(newScale.y, minScale, maxScale);
            newScale.z = 1f; // 2D用に固定

            rectTransform.localScale = newScale;
        }

        // ------------------
        // 元位置リセット
        // ------------------
        if (Input.GetKeyDown(KeyCode.F1))
        {
            rectTransform.localPosition = startPos;
            rectTransform.localScale = Vector3.one;
        }
    }
}
