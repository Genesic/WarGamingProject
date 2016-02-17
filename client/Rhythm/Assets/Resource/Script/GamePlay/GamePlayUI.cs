using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public enum ActSide
{
    ATTACK = 1,
    DEFEND = 2,
}

public class GamePlayUI : MonoBehaviour
{
    public Text roundTimes;
    public Text combo;
    public Text side;
    public Text chName;
    public Text rivalName;
    public RectTransform hpMaxBar;
    public RectTransform hpBar;
    public RectTransform rivalHpMaxBar;
    public RectTransform rivalHpBar;
    public Animator atkTempo;
    public PreTempo preTempo;
    public PreTempo preClick;
    public GameObject touchPanel;
    public PointDefClick pointDefClick;
    public AudioClip correctSE;
    public int hpMax;
    public int hp;
    public int rivalHp;


    Dictionary<ActSide, string> sideText = new Dictionary<ActSide, string>(){
        {ActSide.ATTACK, "ATTACK"},
        {ActSide.DEFEND, "DEFEND"}
    };

    public void updateCombo(int times)
    {
        if (times >= 1)
        {
            combo.text = times + " combo";
        }
        else
        {
            combo.text = string.Empty;
        }
    }

    public void updateSide(ActSide act)
    {
        side.text = sideText[act];
    }

    public void updateChName(int id)
    {
        string useName = Character.getName(id);
        chName.text = useName;
    }

    public void updateRivalName(int id)
    {
        string useName = Character.getName(id);
        rivalName.text = useName;
    }

    public void updateHp(int newHp)
    {
        hp = newHp;
        float percent = (float)hp / (float)hpMax;
        float width = hpMaxBar.rect.width;
        float height = hpMaxBar.rect.height;
        hpBar.sizeDelta = new Vector2(width * percent, height);
    }

    public void updateRivalHp(int newHp)
    {
        rivalHp = newHp;
        float percent = (float)rivalHp / (float)hpMax;
        float width = rivalHpMaxBar.rect.width;
        float height = rivalHpMaxBar.rect.height;
        rivalHpBar.sizeDelta = new Vector2(width * percent, height);
    }

    // 開始攻擊
    public void startAttack()
    {
        touchPanel.SetActive(true);
        StartCoroutine(atkAnime());
    }

    private int atk_round = 0;
    public int atkMaxRound = 4;
    IEnumerator atkAnime()
    {
        // 初始化
        JSONNode res = new JSONClass();
        var pointAtkClick = touchPanel.GetComponent<PointAtkClick>();
        side.text = "ATTACK";
        side.color = Color.blue;
        pointAtkClick.init(atkMaxRound);
        
        yield return new WaitForSeconds(MainManager.beat/2);
        // 攻擊處理
        for (atk_round = 1; atk_round <= atkMaxRound; atk_round++)
        {
            roundTimes.text = atk_round.ToString();
            roundTimes.color = Color.blue;
            atkTempo.SetTrigger(MainManager.getTrigger());
            yield return new WaitForSeconds(MainManager.beat/2);
            string idx = atk_round.ToString();
            
            if( pointAtkClick.atkRes[atk_round] == ClickRes.None )
            {   // 如果到最後都沒按的話就當miss處理
                preTempo.updateResult(ClickRes.Miss);
                pointAtkClick.atkRes[atk_round] = ClickRes.Miss;
            }
            yield return new WaitForSeconds(MainManager.beat/2);
            res[idx][0] = ((int)pointAtkClick.atkRes[atk_round]).ToString();
            res[idx][1] = pointAtkClick.atkPos[atk_round].x.ToString();
            res[idx][2] = pointAtkClick.atkPos[atk_round].y.ToString();
        }

        atk_round = 0;
        roundTimes.color = Color.clear;
        MainManager.socket.SendData("attack " + res.ToString());
        touchPanel.SetActive(false);
    }

    public int getAtkRound() { return atk_round; }

    // 開始防禦
    private int def_round;
    public int getDefRound() { return def_round; }
    private ClickRes defForAtkRes;
    public ClickRes getDefForAtkRes() { return defForAtkRes; }
    public void startDefend(JSONNode data)
    {
        StartCoroutine(defAnime(data));
    }

    IEnumerator defAnime(JSONNode data)
    {
        // 初始化
        JSONNode resJson = new JSONArray();
        RectTransform preClickRT = preClick.gameObject.GetComponent<RectTransform>();
        Animator preClickAnime = preClick.gameObject.GetComponent<Animator>();
        side.text = "DEFEND";
        side.color = Color.red;
        pointDefClick.init(atkMaxRound);
        
        // 開始處理防禦
        for (def_round = 1; def_round <= atkMaxRound; def_round++)
        {
            roundTimes.text = def_round.ToString();
            roundTimes.color = Color.red;
            string idx = def_round.ToString();
            Debug.Log(data[idx][0]);
            defForAtkRes = MainManager.intToClickRes(int.Parse(data[idx][0]));
            float x = float.Parse(data[idx][1]);
            float y = float.Parse(data[idx][2]);
            if (PreTempo.defList.ContainsKey(defForAtkRes))
            {
                preClickRT.position = new Vector2(x, y);
                preClick.gameObject.SetActive(true);
                preClickAnime.SetTrigger(MainManager.getTrigger());
            }
            else
            {
                preClick.gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(MainManager.beat/2);
            if( pointDefClick.defRes[def_round] == ClickRes.None )
            {   // 如果到最後都沒按的話就當miss處理
                preClick.updateResult(ClickRes.Miss);
                pointDefClick.defRes[def_round] = ClickRes.Miss;
            }
            yield return new WaitForSeconds(MainManager.beat/2);
            // 取得防禦結果
            ClickRes defRes = pointDefClick.defRes[def_round];
            int clickRes = (int)defRes;
            // Debug.Log("defRes:"+defRes+" def_round:"+def_round+" clickRes:"+clickRes);
            resJson[def_round-1] = clickRes.ToString();
        }

        // 清除防禦狀態
        roundTimes.color = Color.clear;
        def_round = 0;
        defForAtkRes = ClickRes.None;
        preClick.gameObject.SetActive(false);

        // 送出防禦結果
        MainManager.socket.SendData("defend_res " + resJson.ToString());
    }
}
