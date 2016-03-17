using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {
    public AtkTempo atkTempo;
    
    public void startAtk()
    {
        Vector2 pos = new Vector2(300, -300);
        Vector2 size = new Vector2(400, 400);
        Vector2 tPos = new Vector2(-100, -500);
        Vector2 tSize = new Vector2(100, 100);
        atkTempo.setStartStatus(pos, size, tPos, tSize, 0.6F);
        atkTempo.startScale();
    }
}
