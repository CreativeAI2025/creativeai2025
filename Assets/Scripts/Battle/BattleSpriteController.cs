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
    GameObject _backgroundRenderer;

    /// <summary>
    /// 透明画像
    /// </summary>
    [SerializeField] private Sprite voidSprite;

    /// <summary>
    /// 敵キャラクターの表示用Spriteです。
    /// </summary>
    [SerializeField] private Image[] enemySprites;
    /// <summary>
    /// 敵キャラクターの表示用Spriteです。
    /// </summary>
    [SerializeField] private List<SkillEfectChange> enemyEffectSprites = new List<SkillEfectChange>();

    /// <summary>
    /// カメラへの参照です。
    /// </summary>
    Camera _mainCamera;

    /// <summary>
    /// 背景を表示します。
    /// </summary>
    public void ShowBackground()
    {
        _backgroundRenderer.SetActive(true);
    }

    /// <summary>
    /// 背景を非表示にします。
    /// </summary>
    public void HideBackground()
    {
        _backgroundRenderer.SetActive(false);
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
            if (i < enemyIds.Count)
            {
                // 適切な画像を入れる
                int enemyId = enemyIds[i];
                var enemyData = EnemyDataManager.Instance.GetEnemyDataById(enemyId);
                enemySprites[i].sprite = enemyData.sprite;
                enemySprites[i].gameObject.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                enemySprites[i].gameObject.transform.parent.gameObject.SetActive(false);
            }
        }
    }
    /// <summary>
    /// 敵へのスキルエフェクトを表示します。
    /// </summary>
    /// <param name="enemyId">敵キャラクターのID</param>
    public void PlayEffectAtEnemy(int targetIndex, int animationNum)
    {
        enemyEffectSprites[targetIndex].PlaySkillAnimation(animationNum);
        //  enemyEffectSprites[i].sprite = effectSprite;
        // enemySprites[i].gameObject.SetActive(true);

        //     if (targetIndex < 0 || targetIndex >= enemySprites.Length) return;

        //     // 敵スプライトと同じ座標にエフェクトを生成
        //     Vector3 position = enemySprites[targetIndex].transform.position;

        //     Sprite effect = Instantiate(effectSprite, position, Quaternion.identity, this.transform);

        //     // 任意: 1秒後削除
        //     Destroy(effect, 1f);
    }

    /// <summary>
    /// 敵キャラクターを非表示にします。
    /// </summary>
    public void HideEnemy()
    {
        foreach (var image in enemySprites)
        {
            image.gameObject.transform.parent.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 💡 追記: 敵の生存状況に基づいて、アクティブな敵スプライトを更新します。
    /// </summary>
    public void RefreshActiveEnemies()
    {
        var enemies = EnemyStatusManager.Instance.GetEnemyStatusList();
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].isDefeated || enemies[i].isRunaway)
            {
                enemySprites[i].sprite = voidSprite;
            }
        }
    }
}