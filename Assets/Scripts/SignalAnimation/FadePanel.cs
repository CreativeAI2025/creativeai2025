using UnityEngine;

public class FadePanel : MonoBehaviour
{
    public Animator animator;

    // TimelineのSignalから呼ばれる関数
    public void FadeIn()
    {
        animator.SetTrigger("FadeInTrigger");
    }

    public void FadeOut()
    {
        animator.SetTrigger("FadeOutTrigger");
    }
    public void FadeInFast()
    {
        animator.SetTrigger("FadeInFastTrigger");
    }
    public void FadeOutFast()
    {
        animator.SetTrigger("FadeOutFastTrigger");
    }
}
