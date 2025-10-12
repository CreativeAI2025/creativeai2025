using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAnimation : MonoBehaviour
{
    public string triggerTextWalk;
    public string attackTrigger;
    public string idleBool;
    public Animator animator;


    public void ChangeToWalk()
    {
        animator.SetTrigger(triggerTextWalk);     
    }
    public void ChangeToAttack()
    {
        animator.SetTrigger(attackTrigger);
    }
    public void ChangeToIdle()
    {
        animator.SetBool(idleBool, false);
    }
}