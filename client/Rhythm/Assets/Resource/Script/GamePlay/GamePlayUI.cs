using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public enum ActSide
{
    ATTACK = 1,
    DEFEND = 2,
}

public class GamePlayUI : MonoBehaviour
{
    public Text side;
    public Image headPic;
    public Image rivalPic;
    public Text chName;
    public Text rivalName;
    public RectTransform hpMaxBar;
    public RectTransform hpBar;
    public RectTransform rivalHpMaxBar;
    public RectTransform rivalHpBar;
    public RectTransform mpMaxBar;
    public RectTransform mpBar;
    public RectTransform rivalMpMaxBar;
    public RectTransform rivalMpBar;
    public SkillQueue skillQueue;
    
    public Text combo;
    public Text rivalCombo;
    public int comboNum;
    public int hpMax;
    public int rivalHpMax;
    public int hp;
    public int rivalHp;    
    public int mpMax;
    public int rivalMpMax;
    public int mp;
    public int rivalMp;
    
    private int selectRole;
    private int rivalSelectRole;
    
    public int getSelectRole() {return selectRole;}
    public int getRivalSelectRole() {return rivalSelectRole; }
    public void clearSelectRole() { selectRole = 0; rivalSelectRole = 0; }

    Dictionary<ActSide, string> sideText = new Dictionary<ActSide, string>(){
        {ActSide.ATTACK, "ATTACK"},
        {ActSide.DEFEND, "DEFEND"}
    };

    public void updateCombo(int times)
    {
        comboNum = times;
        combo.text = (times >= 1)? times + " combo" : string.Empty;
    }
    
    public void updateRivalCombo(int times)
    {
        rivalCombo.text = (times >= 1)? times + " combo" : string.Empty;
    }

    public void updateSide(ActSide act)
    {
        side.text = sideText[act];
    }

    public void initCharacter(int id)
    {
        var cobj = MainManager.dataCenter.characterGroup.getCharacter(id);
        chName.text = cobj.cName;
        headPic.sprite = cobj.headPic;        
        hpMax = cobj.maxHp;
        mpMax = cobj.maxMp;
        updateHp(hpMax);
        
        // 設定技能
        for(int i=0 ; i< cobj.skillList.Length ; i++)
        {
            var skill = cobj.skillList[i];
            MainManager.skill.setSkill(i, skill.id, skill.mp);            
        }
        updateMp(0);        
               
        selectRole = id;     
    }
    
    public void initRivalCharacter(int id)
    {
        var cobj = MainManager.dataCenter.characterGroup.getCharacter(id);
        rivalName.text = cobj.cName;
        rivalPic.sprite = cobj.headPic;
        rivalHpMax = cobj.maxHp;
        rivalMpMax = cobj.maxMp;
        updateRivalHp(rivalHpMax);
        updateRivalMp(0);
        
        rivalSelectRole = id;                
    }

    public void updateHp(int newHp)
    {
        hp = newHp;
        float percent = (float)hp / (float)hpMax;
        float width = hpMaxBar.rect.width;
        float height = hpMaxBar.rect.height;
        hpBar.sizeDelta = new Vector2(width * percent, height);
    }

    public void updateRivalHp(int newHp)
    {
        rivalHp = newHp;
        float percent = (float)rivalHp / (float)rivalHpMax;
        float width = rivalHpMaxBar.rect.width;
        float height = rivalHpMaxBar.rect.height;
        rivalHpBar.sizeDelta = new Vector2(width * percent, height);
    }
    
    public void updateMp(int newMp)
    {
        // mp增加時顯示動畫
        if( newMp - mp > 0 )
        {
            var ma = MainManager.dataCenter.maMgr.Obtain("self");
            ma.setOverTimer(0.2f);            
            ma.setText("+"+(newMp -mp), new Color32(255,255,0,0) );
            ma.setPosition(new Vector2(-50, 235));
            ma.SetEnable();
        }
        mp = newMp;
        float percent = (float)mp / (float)mpMax;
        float width = mpMaxBar.rect.width;
        float height = mpMaxBar.rect.height;
        mpBar.sizeDelta = new Vector2(width * percent, height);
        
        MainManager.skill.updateSkillStatus(mp);       
    }
    public void updateRivalMp(int newMp)
    {
        // mp增加時顯示動畫
        if( newMp - rivalMp > 0 )
        {
            var ma = MainManager.dataCenter.maMgr.Obtain("rival");
            ma.setOverTimer(0.2f);            
            ma.setText("+"+(newMp -rivalMp), new Color32(255,255,0,0) );
            ma.setPosition(new Vector2(60, 235));
            ma.SetEnable();
        }        
        rivalMp = newMp;
        float percent = (float)rivalMp / (float)rivalMpMax;
        float width = rivalMpMaxBar.rect.width;
        float height = rivalMpMaxBar.rect.height;
        rivalMpBar.sizeDelta = new Vector2(width * percent, height);
    }
    
    public void updateQueuSkill(SkillQueueData[] info)
    {
        skillQueue.updateQueue(info);
    }
}
