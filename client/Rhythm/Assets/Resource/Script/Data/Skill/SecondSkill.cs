using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SecondSkill : SkillBase {
    public override IEnumerator castSkill()
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
        for(int i=0 ; i <pos.Count ; i+=2)        
        {
            // 第一個點
            var dp1 = MainManager.dataCenter.dpMgr.Obtain(TouchType.Click.ToString());            
            dp1.setOverTimer(1.5f);
            dp1.setPosition(pos[i]);
            dp1.SetEnable();
            
            // 第二個點
            var dp2 = MainManager.dataCenter.dpMgr.Obtain(TouchType.Click.ToString());
            dp2.setOverTimer(1.5f);
            dp2.setPosition(pos[i+1]);
            dp2.SetEnable();
            yield return new WaitForSeconds(0.4F);            
        }
        
        // 取消技能施放狀態
        MainManager.skill.stopCastingSKill();
    }
}
