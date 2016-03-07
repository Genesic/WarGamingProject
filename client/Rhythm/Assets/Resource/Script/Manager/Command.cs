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
    public GamePlayObject gamePlayObj;

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
        var args = JSON.Parse(jsons);
        if (cmd == "ping")
        {
            socket_ping();
        }
        else if (cmd == "login")
        {
            socket_login();
        }
        else if (cmd == "choose_mode")
        {
            socket_choose_mode();
        }
        else if (cmd == "select_role")
        {
            int res = int.Parse(args[0]);
            socket_select_role(res);
        }
        else if (cmd == "match")
        {
            socket_match(args);
        }
        else if (cmd == "start_game")
        {
            socket_start_game(args);
        }
        else if (cmd == "skill")
        {   // 技能放進施放陣列
            socket_skill(args);
        }
        else if (cmd == "be_skill")
        {   // 被使用技能 
            socket_be_skilled(args);
        }
        else if (cmd == "use_skill")
        {   // 使用技能
            socket_use_skill(args);
        }
        else if (cmd == "atk_show")
        {	// 使用技能攻擊對手時顯示攻擊的點 
            socket_atk_show(args);
        }
        else if (cmd == "def_show")
        {
            socket_def_show(args);            
        }
        else if (cmd == "skill_queue")
        {
            socket_skill_queue(args);
        }
        else if (cmd == "sync")
        {
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

        // 開啟選擇模式面板
        startSelect.selectModePanel.SetActive(true);
    }

    public void socket_choose_mode()
    {
        // 關閉選擇模式面板 開啟選擇角色面板
        startSelect.selectModePanel.SetActive(false);
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
            SkillQueueData[] tmp = new SkillQueueData[0];
            gamePlayUI.skillQueue.updateQueue(tmp); 

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
        gamePlayObj.startGame();
    }

    public void socket_skill(JSONNode data)
    {
        int id = data[0].AsInt;
        skill.showSkillText(id);
    }

    public void socket_be_skilled(JSONNode data)
    {
        int id = int.Parse(data[0]);
        skill.startBeSkill(id);
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
        if (data["hp"] != null)
            gamePlayUI.updateHp(data["hp"].AsInt);

        if (data["rival_hp"] != null)
            gamePlayUI.updateRivalHp(data["rival_hp"].AsInt);

        if (data["mp"] != null)
            gamePlayUI.updateMp(data["mp"].AsInt);

        if (data["rival_mp"] != null)
            gamePlayUI.updateRivalMp(data["rival_mp"].AsInt);

        if (data["combo"] != null)
            gamePlayUI.updateCombo(data["combo"].AsInt);

        if (data["rival_combo"] != null)
            gamePlayUI.updateRivalCombo(data["rival_combo"].AsInt);
        /*            
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
        */
    }

    public void socket_use_skill(JSONNode data)
    {
        MainManager.skill.showSkillText(data[0].AsInt);
        skill.startCastSkill(data[0].AsInt);
    }

    public void socket_skill_queue(JSONNode data)
    {
        int count = (gamePlayUI.skillQueue.queue.Length > data.AsArray.Count) ? data.AsArray.Count : gamePlayUI.skillQueue.queue.Length;
        var info = new SkillQueueData[count];
        for (int i = 0; i < count; i++)
        {
            info[i].target = data[i][0].AsInt;
            info[i].skillId = data[i][1].AsInt;
        }

        gamePlayUI.skillQueue.updateQueue(info);
        
        // 如果queue裡面第一個是對手技能的話更新施放旗標
        if( count > 0 && info[0].target == SkillQueue.RIVAL_SKILL )
            MainManager.skill.setRivalSkill(true);
    }
    
    public void socket_atk_show(JSONNode data)
    {
        skill.atkShow(data[0].AsInt, data[1].AsInt);
    }
    
    public void socket_def_show(JSONNode data)
    {                
    }

    public void socket_end_game()
    {
        MainManager.updateEndGame(true);
    }
}
