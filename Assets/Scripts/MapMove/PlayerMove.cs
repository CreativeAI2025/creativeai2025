using UnityEngine.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]//これがないとOnCollisionEnter2Dが動作しません。

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private MapDataController mapDataController;
    public float moveSpeed = 3f;//移動速度
    public float allowDistance = 0.03f;//目的地に到達したとみなす距離
    private bool canInput = true;//trueなら入力を受け付ける
    private Vector2Int targetPosition = Vector2Int.zero;//目的地のグリッド座標
    private Vector2Int startPosition = Vector2Int.zero;//現在のグリッド座標
    private Transform _playerTransform;
    private InputSetting _inputSetting;//入力設定を管理するクラス
    public Vector2Int LastInputVector { get; private set; }//最後に入力された方向ベクトル

    public Vector2Int Direction { get; private set; }//現在の移動方向ベクトル

    private void Start()
    {
        OnStart();
    }

    protected virtual void OnStart()
    {
        _inputSetting = InputSetting.Load();//入力設定をロード
        _playerTransform = transform;
    }

    void Update()
    {
        if (canInput)
        {
            LastInputVector = GetInputVector();//入力された方向ベクトルを取得
            startPosition = GetGridPosition();//現在のグリッド座標を取得
            if (LastInputVector != Vector2Int.zero)
            {
                Direction = LastInputVector;//現在の移動方向ベクトルを更新
                bool isInRange = !mapDataController.IsGridPositionOutOfRange(startPosition + LastInputVector);//入力された方向ベクトルが範囲内かどうかをチェック
                Vector2Int convertedNextPosition = mapDataController.ConvertGridPosition(startPosition + LastInputVector);//入力された方向ベクトルをグリッド座標に変換
                Vector3Int nextPosition = new Vector3Int(convertedNextPosition.x, convertedNextPosition.y,0);//次の位置を計算
                canInput = isInRange && !mapDataController.IsWalkable(nextPosition);//次の位置が歩行可能かどうかをチェック
            }
            targetPosition = mapDataController.ConvertGridPosition(startPosition + LastInputVector);//グリッド座標 → 実座標への変換

        }
        else
        {
            Move();
            MoveEnd();
        }

    }

    Vector2Int GetInputVector()
    {
        Vector2Int vector = Vector2Int.zero;
        if (_inputSetting.GetForwardKey())
        {
            vector += Vector2Int.up;
        }
        if (_inputSetting.GetLeftKey())
        {
            vector += Vector2Int.left;
        }
        if (_inputSetting.GetBackKey())
        {
            vector += Vector2Int.down;
        }
        if (_inputSetting.GetRightKey())
        {
            vector += Vector2Int.right;
        }
        Vector2Int result = vector.x * vector.x + vector.y * vector.y != 1 ?
            Vector2Int.zero : vector;
        return result;
    }

    protected virtual void Move()
    {
        Vector3 targetPosition = _playerTransform.position + new Vector3(LastInputVector.x, LastInputVector.y, 0);
        _playerTransform.position = Vector3.MoveTowards(_playerTransform.position, targetPosition, Time.deltaTime * moveSpeed);
    }

    void MoveEnd()
    {
        Vector3 targetVector = new Vector3(targetPosition.x, targetPosition.y, 0);
        if (Vector3.Distance(targetVector, _playerTransform.position) >= allowDistance) return;

        //SoundManager.Instance.PlaySE(27, 1f);
        _playerTransform.position = targetVector;
        MovePrepare();
    }

    protected virtual void MovePrepare()
    {
        canInput = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        ResetPosition();
    }

    protected void ResetPosition()
    {
        _playerTransform.position = new Vector3(startPosition.x, startPosition.y, 0);
        MovePrepare();
    }

    public Vector2Int GetGridPosition()
    {
        return mapDataController.ConvertGridPosition(new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)));
    }
}
