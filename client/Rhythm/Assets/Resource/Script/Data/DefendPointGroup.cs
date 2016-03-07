using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum TouchType {
    ForAtkShow = 0,
    ForDefShow = 1,
    Click = 2,    
}
public class DefendPointGroup : MonoBehaviour
{
    Dictionary<TouchType, DefendPoint> list;
    Dictionary<string, DefendPoint> idList;
    void Awake()
    {
        list = new Dictionary<TouchType, DefendPoint>();
        idList = new Dictionary<string, DefendPoint>();

        foreach (Transform child in transform)
        {
            var obj = child.gameObject.GetComponent<DefendPoint>();
            list[obj.type] = obj;
            idList.Add(obj.type.ToString(), obj);
        }
    }

    public DefendPoint getDefendPoint(TouchType type) { return list[type]; }
    public DefendPoint getDefendPointByString(string id) { return idList[id]; }
}
