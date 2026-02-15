using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillEfectExample : MonoBehaviour
{
    [SerializeField] List<SkillEfectChange> skillEfectChange = new List<SkillEfectChange>();
    [SerializeField] GameObject efectUI;
    [Header("画像番号（3桁）")] public int[] animationNum;
    bool newCreate;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        newCreate = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            // skillEfectChange[0].PlaySkillAnimation(animationNum[0]);
            // skillEfectChange[1].PlaySkillAnimation(animationNum[1]);
            // skillEfectChange[2].PlaySkillAnimation(animationNum[2]);
            //EfectView();
        }
    }

    public void EfectView()
    {
        string s = null;
        if (newCreate)
        {
            for (int i = 0; i < animationNum.Length; i++)
            {
                var obj = Instantiate(efectUI, this.transform);
                var effect = obj.GetComponent<SkillEfectChange>();

                skillEfectChange.Add(effect);
                if (i < animationNum.Length - 1)
                {
                    s += $"{animationNum[i]},";
                }
                else
                {
                    s += $"{animationNum[i]}";
                }
            }
            Debug.Log(s + "を再生しました");
            newCreate = false;
        }

        for (int i = 0; i < animationNum.Length; i++)
        {
            skillEfectChange[i].PlaySkillAnimation(animationNum[i]);
        }
    }

}
