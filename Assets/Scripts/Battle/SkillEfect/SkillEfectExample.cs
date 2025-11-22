using UnityEngine;

public class SkillEfectExample : MonoBehaviour
{
    [SerializeField] SkillEfectChange[] skillEfectChange;
    [Header("画像番号（3桁）")] public int[] animationNum;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            skillEfectChange[0].PlaySkillAnimation(animationNum[0]);
            skillEfectChange[1].PlaySkillAnimation(animationNum[1]);
            skillEfectChange[2].PlaySkillAnimation(animationNum[2]);
        }
    }
}
