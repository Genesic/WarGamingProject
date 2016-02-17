using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClickButton : MonoBehaviour
{
    public Animator preClickAnime;
    int actHash = Animator.StringToHash("act");
    public GameFlow gameFlow;
    public bool isClick;
    public RectTransform targetObj;

    public Text result;
    string percect = "Perfect !";
    Color pColor = new Color32(255, 187, 0, 255);
    string great = "Great !";
    Color gColor = new Color32(0, 187, 0, 255);
    string miss = "miss !";
    Color mColor = Color.red;

    public void startAct()
    {
        isClick = false;
        preClickAnime.SetTrigger(actHash);
        result.text = string.Empty;
    }
    public void click()
    {
        // 一個click只判斷一次        
        if( isClick )
            return;
                    
        isClick = true;
        RectTransform preObj = gameObject.GetComponent<RectTransform>();
        float diff = preObj.rect.width - targetObj.rect.width;
        if (diff > 30)
            clickMiss();
        else if (diff > 10)
        {
            gameFlow.patchCombo(1);
            result.text = great;
            result.color = gColor;
        }
        else
        {
            gameFlow.patchCombo(1);
            result.text = percect;
            result.color = pColor;
        }
    }
    public void clickMiss()
    {
        gameFlow.clearCombo();
        result.text = miss;
        result.color = mColor;        
    }
}
