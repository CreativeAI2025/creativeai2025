using UnityEngine;
using UnityEngine.SceneManagement;
using System;

// アニメーション再生を司ります
public class AnimationManager : DontDestroySingleton<AnimationManager>
{
    // アニメーションリストのゲームオブジェクトを渡す
    private AnimationList _currentAnimationList;
    private const string ANIMATION_LIST_NAME = "AnimationList";
    public event Action OnAnimationStart { add => _onAnimationStart += value; remove => _onAnimationStart -= value; }
    private Action _onAnimationStart;
    public event Action OnAnimationEnd { add => _onAnimationEnd += value; remove => _onAnimationEnd -= value; }
    private Action _onAnimationEnd;

    /// <summary>
    /// アニメーションリストがあるゲームオブジェクトを取得する
    /// </summary>
    private void SetCurrentAnimationList()
    {
        _currentAnimationList = GameObject.Find(ANIMATION_LIST_NAME).GetComponent<AnimationList>();
    }
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("[AnimationManager]シーン名「" + scene.name + "」が読み込まれたので、アニメーションリストを更新します。");
        SetCurrentAnimationList();
    }

    /// <summary>
    /// アニメーションを、アニメーション名から開始する
    /// </summary>
    /// <param name="animationName"></param>
    public void InitializeFromString(string animationName)
    {
        _onAnimationStart?.Invoke();
        if (_currentAnimationList == null)
        {
            Debug.Log("[AnimationManager]animationListがnullです。");
            _onAnimationEnd?.Invoke();
            return;
        }
        TimelineController controller = _currentAnimationList.GetTimelineController(animationName);
        if (controller == null)
        {
            Debug.Log("[AnimationManager]受け取ったAnimationControllerがnullです。");
            _onAnimationEnd?.Invoke();
            return;
        }
        TimelineFinishedNotifier notifier = controller.gameObject.GetComponent<TimelineFinishedNotifier>();
        if (notifier == null)
        {
            Debug.Log("[AnimationManager]受け取ったAnimationControllerのゲームオブジェクトに、TimelineFinishedNotifierがアタッチされていません。");
            _onAnimationEnd?.Invoke();
            return;
        }
        // アニメーションが登録されているゲームオブジェクトを表示させる
        controller.gameObject.SetActive(true);
        // アニメーション終了時に、OnTimelineFinished()を呼び出すように設定する
        notifier.OnTimelineEnd += () => OnTimelineFinished(notifier);
        // アニメーションを開始させる
        controller.StartTimeline();
    }

    /// <summary>
    /// アニメーションが終わったときに呼ばれる関数
    /// </summary>
    /// <param name="notifier"></param>
    private void OnTimelineFinished(TimelineFinishedNotifier notifier)
    {
        if (notifier != null)
        {
            // 登録時と同じ形のラムダ式を渡すことで解除
            notifier.OnTimelineEnd -= () => OnTimelineFinished(notifier);

            //  NotifierがアタッチされているGameObject（＝TimelineControllerと同じGameObject）を非アクティブにする
            notifier.gameObject.SetActive(false);
            EndAnimation();
        }
    }

    /// <summary>
    /// アニメーションを終了する
    /// </summary>
    private void EndAnimation()
    {
        _onAnimationEnd?.Invoke();
    }
}
