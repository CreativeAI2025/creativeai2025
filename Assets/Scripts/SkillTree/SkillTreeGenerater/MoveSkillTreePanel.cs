using UnityEngine;
using UnityEngine.EventSystems;

public class MoveSkillTreePanel : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector3 previousMousePos;
    private Vector3 startPos;

    [SerializeField, Range(0.01f, 1f)]
    private float sensitivity = 0.5f; // マウス移動の感度

    // 移動範囲をローカル座標で指定（親RectTransform内）
    [SerializeField]
    private Vector2 minPosition = new Vector2(-200, -200); // 左下
    [SerializeField]
    private Vector2 maxPosition = new Vector2(200, 200);   // 右上

    // スケール制御用
    [SerializeField] private float scaleSpeed = 0.01f; // スクロール感度
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 2f;

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

            Vector3 newPos = rectTransform.localPosition + new Vector3(-mouseDelta.y, mouseDelta.x, 0) * sensitivity / rectTransform.lossyScale.x;

            // 範囲制限
            newPos.x = Mathf.Clamp(newPos.x, minPosition.x, maxPosition.x);
            newPos.y = Mathf.Clamp(newPos.y, minPosition.y, maxPosition.y);

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
