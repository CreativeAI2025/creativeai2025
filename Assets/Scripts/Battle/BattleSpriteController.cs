using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public class BattleBackgroundMapData
{
    public string sceneName;
    public Sprite sprite;
}

/// <summary>
/// 戦闘関連のスプライトを制御するクラスです。
/// </summary>
public class BattleSpriteController : MonoBehaviour
{
    /// <summary>
    /// 背景の表示用Spriteです。
    /// </summary>
    [SerializeField]
    Image _backgroundRenderer;

    [SerializeField] private BattleBackgroundMapData[] battleBackgroundMapDataList;
    private Dictionary<string, int> _battleBackgroundMapDataDict = new Dictionary<string, int>();

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

    void Awake()
    {
        for (int i = 0; i < battleBackgroundMapDataList.Length; i++)
        {
            _battleBackgroundMapDataDict.Add(battleBackgroundMapDataList[i].sceneName, i);
        }
    }

    /// <summary>
    /// 背景を表示します。
    /// </summary>
    public void ShowBackground()
    {
        // シーンに合わせた戦闘背景を設定する
        string sceneName = SceneManager.GetActiveScene().name;
        if (!_battleBackgroundMapDataDict.ContainsKey(sceneName))
        {
            Debug.Log($"[BattleSpriteController]戦闘背景としてシーン「{sceneName}が配置されていません。");
            return;
        }
        _backgroundRenderer.sprite = battleBackgroundMapDataList[_battleBackgroundMapDataDict[sceneName]].sprite;
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