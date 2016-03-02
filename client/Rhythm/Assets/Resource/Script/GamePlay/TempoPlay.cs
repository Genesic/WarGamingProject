using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System;

public struct DefData
{
    public int res;
    public float x;
    public float y;
}
public enum ClickRes
{
    Perfect = 1,
    Great,
    Miss,
    None,   // not yet
};
public class TempoPlay : MonoBehaviour
{
    private BorderAtkClick borderAtkClick;
    // public BorderDefClick borderDefclick;
    public List<DefData> defQueue = new List<DefData>();
    private Animator flashBorder;
    private AudioSource source;

    public void init()
    {
        borderAtkClick = GetComponent<BorderAtkClick>();
        flashBorder = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        MainManager.bgm.Play();
    }
    public void playSE()
    {
        source.Play();
    }

    public void processStart()
    {
        borderAtkClick.init();
    }

    public void processEnd()
    {
        borderAtkClick.processColor();
        //        if (borderAtkClick.clickRes == ClickRes.None)
        if (borderAtkClick.successFlag == false)
            MainManager.socket.SendData("tempo [0]");
    }

    public void startTempo()
    {
        StartCoroutine(playTempo());
    }

    public void pushDefQueue(int res, float x, float y)
    {
        //var clickRes = MainManager.intToClickRes(res);
        DefData defData = new DefData();
        defData.res = res;
        defData.x = x;
        defData.y = y;
        defQueue.Add(defData);
    }
    IEnumerator playTempo()
    {
        int role = MainManager.getSelectRole();
        var cobj = MainManager.dataCenter.characterGroup.getCharacter(role);
        //float startTempo = 0.5f;
        //yield return new WaitForSeconds(cobj.getBeat(0.5f));

        while (!MainManager.endGame)
        {
            // 播動畫
            flashBorder.SetTrigger(MainManager.bpm60);
            yield return new WaitForSeconds(cobj.getBeat(2));

            // 不是被施放技能中的話可以被施放技能            
            if (!MainManager.skill.isCastingSkill() && MainManager.skill.getRivalSkillQueueNum() > 0)
                MainManager.socket.SendData("be_skill");
        }
    }
//=============以下測試=================
    public void testTempo()
    {
        init();
        //GetComponent<BorderAtkClick>().trace = true;
        StartCoroutine(playTest());
    }
    IEnumerator playTest()
    {
        int role = 1;
        var cobj = MainManager.dataCenter.characterGroup.getCharacter(role);

        DateTime st;
        DateTime ed;
        while (!MainManager.endGame)
        {
            st = DateTime.Now;            
            // 播動畫
            GetComponent<Animator>().SetTrigger(MainManager.bpm60);
            yield return new WaitForSeconds(cobj.getBeat(2));
            ed = DateTime.Now;
            TimeSpan diff = ed - st;
            int ms = (int)diff.TotalMilliseconds;
            Debug.Log("act:"+ms);
        }
    }
    /*
    IEnumerator playTempo()
    {
        while (!MainManager.endGame)
        {
            // 初始化
            borderAtkClick.init();
            bool defStart = false;
            // Debug.Log("queue count:"+defQueue.Count);  
            if( defQueue.Count > 0 ){
                DefData defData = defQueue[0];
                // borderDefclick.init(defData.x, defData.y);
                borderDefclick.gameObject.SetActive(false);
                defStart = true;
            }   

            // 播動畫
            flashBorder.SetTrigger(MainManager.bpm60);
            
            yield return new WaitForSeconds(MainManager.beat/2);
            // Debug.Log("wait second:"+MainManager.beat/2);
            // 防禦節拍出現
            if(defStart)
                borderDefclick.gameObject.SetActive(true);            
            yield return new WaitForSeconds(MainManager.beat/2);
            
            // 防禦結果            
            if( defStart ){
                int result = (int)borderDefclick.clickRes;
                Debug.Log("result:"+result);
                MainManager.socket.SendData("def_res ["+result+"]");
                defQueue.RemoveAt(0);
                borderDefclick.gameObject.SetActive(false);
            }                                                                                  
        }
    }
    */

    DateTime startTime;
    DateTime endTime;
    public void startCalc()
    {
        startTime = DateTime.Now;
    }

    public void endCalc()
    {
        endTime = DateTime.Now;
        TimeSpan span = endTime - startTime;
        int ms = (int)span.TotalMilliseconds;

        Debug.Log(ms);
    }
}
