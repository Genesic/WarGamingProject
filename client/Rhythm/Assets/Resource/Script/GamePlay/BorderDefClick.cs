using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
public class BorderDefClick :  MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler {
    public ClickRes clickRes = ClickRes.Miss;
    public Text resText;
    
    //public void init(float x, float y)
    public void init()
    {
        // 移動到防禦位置
        //RectTransform rt = gameObject.GetComponent<RectTransform>();
        //Debug.Log("x:"+x+" y:"+y);       
        //rt.anchoredPosition = new Vector2(x, y);
        
        // 初始化點擊結果
        clickRes = ClickRes.Miss;
        resText.text = string.Empty;
        resText.color = Color.clear;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        clickRes = ClickRes.Perfect;
        gameObject.GetComponent<Image>().color = Color.clear;
        MainManager.socket.SendData("def_res [1]");
        StartCoroutine(showResText());
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
}
