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
        else if (cmd == "skill")
        {   // 技能放進施放陣列
            var args = JSON.Parse(jsons);
            socket_skill(args);
        }
        else if (cmd == "be_skill")
        {   // 被使用技能 
            var args = JSON.Parse(jsons);
            socket_be_skilled(args);
        }
        else if( cmd == "use_skill")
        {   // 使用技能
            
        }
        else if (cmd == "sync")
        {
            var args = JSON.Parse(jsons);
            socket_sync(args);
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
            int role = int.Parse(data["role"]);
            int rivalRole = int.Parse(data["rival_role"]);
            
            // 初始化
            gamePlayUI.updateCombo(0);
            gamePlayUI.updateRivalCombo(0);
            startSelect.match.SetActive(false);
            gamePlayUI.gameObject.SetActive(true);

            // 我方初始化
            gamePlayUI.initCharacter(role);

            // 敵方初始化
            gamePlayUI.initRivalCharacter(rivalRole);

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
    
    public void socket_skill(JSONNode data)
    {
        int id = data[0].AsInt;
        skill.showSkillText(id);
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
        if ( data["hp"] != null)
            gamePlayUI.updateHp(data["hp"].AsInt);
        
        if ( data["rival_hp"] != null )
            gamePlayUI.updateRivalHp(data["rival_hp"].AsInt);
        
        if ( data["mp"] != null)        
            gamePlayUI.updateMp(data["mp"].AsInt);
        
        if ( data["rival_mp"] != null)
            gamePlayUI.updateRivalMp(data["rival_mp"].AsInt);
      
        if( data["combo"] != null )
            gamePlayUI.updateCombo(data["combo"].AsInt);
                
        if( data["rival_combo"] != null )
            gamePlayUI.updateRivalCombo(data["rival_combo"].AsInt);
            
        if( data["skill_queue"] != null )
        {
            int[] skillIds = new int[data["skill_queue"].Count];
            for(int i=0; i<skillIds.Length ; i++)
                skillIds[i] = data["skill_queue"][i].AsInt;
                            
            gamePlayUI.updateQueuSkill(skillIds);
        }

        if( data["rival_skill_queue"] != null )
        {
            int[] skillIds = new int[data["rival_skill_queue"].Count];
            for(int i=0; i<skillIds.Length ; i++)
                skillIds[i] = data["rival_skill_queue"][i].AsInt;
                                
            skill.setRivalSkillQueueNum(skillIds.Length);
            gamePlayUI.updateRivalQueueSkll(skillIds);
        }            
    }
    
    public void socket_end_game()
    {
        MainManager.updateEndGame(true);
    }
}
