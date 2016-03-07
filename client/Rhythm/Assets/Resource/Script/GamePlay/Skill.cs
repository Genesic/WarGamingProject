using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
public class Skill : MonoBehaviour
{
    public const int DEF_SUCCESS = 1;
    public Text skillName;
    public Text rivalSkillName;
    public Animator skillTextAnime;
    public Animator rivalSkillTextAnime;
    public SkillClick[] skillButton;
    private bool beCastingSkill; // 是否正在被施放技能中
    private bool castingSkill; // 是否正在施放技能中
    private bool rivalSkill; // 檢查skill_queue的下一個技能是不是敵方技能
    int actHash = Animator.StringToHash("act");
    public bool isBeCastingSkill() { return beCastingSkill; }
    public bool isCastingSkill() { return castingSkill; }
    public void stopBeCastingSKill() { beCastingSkill = false; }
    public void stopCastingSkill() { castingSkill = false; }
    public void setRivalSkill(bool castSkill) { rivalSkill = castSkill; }
    public bool getRivalSkill() { return rivalSkill; }

    public void testSkill(int id)
    {
        // startSkill(id);
    }

    public void setSkill(int pos, int id, int cost)
    {
        skillButton[pos].setCastSkill(id, cost);
    }

    public void updateSkillStatus(int nowMp)
    {
        foreach (var skill in skillButton)
            skill.setEnable(nowMp);
    }

    public void showSkillText(int skillId)
    {
        var skill = MainManager.dataCenter.skillGroup.getSkill(skillId);
        skillName.text = skill.sName;
        skillTextAnime.SetTrigger(actHash);
    }

    public void startBeSkill(int skillId)
    {
        beCastingSkill = true;
        var skill = MainManager.dataCenter.skillGroup.getSkill(skillId);
        rivalSkillName.text = skill.sName;
        // 播放技能名稱跳出動畫
        rivalSkillTextAnime.SetTrigger(actHash);
        // Debug.Log("播放 "+skill.sName+" 動畫");
        // 播放技能預發點
        StartCoroutine(skill.showBeCastSkill());
        // 播放技能
        StartCoroutine(skill.beCastSkill());
    }

    public void startCastSkill(int skillId)
    {
        castingSkill = true;
        var skill = MainManager.dataCenter.skillGroup.getSkill(skillId);
        StartCoroutine(skill.castSkill());
    }

    public void atkShow(int atkId, int defRes)
    {
        int successHash = Animator.StringToHash("success");
        int failHash = Animator.StringToHash("fail");
        var atkList = MainManager.dataCenter.atkMgr.atkList;
        var ap = atkList[atkId];
        if (defRes == DEF_SUCCESS)
        {   // 播放攻擊失敗動畫          
            ap.GetComponentInChildren<Animator>().SetTrigger(failHash);
        }
        else
        {   //  播放攻擊成功動畫
            ap.GetComponentInChildren<Animator>().SetTrigger(successHash);
            ap.startPointAtk();            
        }        
        
        // 如果atkList為空的話就更新施放技能旗標
        atkList.Remove(atkId);
        Debug.Log("count:"+atkList.Count+" list:"+Json.Serialize(atkList));
        if( atkList.Count == 0 )
            stopCastingSkill();
    }
    
}
