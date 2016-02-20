using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DefendPointGroup : MonoBehaviour
{
    public Dictionary<TouchType, DefendPoint> list;
    void Awake()
    {
        list = new Dictionary<TouchType, DefendPoint>();

        foreach (Transform child in transform)
        {
            var obj = child.gameObject.GetComponent<DefendPoint>();
            list[obj.type] = obj;
        }
    }

    public DefendPoint getDefendPoint(TouchType type) { return list[type]; }
}
