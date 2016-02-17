using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public class PointAtkClick : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler {
    public PreTempo preTempo;
    public GamePlayUI gamePlayUI;
    private Dictionary<int, bool> roundRecord;
    public Dictionary<int, Vector2> atkPos;
    public Dictionary<int, ClickRes> atkRes;
    public void init(int round)
    {
        roundRecord = new Dictionary<int, bool>();
        atkPos = new Dictionary<int, Vector2>();
        atkRes = new Dictionary<int, ClickRes>();
        
        for(int i=1; i<=round ; i++){
            atkPos.Add(i, Vector2.zero);
            atkRes.Add(i, ClickRes.None);
        }
    }
    public void OnPointerDown(PointerEventData eventData)    
    {
        int round = gamePlayUI.getAtkRound();
        if( !roundRecord.ContainsKey(round) && round > 0 ){
            roundRecord.Add(round, true);   
            var res = preTempo.getClickResult(Side.Atk, ClickRes.None);
            atkRes[round] = res;
            atkPos[round] = eventData.position;
        }       
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }
}
