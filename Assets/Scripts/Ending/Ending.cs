using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

[System.Serializable]
public class End
{
    public int bgm_id;
    public Sprite end_picture;
}

public class Ending : MonoBehaviour
{
    PlayableDirector playableDirector;
    [SerializeField] Image endImage;
    [SerializeField] List<End> endList;
    [SerializeField] int end_num = 0;
    private bool isEnd;//　終了を検知したかどうか

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playableDirector = this.gameObject.GetComponent<PlayableDirector>();
        isEnd = false;
        EndSelect(end_num);
    }

    // Update is called once per frame
    void Update()
    {
        //　タイムラインが終了したら次のシーンを読み込む
        if (!isEnd && playableDirector.state != PlayState.Playing)
        {
            Debug.Log("Openingに戻る");
            SoundManager.Instance.StopBGM();
        }
    }

    /// <summary>
    /// Endingによって写真とBGMを変える
    /// </summary>
    /// <param name="end_num"></param>
    public void EndSelect(int end_num)
    {
        endImage.sprite = endList[end_num].end_picture;
        SoundManager.Instance.PlayBGM(endList[end_num].bgm_id, 0.5f);
    }
}
