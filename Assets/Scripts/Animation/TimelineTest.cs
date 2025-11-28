using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Playables;

[System.Serializable]
public class TimelineEventMapping
{
    //  イベントデータに記述する名前
    public string EventName;

    //  シーン内に配置された対応するTimelineControllerへの参照
    public TimelineController Controller;
}

public class TimelineTest : MonoBehaviour
{
    [SerializeField] private string _timelineName;
    [SerializeField] private TimelineEventMapping[] _timelineEventMappings; //  ここで全タイムラインを参照
    private Dictionary<string, TimelineController> _timelineControllerDict;
    private bool _timelineIsPlaying = false;
    private InputSetting _inputSetting;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputSetting = InputSetting.Load();
        _timelineControllerDict = new Dictionary<string, TimelineController>();
        foreach (var map in _timelineEventMappings)
        {
            _timelineControllerDict.Add(map.EventName, map.Controller);
        }
    }

    // Update is called once per frame
    async Task Update()
    {
        if (_timelineIsPlaying)
        {
            return;
        }
        if (_inputSetting.GetDecideInputDown())
        {
            await CallEvent();
        }
    }

    private async UniTask CallEvent()
    {
        Debug.Log($"タイムライン開始{_timelineName}");
        _timelineIsPlaying = true;
        PlayTimeline(_timelineName);
        await UniTask.WaitUntil(() => !_timelineIsPlaying);
        Debug.Log($"タイムライン終了{_timelineName}");
    }

    private void PlayTimeline(string timelineName)
    {
        if (!_timelineControllerDict.ContainsKey(timelineName))
        {
            Debug.Log($"Timeline Controller が Dictionary に見つかりません: {timelineName}");
            _timelineIsPlaying = false;
            return;
        }
        TimelineController controller = _timelineControllerDict[timelineName];
        // Notifier は Controller の GameObject に付いている前提
        TimelineFinishedNotifier notifier = controller.GetComponent<TimelineFinishedNotifier>();

        if (notifier == null)
        {
            Debug.Log("タイムラインに必要なコンポーネントがありません。");
            _timelineIsPlaying = false;
            return;
        }

        if (!controller.PlayableIsSet())
        {
            Debug.Log("タイムラインの Playable Asset が設定されていません。");
            _timelineIsPlaying = false;
            return;
        }

        //  TimelineControllerがアタッチされているGameObhectをアクティブにする
        controller.gameObject.SetActive(true);

        notifier.OnTimelineEnd += () => OnTimelineFinished(notifier);
        controller.StartTimeline();
    }

    private void OnTimelineFinished(TimelineFinishedNotifier notifier)
    {
        _timelineIsPlaying = false;

        if (notifier != null)
        {
            // 登録時と同じ形のラムダ式を渡すことで解除
            notifier.OnTimelineEnd -= () => OnTimelineFinished(notifier);

            //  NotifierがアタッチされているGameObject（＝TimelineControllerと同じGameObject）を非アクティブにする
            notifier.gameObject.SetActive(false);
        }
    }
}
