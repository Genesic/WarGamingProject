using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public struct DefData {
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
    public BorderAtkClick borderAtkClick;
    public BorderDefClick borderDefclick;
    public List<DefData> defQueue = new List<DefData>(); 
    private Animator flashBorder;
    private AudioSource source;

    public void init()
    {
        borderAtkClick = GetComponent<BorderAtkClick>();
        flashBorder = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
    }
    
    public void playSE()
    {
        source.Play();
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
        while (!MainManager.endGame)
        {
            // 初始化
            borderAtkClick.init();
            bool defStart = false;
            // Debug.Log("queue count:"+defQueue.Count);  
            if( defQueue.Count > 0 ){
                DefData defData = defQueue[0];
                borderDefclick.init(defData.x, defData.y);
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
}
