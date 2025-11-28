/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(target.position.x, target.position.y, -10);
    }
}*/
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // プレイヤー
    [SerializeField] private Vector2 minPosition; // マップ左下の制限座標
    [SerializeField] private Vector2 maxPosition; // マップ右上の制限座標
    [SerializeField] private float smoothSpeed = 0.1f; // スムーズ追従の速さ

    private float halfHeight;
    private float halfWidth;

    void Start()
    {
        // カメラサイズ（OrthographicSize）から画面の半分の高さ・幅を計算
        Camera cam = GetComponent<Camera>();
        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;
    }

    void LateUpdate()
    {
        // ターゲットの位置を追跡
        Vector3 targetPos = new Vector3(target.position.x, target.position.y, -10);

        // カメラの中心がマップ外に出ないように制限
        float clampX = Mathf.Clamp(targetPos.x, minPosition.x + halfWidth, maxPosition.x - halfWidth);
        float clampY = Mathf.Clamp(targetPos.y, minPosition.y + halfHeight, maxPosition.y - halfHeight);

        // スムーズに追従
        Vector3 smoothPos = Vector3.Lerp(transform.position, new Vector3(clampX, clampY, targetPos.z), smoothSpeed);

        transform.position = smoothPos;
    }
}


