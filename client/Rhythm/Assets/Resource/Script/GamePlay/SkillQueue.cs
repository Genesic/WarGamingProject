using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public struct SkillQueueData
{
    public int target;
    public int skillId;
}

public class SkillQueue : MonoBehaviour {
    public GameObject[] queue;
    public const int SELF_SKILL = 1;
    public const int RIVAL_SKILL = 2;
    public void updateQueue(SkillQueueData[] info)
    {
        int i;
        for(i=0; i< queue.Length && i<info.Length; i++)
        {
            var data = info[i];
            var obj = queue[i];
            var skill = MainManager.dataCenter.skillGroup.getSkill(data.skillId);
            obj.SetActive(true);
            obj.GetComponentInChildren<Text>().text = skill.sName;
            obj.GetComponent<Image>().color = ( data.target == SELF_SKILL )? Color.green : Color.red; 
        }
        
        for( ; i<queue.Length ; i++)
        {
            var obj = queue[i];
            obj.SetActive(false);
        }
    }
}
