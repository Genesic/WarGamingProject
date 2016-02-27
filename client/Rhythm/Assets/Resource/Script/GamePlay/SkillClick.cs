using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
public class SkillClick : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
{
    private int skillId;
    private int cost;
    public Text sName;
    private bool enable;
    public void setCastSkill(int id, int costMp)
    {
        skillId = id;
        var skill = MainManager.dataCenter.skillGroup.getSkill(id);
        sName.text = skill.sName;
        cost = costMp;
    }

    public void setEnable(int nowMp)
    {
        if( nowMp >= cost){
            enable = true;
            sName.color = Color.black;            
        } else{
            enable = false;
            sName.color = Color.gray;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (enable)
        {
            var skill = MainManager.dataCenter.skillGroup.getSkill(skillId);
            MainManager.socket.SendData("skill [" + skillId + "," + skill.mp + "]");
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }
}
