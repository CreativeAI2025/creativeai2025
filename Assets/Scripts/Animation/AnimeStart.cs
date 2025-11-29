using UnityEngine;
using UnityEngine.Playables;

public class TimelineAutoPlay : MonoBehaviour
{
    private PlayableDirector director;

    void Start()
    {
        director = GetComponent<PlayableDirector>();
        if (director != null)
        {
            // シーンがロードされた瞬間に再生開始
            director.Play();
        }
        else
        {
            Debug.LogWarning("PlayableDirectorが見つかりません。");
        }
    }
}
