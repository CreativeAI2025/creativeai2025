using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    //  シーン内でアタッチされたPlayableDirectorを参照
    [SerializeField] private PlayableDirector _playableDirector;
    //  同じゲームオブジェクトにアタッチされた終了通知クラスを参照
    [SerializeField] private TimelineFinishedNotifier _notifier;

    /// <summary>
    /// タイムラインの再生を開始する
    /// </summary>
    /// <param name="timelineAsset">再生するTimelineアセット</param>
    public void StartTimeline(PlayableAsset timelineAsset)
    {
        if (_playableDirector == null)
        {
            Debug.Log("[TimelineController] PlayableDirectorが設定されていません。");
            return;
        }
        if (_notifier == null)
        {
            Debug.Log("[TimelineController] Notifierが設定されていません。");
            return;
        }

        //  Timelineアセットを設定し、再生する
        _playableDirector.playableAsset = timelineAsset;
        _playableDirector.Play();

        //  Notifierに現在のDirectorを渡し、監視を開始させる
        _notifier.SetupForTimeline(_playableDirector);
    }

    /// <summary>
    /// タイムラインを強制的に停止する
    /// </summary>
    public void StopTimeLine()
    {
        if (_playableDirector != null)
        {
            _playableDirector.Stop();
        }
    }
}
