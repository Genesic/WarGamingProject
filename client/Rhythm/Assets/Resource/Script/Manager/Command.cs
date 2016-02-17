﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using MiniJSON;

public class Command : MonoBehaviour {
    public GameFlow gameFlow;
    public StartSelect startSelect;
    public GamePlayUI gamePlayUI;
    public TempoPlay tempoPlay;
    
    public List<string> socketQueue;
    
    public void init()
    {
        gameFlow = gameObject.GetComponent<GameFlow>();
    }
    
    void FixedUpdate()
    {
        while( socketQueue.Count > 0 ){
            Debug.Log("GetLine:"+socketQueue[0]);
            getSocket(socketQueue[0]);
            socketQueue.RemoveAt(0);
        }
    }
    
    public void getSocket(string line)
    {
        string[] strs = line.Split(' ');
        string cmd = strs[0];
        string jsons = ( strs.Length > 1)? strs[1] : "[]";
        if( cmd == "ping"){
            socket_ping();
        } else if( cmd == "login"){
            var args = JSON.Parse(jsons);
            int res = int.Parse(args[0]);
            socket_login(res);
        } else if( cmd == "select_role"){
            var args = JSON.Parse(jsons);
            int res = int.Parse(args[0]);
            socket_select_role(res);
        } else if( cmd == "match"){
            var args = JSON.Parse(jsons);
            int res = int.Parse(args[0]);
            if( res == 1 )
                socket_match();
        } else if( cmd == "start_game"){
            var args = JSON.Parse(jsons);
            socket_start_game(args);
        } else if( cmd == "round_set"){
            var args = JSON.Parse(jsons);
            socket_round_set(args);
        } else if( cmd == "round_start"){
            socket_round_start();
        } else if( cmd == "defend_start"){ // v1.0
            var args = JSON.Parse(jsons);
            socket_defend_start(args);
        } else if( cmd == "def"){ // v2.0
            var args = JSON.Parse(jsons);
            socket_def(args);
        } else if( cmd == "sync"){
            var args = JSON.Parse(jsons);
            socket_sync(args);
        } else if( cmd == "end_game"){
            socket_end_game();
        }
    }
    public void socket_ping()
    {
        MainManager.socket.SendData("pong");
    }
    public void socket_login(int res)
    {
        if(res == 1){            
            Dictionary<string, float> tmp = gameFlow.getGameSet();
            string gameSet = Json.Serialize(tmp);
            MainManager.socket.SendData("game_set "+gameSet);
            // 開啟選擇角色面板
            startSelect.selectRolePanel.SetActive(true);
        } else {
            // StartCoroutine(reLogin());
        }
    }
    
     IEnumerator reLogin()
     {
         yield return new WaitForSeconds(3);
         MainManager.socket.connectToServer();
     }
     
     public void socket_select_role(int res)
     {
         if( res >= 1 ){
            // 選擇角色成功後開啟match面板
            startSelect.login.SetActive(false);
            startSelect.selectRolePanel.SetActive(false);
            startSelect.match.SetActive(true);
            MainManager.socket.SendData("match");
         }
     }
     
     public void socket_match()
     {
         gamePlayUI.updateCombo(0);
         startSelect.match.SetActive(false);
         gamePlayUI.gameObject.SetActive(true);
         MainManager.socket.SendData("ready");         
     }
     
     public void socket_start_game(JSONNode data)
     {
         int maxHp = int.Parse(data["max_hp"]);
         int role = int.Parse(data["role"]);
         int rivalRole = int.Parse(data["rival_role"]);
         Debug.Log("maxHp:"+maxHp+" role:"+role+" rivalRole:"+rivalRole);
         gamePlayUI.hpMax = maxHp;
         gamePlayUI.updateChName(role);
         gamePlayUI.updateRivalName(rivalRole);
         MainManager.socket.SendData("round_set");
     }
     
     public void socket_round_set(JSONNode data)
     {
         int hp = int.Parse(data["hp"]);
         int rival_hp = int.Parse(data["rival_hp"]);
         int max_atk = int.Parse(data["max_atk"]);
         int bpm = int.Parse(data["bpm"]);
         int tempo = int.Parse(data["tempo"]);
         
         gamePlayUI.updateHp(hp);
         gamePlayUI.updateRivalHp(rival_hp);
         gamePlayUI.atkMaxRound = max_atk;
         MainManager.updateBpm(bpm, tempo);
         MainManager.socket.SendData("round_ready");
     }
     
     public void socket_round_start()
     {
         tempoPlay.init();
         tempoPlay.startTempo();
         // gamePlayUI.startAttack();
     }
     
     public void socket_defend_start(JSONNode data)
     {
         gamePlayUI.startDefend(data);             
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
         int hp = int.Parse(data["hp"]);
         int rival_hp = int.Parse(data["rival_hp"]);
         gamePlayUI.updateHp(hp);
         gamePlayUI.updateRivalHp(rival_hp);
     }
     
     public void socket_end_game()
     {
         MainManager.updateEndGame(true);
     }
}
