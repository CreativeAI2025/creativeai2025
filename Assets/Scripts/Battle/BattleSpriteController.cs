using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

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
    /// 透明画像
    /// </summary>
    [SerializeField] private Sprite voidSprite;

    /// <summary>
    /// 敵キャラクターの表示用Spriteです。
    /// </summary>
    [SerializeField] private Image[] enemySprites;

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
    /*
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
    */

    /// <summary>
    /// 敵キャラクターを表示します。
    /// </summary>
    /// <param name="enemyId">敵キャラクターのID</param>
    public void ShowEnemy(List<int> enemyIds)
    {
        const int EncountMax = 5;
        for (int i = 0; i < EncountMax; i++)
        {
            Sprite enemySprite = voidSprite;
            if (i < enemyIds.Count)
            {
                // 適切な画像を入れる
                int enemyId = enemyIds[i];
                var enemyData = EnemyDataManager.Instance.GetEnemyDataById(enemyId);
                if (enemyData == null)
                {
                    Logger.Instance.LogWarning($"敵キャラクターの画像が取得できませんでした。 ID: {enemyId}");
                }
                else
                {
                    enemySprite = enemyData.sprite;
                }
                enemySprites[i].sprite = enemySprite;
                enemySprites[i].gameObject.SetActive(true);
            }
            else
            {
                // 透明の画像を入れる
                enemySprites[i].sprite = enemySprite;
                enemySprites[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 敵キャラクターを非表示にします。
    /// </summary>
    public void HideEnemy()
    {
        foreach (var image in enemySprites)
        {
            image.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 💡 追記: 敵の生存状況に基づいて、アクティブな敵スプライトを更新します。
    /// </summary>
    public void RefreshActiveEnemies()
    {
        const int EncountMax = 5;
        var activeEnemies = EnemyStatusManager.Instance.GetEnemyStatusList()
            .Where(status => !status.isDefeated && !status.isRunaway)
            .ToList();

        // 敵のスロット全体をクリアし、生存している敵のみ再表示
        for (int i = 0; i < EncountMax; i++)
        {
            if (i < activeEnemies.Count)
            {
                // 生存している敵を、配置スロット i に表示する（ここではインデックス順に表示すると仮定）
                var enemyStatus = activeEnemies[i];
                enemySprites[i].sprite = enemyStatus.enemyData.sprite;
                enemySprites[i].gameObject.SetActive(true);
            }
            else
            {
                // 生存していないスロットは非表示
                enemySprites[i].gameObject.SetActive(false);
            }
        }
    }
}