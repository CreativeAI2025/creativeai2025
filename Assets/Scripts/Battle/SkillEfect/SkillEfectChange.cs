using UnityEngine;

public class SkillEfectChange : MonoBehaviour
{
    [Header("画像番号（前から3桁）")] int animationNum = 0;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     PlaySkillAnimation(animationNum);
        // }
    }

    /// <summary>
    /// スキルアニメーションの再生（引数：画像番号前から3桁）
    /// </summary>
    /// <param name="animationNum"></param>
    public void PlaySkillAnimation(int animationNum)
    {
        this.animationNum = animationNum;
        animator.SetInteger("SkillID", animationNum);
        animator.SetTrigger("Play");
    }
}
