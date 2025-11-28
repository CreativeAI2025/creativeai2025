using UnityEngine;

public class SkillEfectChange :  MonoBehaviour
{
    [Header("画像番号（前から3桁）")] int animationNum = 0;
    private Animator animator;
    private BattleManager _battleManager;
        public void SetReferences(BattleManager battleManager)
    {
        _battleManager = battleManager;
    }

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
         if (animator == null)
    {
        Debug.LogError("【エラー】Animator がアタッチされていません！", this);
        return;
    }
        animator.SetInteger("SkillID", animationNum);
        animator.SetTrigger("Play");
        Logger.Instance.Log("アニメーション再生");
    }
}
