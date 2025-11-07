using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject player; // 既に配置済みのプレイヤーをInspectorで指定

    void Start()
    {
        string spawnPointName = PlayerPrefs.GetString("LastSpawnPointName", "");
        GameObject spawnPoint = GameObject.Find(spawnPointName);

        if (spawnPoint != null)
        {
            player.transform.position = spawnPoint.transform.position;
        }
        else
        {
            Debug.LogWarning($"出現地点 '{spawnPointName}' が見つかりません。");
        }
    }
}
