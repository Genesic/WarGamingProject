using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum Side
{
    Atk = 1,
    Def = 2,
}

public class PreTempo : MonoBehaviour
{
    public RectTransform preTempo;
    public RectTransform tempo;
    public Text result;
    private AudioSource source;
    void Awake()
    {
        source = GetComponent<AudioSource>();
    }
    public void playSE()
    {
        source.Play();
    }

    public static Dictionary<ClickRes, List<int>> defList = new Dictionary<ClickRes, List<int>>(){
        {ClickRes.Perfect, new List<int>() { 5, 15 } },     // Perfect攻擊的判斷範圍
        {ClickRes.Great, new List<int>() { 10, 30} },       // Great攻擊的判斷範圍
        {ClickRes.Miss, new List<int>() { 20, 40} }         // Miss
    };

    // click結果文字
    Dictionary<ClickRes, string> resText = new Dictionary<ClickRes, string>(){
        {ClickRes.Perfect, "Perfect"},
        {ClickRes.Great, "Great"},
        {ClickRes.Miss, "Miss"},
        {ClickRes.None, ""},
    };

    // click結果顏色 
    Dictionary<ClickRes, Color> resColor = new Dictionary<ClickRes, Color>(){
        {ClickRes.Perfect, new Color32(255, 187, 0, 255) },
        {ClickRes.Great, Color.green},
        {ClickRes.Miss, Color.red},
        {ClickRes.None, Color.clear},
    };
    public void clearResult()
    {
        result.text = string.Empty;
    }
    public void updateResult(ClickRes res)
    {        
        StartCoroutine(showResText(res));
    }
    
    IEnumerator showResText(ClickRes res)
    {
        if (result)
        {
            result.text = resText[res];
            result.color = resColor[res];
        }
        
        yield return new WaitForSeconds(0.5F);
        result.text = string.Empty;
        result.color = Color.clear;
    }

    public ClickRes getClickResult(Side side, ClickRes res)
    {
        float diff = preTempo.rect.width - tempo.rect.width;
        ClickRes clickRes;
        if (side == Side.Atk)
        {   // 攻擊判斷
            if (diff < 5)
                clickRes = ClickRes.Perfect;
            else if (diff < 15)
                clickRes = ClickRes.Great;
            else
                clickRes = ClickRes.Miss;
        }
        else
        {   // 防禦判斷
            if (diff < defList[res][0])
                clickRes = ClickRes.Perfect;
            else if (diff < defList[res][1])
                clickRes = ClickRes.Great;
            else
                clickRes = ClickRes.Miss;
        }

        // 更新結果顯示文字
        updateResult(clickRes);

        return clickRes;
    }
}
