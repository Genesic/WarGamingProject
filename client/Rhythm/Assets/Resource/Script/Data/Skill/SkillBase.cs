using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillBase : MonoBehaviour {
    public int id;
    public string sName;
    public int mp;
    protected Animator cameraAnime;
    public void setCameraAnime(Animator anime){
        cameraAnime = anime;
    }
    
    // 顯示被施放技能的軌跡
    public virtual IEnumerator showBeCastSkill()
    {
        yield return new WaitForSeconds(0.1F);
    }
    // 實際被施放技能
    public virtual IEnumerator beCastSkill()
    {
        yield return new WaitForSeconds(0.1F);        
    }
    
    public Dictionary<int, AtkPointBasic> atkList;
    // 顯示施放技能
    public virtual IEnumerator castSkill()
    {
        yield return new WaitForSeconds(0.1F);
    }
    
    public AtkPointBasic getAtkPoint(int id)
    {
        return atkList[id];
    }       
}
