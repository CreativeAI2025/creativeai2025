using UnityEngine;

public class PlayerAnimeSwitch : MonoBehaviour
{
    public Animator animator;

    // TimelineのSignalから呼ばれる関数
    public void ZophyWalk_GoBack()
    {
        animator.SetTrigger("Walk_GoBackTrigger");
    }

    public void ZophyWalk_GoFront()
    {
        animator.SetTrigger("Walk_GoFrontTrigger");
    }

    public void ZophyWalk_GoRight()
    {
        animator.SetTrigger("Walk_GoRightTrigger");
    }

    public void ZophyWalk_GoLeft()
    {
        animator.SetTrigger("Walk_GoLeftTrigger");
    }
    public void ZophyBase()
    {
        animator.SetTrigger("BaseTrigger");
    }
    public void ZophyIdol_Back()
    {
        animator.SetTrigger("Idol_BackTrigger");
    }

    public void ZophyIdol_Front()
    {
        animator.SetTrigger("Idol_FrontTrigger");
    }

    public void ZophyIdol_Right()
    {
        animator.SetTrigger("Idol_RightTrigger");
    }

    public void ZophyIdol_Left()
    {
        animator.SetTrigger("Idol_LeftTrigger");
    }

    public void ZophyDown()
    {
        animator.SetTrigger("ZophyDownTrigger");
    }
    public void NoahWalk()
    {
        animator.SetTrigger("NoahWalkTrigger");
    }
    public void NoahIdolBack()
    {
        animator.SetTrigger("NoahIdolBackTrigger");
    }
    public void NoahIdolFront()
    {
        animator.SetTrigger("NoahIdolFrontTrigger");
    }
    public void LinaIdolFront()
    {
        animator.SetTrigger("LinaIdolFrontTrigger");
    }
    public void LinaIdolBack()
    {
        animator.SetTrigger("LinaIdolBackTrigger");
    }
    public void LinaWalk()
    {
        animator.SetTrigger("LinaWalkTrigger");
    }
    public void LinaDown()
    {
        animator.SetTrigger("LinaDownTrigger");
    }
}
