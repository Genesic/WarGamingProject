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
    public void init()
    {
        clickRes = ClickRes.None;
        resText.text = string.Empty;
        resText.color = Color.clear;
        start = true;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        //DebugPoint(eventData);
        if (start)
        {
            start = false;
            if (border.color.a == 1)
                clickRes = ClickRes.Perfect;
            else if (border.color.a > 0.4)
                clickRes = ClickRes.Great;
            else
                clickRes = ClickRes.Miss;

            Vector2 position = getPoint(eventData);
            JSONNode json = new JSONArray();
            json[0] = ((int)clickRes).ToString();
            json[1] = position.x.ToString();
            json[2] = position.y.ToString();
            MainManager.socket.SendData("atk " + json.ToString());
            StartCoroutine(showResText());
        }
    }
    IEnumerator showResText()
    {
        resText.text = clickRes.ToString();
        resText.color = MainManager.clickResToColor[clickRes];
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
