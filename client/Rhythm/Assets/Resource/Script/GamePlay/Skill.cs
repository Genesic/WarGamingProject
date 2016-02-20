using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill : MonoBehaviour
{
    public DefendPointGroup dpGroup;
    public DefendPointManager dpMgr;
    
    public void testSkill(int id)
    {
        startSkill(id);
    }

    public void startSkill(int skillId)
    {
        if (skillId == 1)
        {
            StartCoroutine(skill_1());
        }
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
        var dpData = dpGroup.list[TouchType.Click];        
        for(int i=0 ; i<9 ; i++){            
            var dp = dpMgr.Obtain(dpData.type.ToString());
            dp.setOverTimer(2);
            dp.setPosition(pos[i]);
            dp.SetEnable();            
            yield return new WaitForSeconds(0.4F);
        }
    }
}
