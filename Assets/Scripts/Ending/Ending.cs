using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEditor;
using UnityEngine.SceneManagement;

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
    [Header("王道：0,主人公死亡：1,恋愛：2,全滅：3,敵の勘違い：4,スチルなし：5"), SerializeField] int end_num = 0;
    private bool isEnd;//　終了を検知したかどうか
    InputSetting inputSetting;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputSetting = InputSetting.Load();
        playableDirector = this.gameObject.GetComponent<PlayableDirector>();
        playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0.5f);
        isEnd = false;
        EndingFlag();
        EndSelect(end_num);
        SkillpointManager.Instance.SaveSkillpoint();
        PlayerPrefs.SetInt("Ending", 1);
        Time.timeScale = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        //Timelineをスキップ
        if (inputSetting.GetDecideInput())
        {
            try
            {
                playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(5.0f);
            }
            catch (System.NullReferenceException e)
            {
                return;
            }
        }

        //　タイムライン(エンドロール)が終了したらTitleシーンを読み込む
        if (!isEnd && playableDirector.state != PlayState.Playing)
        {
            Time.timeScale = 1.0f;
            //Debug.Log("Openingに戻る");
            SoundManager.Instance.StopBGM();
            //Titleシーンを呼び出す
            SceneManager.LoadScene("Title");
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

    /// <summary>
    /// フラグによるエンディングスチルの変更
    /// </summary>
    void EndingFlag()
    {
        if (FlagManager.Instance.HasFlag("rina_friend") && FlagManager.Instance.HasFlag("noa_friend"))
        {
            if (!FlagManager.Instance.HasFlag("zofy_human"))
            {
                if (FlagManager.Instance.HasFlag("ZebusBattleFinished"))
                {
                    end_num = 0;
                }
                else if (!FlagManager.Instance.HasFlag("ZebusBattleFinished"))
                {
                    end_num = 1;
                }
            }
            else if (FlagManager.Instance.HasFlag("zofy_human"))
            {
                if (!FlagManager.Instance.HasFlag("RinaNoaBattleFinished"))
                {
                    end_num = 3;
                }
                else if (FlagManager.Instance.HasFlag("RinaNoaBattleFinished"))
                {
                    end_num = 4;
                }
            }
        }
        else if (FlagManager.Instance.HasFlag("rina_friend"))
        {
            if (FlagManager.Instance.HasFlag("zofy_human"))
            {
                end_num = 2;
            }
            else if (!FlagManager.Instance.HasFlag("zofy_human"))
            {
                if (!FlagManager.Instance.HasFlag("PochariusBattleFinished"))
                {
                    end_num = 3;
                }
                else if (FlagManager.Instance.HasFlag("PochariusBattleFinished"))
                {
                    end_num = 4;
                }
            }
        }
        else if (FlagManager.Instance.HasFlag("noa_friend"))
        {
            if (!FlagManager.Instance.HasFlag("zofy_human"))
            {
                if (!FlagManager.Instance.HasFlag("RinaNoaBattleFinished"))
                {
                    end_num = 0;
                }
                else if (FlagManager.Instance.HasFlag("RinaNoaBattleFinished"))
                {
                    end_num = 1;
                }
            }
            else if (FlagManager.Instance.HasFlag("zofy_human"))
            {
                if (!FlagManager.Instance.HasFlag("nusiItem"))
                {
                    end_num = 2;
                }
                else if (!FlagManager.Instance.HasFlag("LastNusiBattleFinished"))
                {
                    end_num = 3;
                }
                else if (FlagManager.Instance.HasFlag("LastNusiBattleFinished"))
                {
                    end_num = 4;
                }
            }
        }
        else
        {
            if (FlagManager.Instance.HasFlag("zofy_human"))
            {
                if (FlagManager.Instance.HasFlag("KentBattleFinished"))
                {
                    end_num = 0;
                }
                else if (!FlagManager.Instance.HasFlag("KentBattleFinished"))
                {
                    end_num = 1;
                }
            }
            else if (!FlagManager.Instance.HasFlag("zofy_human"))
            {
                if (FlagManager.Instance.HasFlag("PochariusBattleFinished"))
                {
                    end_num = 3;
                }
            }
        }
    }
}
