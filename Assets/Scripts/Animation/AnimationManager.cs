using UnityEngine;
using System;

// アニメーション再生を司ります
public class AnimationManager : DontDestroySingleton<AnimationManager>
{
    private const string ANIMATION_LIST_NAME = "AnimationList";
    public event Action OnAnimationStart { add => _onAnimationStart += value; remove => _onAnimationStart -= value; }
    private Action _onAnimationStart;
    public event Action OnAnimationEnd { add => _onAnimationEnd += value; remove => _onAnimationEnd -= value; }
    private Action _onAnimationEnd;
    private bool isPlaying = false;
    private InputSetting _inputSetting;
    [SerializeField] AnimationWindowController _windowController;

    void Start()
    {
        _inputSetting = InputSetting.Load();
        _windowController.HideWindow();
    }

    void Update()
    {
        if (!isPlaying)
        {
            return;
        }
        if (_inputSetting.GetDecideInputDown())
        {
            if (!_windowController.gameObject.activeSelf)
            {
                _windowController.ShowWindow();
            }
        }
    }

    /// <summary>
    /// アニメーションを、アニメーション名から開始する
    /// </summary>
    /// <param name="animationName"></param>
    public void InitializeFromString(string animationName)
    {
        _onAnimationStart?.Invoke();
        Debug.Log($"[AnimationManager]アニメーション名「{animationName}」を再生します。");
        GameObject obj = (GameObject)Resources.Load(string.Join("/", "Animation", animationName));
        if (obj == null)
        {
            Debug.Log($"[AnimationManager]「{animationName}が存在しません。");
            _onAnimationEnd?.Invoke();
            return;
        }
        GameObject instance = Instantiate(obj);
        TimelineController controller = instance.GetComponent<TimelineController>();
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
        obj.SetActive(true);
        // アニメーション終了時に、OnTimelineFinished()を呼び出すように設定する
        notifier.OnTimelineEnd += () => OnTimelineFinished(notifier);
        // アニメーションを開始させる
        controller.StartTimeline();
        _windowController.OnTimelineEnd += () => OnTimelineFinished(notifier);
        isPlaying = true;
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
            _windowController.OnTimelineEnd -= () => OnTimelineFinished(notifier);

            //  NotifierがアタッチされているGameObject（＝TimelineControllerと同じGameObject）を非アクティブにする
            notifier.gameObject.SetActive(false);
            Destroy(notifier.gameObject);
            _windowController.HideWindow();
            EndAnimation();
        }
    }

    /// <summary>
    /// アニメーションを終了する
    /// </summary>
    private void EndAnimation()
    {
        _onAnimationEnd?.Invoke();
        isPlaying = false;
    }
}
