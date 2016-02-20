using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using MiniJSON;

public class Command : MonoBehaviour
{
    public GameFlow gameFlow;
    public StartSelect startSelect;
    public GamePlayUI gamePlayUI;
    public TempoPlay tempoPlay;
    public Skill skill;

    public List<string> socketQueue;

    public void init()
    {
        gameFlow = gameObject.GetComponent<GameFlow>();
        skill = gameObject.GetComponent<Skill>();
    }

    void FixedUpdate()
    {
        while (socketQueue.Count > 0)
        {
            Debug.Log("GetLine:" + socketQueue[0]);
            getSocket(socketQueue[0]);
            socketQueue.RemoveAt(0);
        }
    }

    public void getSocket(string line)
    {
        string[] strs = line.Split(' ');
        string cmd = strs[0];
        string jsons = (strs.Length > 1) ? strs[1] : "[]";
        if (cmd == "ping")
        {
            socket_ping();
        }
        else if (cmd == "login")
        {
            socket_login();
        }
        else if (cmd == "select_role")
        {
            var args = JSON.Parse(jsons);
            int res = int.Parse(args[0]);
            socket_select_role(res);
        }
        else if (cmd == "match")
        {
            var args = JSON.Parse(jsons);
            socket_match(args);
        }
        else if (cmd == "start_game")
        {
            var args = JSON.Parse(jsons);
            socket_start_game(args);
        }
        //        else if (cmd == "def")
        //        { // v2.0
        //            var args = JSON.Parse(jsons);
        //            socket_def(args);
        //        }
        else if (cmd == "be_skilled")
        {
            var args = JSON.Parse(jsons);
            socket_be_skilled(args);
        }
        else if (cmd == "sync")
        {
            var args = JSON.Parse(jsons);
            socket_sync(args);
        }
        else if (cmd == "combo")
        {
            var args = JSON.Parse(jsons);
            socket_combo(args);
        }        
        else if (cmd == "end_game")
        {
            socket_end_game();
        }
    }
    public void socket_ping()
    {
        MainManager.socket.SendData("pong");
    }
    public void socket_login()
    {
        //Dictionary<string, float> tmp = gameFlow.getGameSet();
        //string gameSet = Json.Serialize(tmp);
        // MainManager.socket.SendData("game_set " + gameSet);
        // 開啟選擇角色面板
        startSelect.selectRolePanel.SetActive(true);
    }

    IEnumerator reLogin()
    {
        yield return new WaitForSeconds(3);
        MainManager.socket.connectToServer();
    }

    public void socket_select_role(int res)
    {
        if (res >= 1)
        {
            // 選擇角色成功後開啟match面板
            startSelect.login.SetActive(false);
            startSelect.selectRolePanel.SetActive(false);
            startSelect.match.SetActive(true);
            MainManager.socket.SendData("match");
        }
    }

    public void socket_match(JSONNode data)
    {
        int res = int.Parse(data["res"]);
        if (res == 1)
        {
            int hp = int.Parse(data["hp"]);
            int rivalHp = int.Parse(data["rival_hp"]);
            int role = int.Parse(data["role"]);
            int rivalRole = int.Parse(data["rival_role"]);
            // 初始化
            gamePlayUI.updateCombo(0);
            startSelect.match.SetActive(false);
            gamePlayUI.gameObject.SetActive(true);

            // 我方初始化
            gamePlayUI.hpMax = hp;
            gamePlayUI.updateChName(role);

            // 敵方初始化
            gamePlayUI.rivalHpMax = rivalHp;
            gamePlayUI.updateRivalName(rivalRole);

            MainManager.socket.SendData("ready");
        }
        else
        {
            Debug.Log("not match res:" + res);
        }
    }

    public void socket_start_game(JSONNode data)
    {
        tempoPlay.init();
        tempoPlay.startTempo();
        MainManager.socket.SendData("round_set");
    }

    public void socket_be_skilled(JSONNode data)
    {
        int id = int.Parse(data[0]);
        skill.startSkill(id);
    }

    public void socket_def(JSONNode data)
    {
        int res = int.Parse(data[0]);
        float x = float.Parse(data[1]);
        float y = float.Parse(data[2]);
        tempoPlay.pushDefQueue(res, x, y);
    }

    public void socket_sync(JSONNode data)
    {
        if (!string.IsNullOrEmpty(data["hp"]))
        {
            int hp = data["hp"].AsInt;
            gamePlayUI.updateHp(hp);
        }
        if (!string.IsNullOrEmpty(data["rival_hp"]))
        {
            int rival_hp = data["rival_hp"].AsInt;
            gamePlayUI.updateRivalHp(rival_hp);
        }
    }
    
    public void socket_combo(JSONNode data)
    {
        if(!string.IsNullOrEmpty(data["combo"]))
        {
            int combo = data["combo"].AsInt;
            gamePlayUI.updateCombo(combo);
        }
        
        if(!string.IsNullOrEmpty(data["rival_combo"])){
            int rivalCombo = data["rival_combo"].AsInt;
            gamePlayUI.updateRivalCombo(rivalCombo);
        }
    }

    public void socket_end_game()
    {
        MainManager.updateEndGame(true);
    }
}
