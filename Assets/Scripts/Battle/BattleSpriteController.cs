using UnityEngine;


/// <summary>
/// 戦闘関連のスプライトを制御するクラスです。
/// </summary>
public class BattleSpriteController : MonoBehaviour
{
    /// <summary>
    /// 背景の表示用Spriteです。
    /// </summary>
    [SerializeField]
    SpriteRenderer _backgroundRenderer;

    /// <summary>
    /// 敵キャラクターの表示用Spriteです。
    /// </summary>
    [SerializeField]
    SpriteRenderer _enenyRenderer;

    /// <summary>
    /// カメラへの参照です。
    /// </summary>
    Camera _mainCamera;

    /// <summary>
    /// 背景を表示します。
    /// </summary>
    public void ShowBackground()
    {
        _backgroundRenderer.gameObject.SetActive(true);
    }

    /// <summary>
    /// 背景を非表示にします。
    /// </summary>
    public void HideBackground()
    {
        _backgroundRenderer.gameObject.SetActive(false);
    }

    /// <summary>
    /// 背景と敵キャラクターの位置をカメラに合わせて設定します。
    /// </summary>
    public void SetSpritePosition()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }

        var cameraPos = _mainCamera.transform.position;
        var newPosition = new Vector3(cameraPos.x, cameraPos.y, 0);

        var backgroundPosOffset = new Vector3(0, 0, 0);
        _backgroundRenderer.transform.position = newPosition + backgroundPosOffset;

        var enemyPosOffset = new Vector3(0, -0.5f, 0);
        _backgroundRenderer.transform.position = newPosition + enemyPosOffset;
    }

    /// <summary>
    /// 敵キャラクターを表示します。
    /// </summary>
    /// <param name="enemyId">敵キャラクターのID</param>
    public void ShowEnemy(int enemyId)
    {
        Sprite enemySprite = null;
        var enemyData = EnemyDataManager.Instance.GetEnemyDataById(enemyId);
        if (enemyData == null)
        {
            Logger.Instance.LogWarning($"敵キャラクターの画像が取得できませんでした。 ID: {enemyId}");
        }
        else
        {
            enemySprite = enemyData.sprite;
        }
        _enenyRenderer.sprite = enemySprite;
        _enenyRenderer.gameObject.SetActive(true);
    }

    /// <summary>
    /// 敵キャラクターを非表示にします。
    /// </summary>
    public void HideEnemy()
    {
        _enenyRenderer.gameObject.SetActive(false);
    }
}