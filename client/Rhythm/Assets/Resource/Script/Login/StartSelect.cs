using UnityEngine;
using System.Collections;

public class StartSelect : MonoBehaviour {
    public GameObject login;
    public GameObject selectRolePanel;
    public GameObject match;
    public GameObject selectModePanel;
    public void selectRole(int role)
    {
        MainManager.socket.SendData("select_role ["+role+"]");
    }
}
