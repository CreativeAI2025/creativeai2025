using System;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineFinishedNotifier : MonoBehaviour
{
    // 外部（ObjectEngine）に追加するためのイベント
    public event Action OnTimelineEnd;
    private PlayableDirector _currentDirector;

    /// <summary>
    /// 監視対象のPlayableDirectorを設定し、監視を開始する
    /// </summary>
    /// <param name="director"></param> <summary>
    /// 再生中のPlayableDirector
    /// </summary>
    /// <param name="director"></param>
    public void SetupForTimeline(PlayableDirector director)
    {
        _currentDirector = director;
    }

    void Update()
    {
        //  監視対象があり、かつ、Directorの状態がPaused（＝通常は再生終了）になったら
        if (_currentDirector != null && _currentDirector.state == PlayState.Paused)
        {
            //  タイムラインのリセット
            _currentDirector = null;

            //  終了イベントを発火し、ObjectEngineに通知
            OnTimelineEnd?.Invoke();
        }
    }
}
