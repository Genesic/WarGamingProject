using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainManager : MonoBehaviour
{
    public static Client socket;
    public static Command command;
    public static DataCenter dataCenter;
    public static Skill skill;
    public static MainManager ins;

    // Use this for initialization
    void Awake()
    {
        if (dataCenter == null)
        {
            dataCenter = gameObject.GetComponent<DataCenter>();
            ins = this;
            GameObject.DontDestroyOnLoad(gameObject);
            skill = gameObject.GetComponent<Skill>();
            command = gameObject.GetComponent<Command>();
            command.init();

            socket = new Client(command);
        }
        else if (dataCenter != gameObject.GetComponent<DataCenter>())
        {
            Destroy(gameObject);
            //Debug.Log ("hello");
            //Debug.Log ("ins:"+ins.GetInstanceID()+" dataCenter:"+dataCenter.GetInstanceID() );
        }
    }

    public static int bpm60 = Animator.StringToHash("bpm60");
    public static bool endGame = false;
    public static void updateEndGame(bool flag)
    {
        endGame = flag;
        command.gamePlayUI.clearSelectRole();
    }

    public static int getSelectRole() { return command.gamePlayUI.getSelectRole(); }

    public void connectToServer()
    {
        socket.connectToServer();
    }

    public Text ipHost;
    public void connectToInputServer()
    {
        socket.connectToInputServer(ipHost.text);
    }
}
