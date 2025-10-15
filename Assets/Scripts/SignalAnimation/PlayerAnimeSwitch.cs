using UnityEngine;

public class PlayerAnimeSwitch : MonoBehaviour
{
    public Animator animator;

    // TimelineのSignalから呼ばれる関数
    public void PlayWalk_GoBack()
    {
        animator.SetTrigger("Walk_GoBackTrigger");
    }

    public void PlayWalk_GoFront()
    {
        animator.SetTrigger("Walk_GoFrontTrigger");
    }

    public void PlayWalk_GoRight()
    {
        animator.SetTrigger("Walk_GoRightTrigger");
    }

    public void PlayWalk_GoLeft()
    {
        animator.SetTrigger("Walk_GoLeftTrigger");
    }
    public void PlayBase()
    {
        animator.SetTrigger("BaseTrigger");
    }
}
