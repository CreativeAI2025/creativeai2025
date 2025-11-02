using UnityEngine;

public class PlayerMoveDevelop : MonoBehaviour
{
    [SerializeField] private EnemyEncountManager enemyEncountManager;
    void Start()
    {
        Debug.Log("[PlayerMoveDevelop]これはデバッグ用クラスです。\n実際には、プレイヤーの動きを検知する箇所に変数や関数を定義してください。\n操作方法：\nOキーで１歩カウントします。");
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.O))
        {
            enemyEncountManager.IncreaseEncountProbability();
        }
    }


}
