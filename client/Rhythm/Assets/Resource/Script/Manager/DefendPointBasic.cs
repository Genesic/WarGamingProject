using UnityEngine;
using System.Collections;

public class DefendPointBasic : IPool
{
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
    public override void SetEnable()
    {
        base.SetEnable();
        gameObject.GetComponent<BorderDefClick>().init();
        StartCoroutine(closeDefendPoint());
    }

    private IEnumerator closeDefendPoint()
    {
        yield return new WaitForSeconds(overTime);
        DefendPointManager.Retrieve(this);

        var pointDown = gameObject.GetComponent<BorderDefClick>();
        if (pointDown.clickRes == ClickRes.Miss)
            MainManager.socket.SendData("def_res [0]");
    }
}
