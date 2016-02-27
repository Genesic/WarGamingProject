using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FirstSkill : SkillBase {
    public override IEnumerator castSkill()
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
        for (int i = 0; i < pos.Count; i++)
        {
            var dp = MainManager.dataCenter.dpMgr.Obtain(TouchType.Click.ToString());
            dp.setOverTimer(2);
            dp.setPosition(pos[i]);
            dp.SetEnable();
            yield return new WaitForSeconds(0.4F);
        }
        
        // 取消技能施放狀態
        MainManager.skill.stopCastingSKill();                
    }
}
