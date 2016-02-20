using UnityEngine;
using System.Collections;

public enum TouchType {
    Click = 1,    
}

public class DefendPoint : MonoBehaviour {
    public float overTime;
    public TouchType type;
}
