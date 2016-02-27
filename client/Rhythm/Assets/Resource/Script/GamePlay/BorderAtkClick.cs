using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using SimpleJSON;

public class BorderAtkClick : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
{
    public Image border;
    public ClickRes clickRes = ClickRes.None;
    public Text resText;
    public bool start;
    public bool spButton;
    private int count = 0;
    private Color nColor = new Color32(0, 0, 255, 0);
    private Color pColor = new Color32(255, 87, 0, 0);
    public void init()
    {
        clickRes = ClickRes.None;
        resText.text = string.Empty;
        resText.color = Color.clear;
        start = true;
        count++;
    }
    
    public void processColor()
    {
        if( count % 4 == 0 ){
            GetComponent<Image>().color = pColor;            
        } else {
            GetComponent<Image>().color = nColor;
        }        
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        //DebugPoint(eventData);
        if (start)
        {
            start = false;
            
            if (border.color.a > 0.4)
                clickRes = ClickRes.Perfect;
            else
                clickRes = ClickRes.Miss;
                        
            //Vector2 position = getPoint(eventData);
            JSONNode json = new JSONArray();
            int success = ( count % 4 == 0 && count > 1)? 2 : 1; // 按下去的時候count已經先加過1
            Debug.Log("count:"+count+" success:"+success);
            json[0] = (border.color.a > 0.4)? success.ToString() : "0";            
            
            // json[0] = ((int)clickRes).ToString();
            MainManager.socket.SendData("tempo " + json.ToString());
            
            // StartCoroutine(showResText());
        }
    }
    IEnumerator showResText()
    {
        resText.text = clickRes.ToString();
        resText.color = Color.yellow;
        yield return new WaitForSeconds(0.5f);
        resText.text = string.Empty;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }

    Vector2 getPoint(PointerEventData eventData)
    {
        Vector2 localCursor;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localCursor))
            return Vector2.zero;

        //Debug.Log("LocalCursor:" + localCursor);
        return localCursor;        
    }
}
