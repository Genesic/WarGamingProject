using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Skill : MonoBehaviour
{
    public DefendPointGroup dpGroup;
    public DefendPointManager dpMgr;
    public Text skillName;
    public Text rivalSkillName;
    public Animator skillTextAnime;
    public Animator rivalSkillTextAnime;
    int actHash = Animator.StringToHash("act");

    public void testSkill(int id)
    {
        startSkill(id);
    }

    Dictionary<int, string> sName = new Dictionary<int, string>(){
        {1, "九頭龍閃"},
        {2, "降龍十八掌"},
    };

    public void showSkillText(int skillId)
    {
        if (skillId == 1)
        {
            skillName.text = sName[skillId];
            skillTextAnime.SetTrigger(actHash);
        }
    }

    public void startSkill(int skillId)
    {
        if (sName.ContainsKey(skillId))
        {
            rivalSkillName.text = sName[skillId];
            rivalSkillTextAnime.SetTrigger(actHash);
        }

        if (skillId == 1)
            StartCoroutine(skill_1());
        else if (skillId == 2)
            StartCoroutine(skill_2());
    }

    IEnumerator skill_1()
    {
        List<Vector2> pos = new List<Vector2>(){
            new Vector2(0,200),
            new Vector2(100,100),
            new Vector2(200,0),
            new Vector2(100,-100),
            new Vector2(0,-200),
            new Vector2(-100,-100),
            new Vector2(-200,0),
            new Vector2(-100,100),
            new Vector2(0,0),
        };
        // 取得按鈕種類
        var dpData = dpGroup.list[TouchType.Click];
        for (int i = 0; i < pos.Count; i++)
        {
            var dp = dpMgr.Obtain(dpData.type.ToString());
            dp.setOverTimer(2);
            dp.setPosition(pos[i]);
            dp.SetEnable();
            yield return new WaitForSeconds(0.4F);
        }
    }

    IEnumerator skill_2()
    {
        List<Vector2> pos = new List<Vector2>(){
            new Vector2(400,200),
            new Vector2(-400,200),
            new Vector2(300,100),
            new Vector2(-300,100),
            new Vector2(400,0),
            new Vector2(-400,0),
            new Vector2(300,-100),
            new Vector2(-300,-100),
            new Vector2(400,-200),
            new Vector2(-400,-200),
            new Vector2(0,200),
            new Vector2(0,-200),
            new Vector2(200,0),
            new Vector2(-200,0),                                    
            new Vector2(200,0),
            new Vector2(-2000,0),                                                
        };
        // 取得按鈕種類
        var dpData = dpGroup.list[TouchType.Click];
        for(int i=0 ; i <pos.Count ; i+=2)        
        {
            var dp1 = dpMgr.Obtain(dpData.type.ToString());
            var dp2 = dpMgr.Obtain(dpData.type.ToString());
            dp1.setOverTimer(1.5f);
            dp1.setPosition(pos[i]);
            dp1.SetEnable();
            dp2.setOverTimer(1.5f);
            dp2.setPosition(pos[i+1]);
            dp2.SetEnable();
            yield return new WaitForSeconds(0.4F);            
        }
    }
}
