using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class PointDefClick : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
{
    public PreTempo preClick;
    public GamePlayUI gamePlayUI;
    public Dictionary<int, ClickRes> defRes;
    public void init(int round)
    {
        defRes = new Dictionary<int, ClickRes>();            
        for(int i=1; i<=round; i++){
            defRes.Add(i, ClickRes.None);
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        int round = gamePlayUI.getDefRound();
        if (round > 0)
        {
            var res = preClick.getClickResult(Side.Def, gamePlayUI.getDefForAtkRes());
            defRes[round] = res;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }
}
