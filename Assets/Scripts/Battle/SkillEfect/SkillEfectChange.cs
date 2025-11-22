using UnityEngine;

public class SkillEfectChange : MonoBehaviour
{
    [Header("画像番号（3桁）")] public int animationNum;
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        // PlaySkillAnimation(animationNum);
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
    /// スキルアニメーションの再生（引数：画像番号3桁）
    /// </summary>
    /// <param name="animationNum"></param>
    public void PlaySkillAnimation(int animationNum)
    {
        animator.SetInteger("SkillID", animationNum);
        animator.SetTrigger("Play");
    }
}
