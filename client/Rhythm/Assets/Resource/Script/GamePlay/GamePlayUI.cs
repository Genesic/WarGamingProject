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
    public Text side;
    public Text chName;
    public Text rivalName;
    public RectTransform hpMaxBar;
    public RectTransform hpBar;
    public RectTransform rivalHpMaxBar;
    public RectTransform rivalHpBar;
    public Animator atkTempo;
    public PreTempo preTempo;
    public GameObject touchPanel;
    public AudioClip correctSE;
    public Text combo;
    public Text rivalCombo;
    public int hpMax;
    public int rivalHpMax;
    public int hp;
    public int rivalHp;

    Dictionary<ActSide, string> sideText = new Dictionary<ActSide, string>(){
        {ActSide.ATTACK, "ATTACK"},
        {ActSide.DEFEND, "DEFEND"}
    };

    public void updateCombo(int times)
    {
        combo.text = (times >= 1)? times + " combo" : string.Empty;
    }
    
    public void updateRivalCombo(int times)
    {
        rivalCombo.text = (times >= 1)? times + " combo" : string.Empty;
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
        float percent = (float)rivalHp / (float)rivalHpMax;
        float width = rivalHpMaxBar.rect.width;
        float height = rivalHpMaxBar.rect.height;
        rivalHpBar.sizeDelta = new Vector2(width * percent, height);
    }
}
