using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AtkPointBasic : IPool {

    private RectTransform mTs = null;
    private float overTime;
    private GameObject show;

    public void setParam(string id) { mID = id; }
    public void setPosition(Vector2 position)
    {
        foreach( Transform child in transform)
            show = child.gameObject;
                                 
        mTs = (mTs == null) ? gameObject.GetComponent<RectTransform>() : mTs;
        mTs.anchoredPosition = position;
        mTs.localScale = new Vector3(1, 1, 1);
        GetComponentInChildren<Image>().color = new Color32(255, 255, 255, 50);
        show.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }
    public void setOverTimer(float sTime) { overTime = sTime; }
    public override void SetEnable()
    {
        base.SetEnable();
        StartCoroutine(closeDefendPoint());
    }
    
    private IEnumerator closeDefendPoint()
    {
        yield return new WaitForSeconds(overTime);
        AtkPointManager.Retrieve(this);
        flag = false;        
    }
    
    public void closePoint()
    {
        AtkPointManager.Retrieve(this);
        flag = false;
    }
    public void closePointAndShake()
    {
        MainManager.dataCenter.skillGroup.startShakeRivalHead(0.5F);
        closePoint();
    }
    
    private Vector2 targetPos = new Vector2(700, 470);
    private float diffX;
    private float diffY;
    private bool flag = false;
    public void startPointAtk()
    {
        flag = true;
        diffX = (targetPos.x - mTs.anchoredPosition.x) / 20;
        diffY = (targetPos.y - mTs.anchoredPosition.y) / 20;       
    }
    
    void FixedUpdate()
    {
        if( flag )        
            mTs.anchoredPosition += new Vector2(diffX, diffY);                                
    }
}
