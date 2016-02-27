using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillGroup : MonoBehaviour {
    public Dictionary<int, SkillBase> list;
    void Awake()
    {
        init();
    }
    public void init()
    {
        list = new Dictionary<int, SkillBase>();
        foreach(Transform child in transform)
        {
            var obj = child.gameObject.GetComponent<SkillBase>();            
            list.Add(obj.id, obj);
        }
    }
    
    public SkillBase getSkill(int skillId)
    {
        return list[skillId];
    }
}
