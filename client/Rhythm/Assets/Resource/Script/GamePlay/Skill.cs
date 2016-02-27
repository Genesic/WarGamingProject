using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Skill : MonoBehaviour
{
    public Text skillName;
    public Text rivalSkillName;
    public Animator skillTextAnime;
    public Animator rivalSkillTextAnime;
    public SkillClick[] skillButton;
    private bool castingSkill;
    private int rivalSkillQueue;
    int actHash = Animator.StringToHash("act");
    public bool isCastingSkill() { return castingSkill; }
    public void stopCastingSKill(){ castingSkill = false; }
    public void setRivalSkillQueueNum(int num) {rivalSkillQueue = num; }
    public int getRivalSkillQueueNum() { return rivalSkillQueue; }

    public void testSkill(int id)
    {
        startSkill(id);
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

    public void startSkill(int skillId)
    {
        castingSkill = true;
        var skill = MainManager.dataCenter.skillGroup.getSkill(skillId);
        rivalSkillName.text = skill.sName;
        // 播放技能名稱跳出動畫
        rivalSkillTextAnime.SetTrigger(actHash);
        Debug.Log("播放 "+skill.sName+" 動畫");
        // 播放技能
        StartCoroutine(skill.castSkill());
    }
}
