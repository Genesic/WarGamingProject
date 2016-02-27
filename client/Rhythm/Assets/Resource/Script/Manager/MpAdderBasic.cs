using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MpAdderBasic : IPool {

    private RectTransform mTs = null;
    private float overTime;

    public void setParam(string id) { mID = id; }
    public void setPosition(Vector2 position)
    {
        mTs = (mTs == null) ? gameObject.GetComponent<RectTransform>() : mTs;
        mTs.anchoredPosition = position;
        mTs.localScale = new Vector3(1, 1, 1);
    }
    public void setOverTimer(float sTime) { overTime = sTime; }
    int actHash = Animator.StringToHash("act");
    public void setText(string text, Color color)
    {
        GetComponent<Text>().text = text;
        GetComponent<Text>().color = color;
    }
    public override void SetEnable()
    {
        base.SetEnable();
        GetComponent<Animator>().SetTrigger(actHash) ;  
    }
    
    private IEnumerator closeMpAdder()
    {
        yield return new WaitForSeconds(overTime);
        MpAdderManager.Retrieve(this);
    }
}
