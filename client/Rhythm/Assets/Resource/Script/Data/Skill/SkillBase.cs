using UnityEngine;
using System.Collections;

public class SkillBase : MonoBehaviour {
    public int id;
    public string sName;
    public int mp;
    public virtual IEnumerator castSkill()
    {
        yield return new WaitForSeconds(0.1F);        
    }
}
