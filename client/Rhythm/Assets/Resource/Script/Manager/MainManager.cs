using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainManager : MonoBehaviour {
    public static Client socket;
    public static Command command;
    public static int bpm60 = Animator.StringToHash("bpm60");
    public static int bpm90 = Animator.StringToHash("bpm90");
    public static int bpm120 = Animator.StringToHash("bpm120");
    public static int bpm150 = Animator.StringToHash("bpm150");
    public static int bpm180 = Animator.StringToHash("bpm180");
    public static float bpm = 60;
    public static float beat = (60 / bpm) * 2;
    public static bool endGame = false;
    public Text ipHost;
    public static void updateEndGame(bool flag)
    {
        endGame = flag;
    }
    
    public static int getTrigger()
    {
        if( bpm == 90)
            return bpm90;
        else if( bpm == 120)
            return bpm120;
        else if( bpm == 150)
            return bpm150;
        else if( bpm == 180)
            return bpm180;
            
        return bpm90;
    }
    public static void updateBpm(int newBpm, int tempo)
    {
        bpm = newBpm;
        beat = (60 / bpm) * tempo;
    }
    
    public static Dictionary<ClickRes, Color> clickResToColor = new Dictionary<ClickRes, Color>()
    {
        {ClickRes.Perfect, Color.yellow},
        {ClickRes.Great, Color.green},
        {ClickRes.Miss, Color.red},        
    };
    
    public static Dictionary<string, ClickRes> stringToClickResDic = new Dictionary<string, ClickRes>(){
        {"Perfect", ClickRes.Perfect},
        {"Great", ClickRes.Great},
        {"Miss", ClickRes.Miss},
    };
    
    public static Dictionary<int, ClickRes> intToClickResDic = new Dictionary<int, ClickRes>(){
        {(int)ClickRes.Perfect, ClickRes.Perfect},
        {(int)ClickRes.Great, ClickRes.Great},
        {(int)ClickRes.Miss, ClickRes.Miss},
    };
    public static ClickRes stringToClickRes(string str)
    {
        if( stringToClickResDic.ContainsKey(str) )
            return stringToClickResDic[str];
        else
            return ClickRes.None;
    }
    
    public static ClickRes intToClickRes(int num){
        if( intToClickResDic.ContainsKey(num))
            return intToClickResDic[num];
        else
            return ClickRes.None;
    }
    
    void Awake()
    {
        command = gameObject.GetComponent<Command>();
        command.init();
        
        socket = new Client(command);
    }
    
    public void connectToServer()
    {
        socket.connectToServer();
    }
    
    public void connectToInputServer()
    {
        socket.connectToInputServer(ipHost.text);
    }
}
