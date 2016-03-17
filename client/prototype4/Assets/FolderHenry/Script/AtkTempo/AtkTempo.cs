using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AtkTempo : MonoBehaviour {
    public RectTransform target;
    
    private float tempoTime;
    private Vector2 targetPos;
    public void setStartStatus(Vector2 pos, Vector2 size, Vector2 endPos, Vector2 endSize, float tTime)
    {
        var st = GetComponent<RectTransform>();
        st.sizeDelta = size;
        st.anchoredPosition = pos;        
        target.sizeDelta = endSize;
        targetPos = endPos;
        tempoTime = tTime;
        
        float multiple = tTime / Time.fixedDeltaTime;
        diffX = (endPos.x - pos.x) / multiple;
        diffY = (endPos.y - pos.y) / multiple;
        diffSizeX = (endSize.x - size.x) / multiple;
        diffSizeY = (endSize.y - size.y) / multiple;
    }
    
    public void startScale()
    {
        startFlag = true;
    }
    private bool startFlag = false;
    float diffX;
    float diffY;
    float diffSizeX;
    float diffSizeY;
    void FixedUpdate()
    {
        if(startFlag)
        {
            var rt = GetComponent<RectTransform>();
            Vector2 nowPos = rt.anchoredPosition;
            rt.anchoredPosition = new Vector2(nowPos.x + diffX, nowPos.y + diffY);
            Vector2 nowSize = rt.sizeDelta;
            rt.sizeDelta = new Vector2(nowSize.x + diffSizeX, nowSize.y + diffSizeY );
            
            if( rt.sizeDelta.x <= target.sizeDelta.x )
                startFlag = false;
        }
    }
}
