using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AnimationList : MonoBehaviour
{
    // 特定のシーンで流したいアニメーションをリストに登録してください。
    [SerializeField] private List<TimelineController> animationList;
    private Dictionary<string, TimelineController> animationDict;

    void Start()
    {
        SetDictionary();
    }

    /// <summary>
    /// 計算量を抑えるために、Dictionary型にする。
    /// </summary>
    private void SetDictionary()
    {
        if (animationList == null)
        {
            Debug.Log("このシーンにはアニメーションが設定されていません。");
            return;
        }
        if (animationList.Count == 0)
        {
            Debug.Log("このシーンにはアニメーションが設定されていません。");
            return;
        }
        foreach (var animation in animationList)
        {
            animationDict = animationList.ToDictionary(x => x.gameObject.name, x => x);
        }
    }

    /// <summary>
    /// 引数で与えられたアニメーション名を返す
    /// 引数で与えられたアニメーションがない/Dictionaryがセットされていない場合は、nullを返す
    /// </summary>
    /// <param name="animationname"></param>
    /// <returns></returns>
    public TimelineController GetTimelineController(string animationname)
    {
        if (animationDict == null)
        {
            Debug.Log("[AnimationList]animationDictがセットされていません。");
            return null;
        }
        if (animationDict.ContainsKey(animationname))
        {
            return animationDict[animationname];
        }
        else
        {
            Debug.Log("[AnimationList]指定されたアニメーション名が入っていません。");
            return null;
        }
    }
}
