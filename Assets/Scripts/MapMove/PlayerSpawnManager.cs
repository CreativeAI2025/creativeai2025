using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    void Start()
    {
        // 保存されている出現地点名を取得
        string spawnName = PlayerPrefs.GetString("LastSpawnPointName", "");

        if (string.IsNullOrEmpty(spawnName))
        {
            Debug.LogWarning("Spawn point name not found. Using default position.");
            return;
        }

        // 該当するSpawnPointオブジェクトを探す
        GameObject spawnPoint = GameObject.Find(spawnName);

        if (spawnPoint != null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                player.transform.position = spawnPoint.transform.position;
                Debug.Log($"Player spawned at: {spawnPoint.name}");
            }
            else
            {
                Debug.LogWarning("Player object not found in scene.");
            }
        }
        else
        {
            Debug.LogWarning($"Spawn point '{spawnName}' not found in scene.");
        }
    }
}
