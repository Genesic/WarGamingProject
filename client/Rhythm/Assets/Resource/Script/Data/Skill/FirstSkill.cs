using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FirstSkill : SkillBase
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
    
    // 顯示被施放技能的軌跡    
    public override IEnumerator showBeCastSkill()
    {
        for( int i=0; i<pos.Count; i++)
        {
            var dp = MainManager.dataCenter.atkMgr.Obtain(TouchType.ForDefShow.ToString());
            dp.setOverTimer(1+(pos.Count-i)*0.1F);
            dp.setPosition(pos[i]);
            dp.SetEnable();
            yield return new WaitForSeconds(0.1F);
        }
    }
            
    //  被施放技能
    public override IEnumerator beCastSkill()
    {
        yield return new WaitForSeconds(2F);
        // 取得按鈕種類        
        for (int i = 0; i < pos.Count; i++)
        {
            var dp = MainManager.dataCenter.dpMgr.Obtain(TouchType.Click.ToString());
            dp.setId(i);
            dp.setOverTimer(1.1F);
            dp.setPosition(pos[i]);
            dp.SetEnable();
            yield return new WaitForSeconds(0.4F);
        }

        // 取消技能施放狀態
        MainManager.skill.stopBeCastingSKill();
    }
    
    // 施放技能
    public override IEnumerator castSkill()
    {
        int skillHash = Animator.StringToHash("PlayerSkill");
        int overHash = Animator.StringToHash("over");
        cameraAnime.SetTrigger(skillHash);
        MainManager.dataCenter.atkMgr.atkList = new Dictionary<int, AtkPointBasic>();
        for(int i=0; i< pos.Count; i++)
        {
            var dp = MainManager.dataCenter.atkMgr.Obtain(TouchType.ForAtkShow.ToString());
            dp.setOverTimer(1.5F*(i+1));
            dp.setPosition(pos[i]);
            dp.SetEnable();
            MainManager.dataCenter.atkMgr.atkList.Add(i, dp);
            yield return new WaitForSeconds(0.02F);
        }
        yield return new WaitForSeconds(1F);
        cameraAnime.SetTrigger(overHash);
    }
}
