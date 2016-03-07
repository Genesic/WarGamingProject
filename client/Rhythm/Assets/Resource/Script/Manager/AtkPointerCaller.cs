using UnityEngine;
using System.Collections;

public class AtkPointerCaller : MonoBehaviour {
    public AtkPointBasic atkPos;
    
    public void closePoint()
    {
        atkPos.closePoint();
    }
    public void closePointAndShake()
    {
        atkPos.closePointAndShake();
    }
}
