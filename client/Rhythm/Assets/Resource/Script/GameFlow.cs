using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameFlow : MonoBehaviour
{

    public RectTransform panelRt;
    public ClickButton clickObj;
    float len;
    public void init()
    {
        clearCombo();
    }

    private float border = 100;

    public float getBorder() { return border; }
    public float getWidthRange()
    {
        float width = panelRt.rect.width - border;
        return width;
    }
    public float getHeightRange()
    {
        float height = panelRt.rect.height - border;
        return height;
    }
    public Dictionary<string, float> getGameSet()
    {
        Dictionary<string, float> gameSet = new Dictionary<string, float>();
        gameSet.Add("width", getWidthRange());
        gameSet.Add("height", getHeightRange());
        gameSet.Add("border", getBorder());
        return gameSet;
    }

    public RectTransform up;
    public RectTransform down;
    public RectTransform right;
    public RectTransform left;

    float bpm = 90;
    void FixedUpdate()
    {
        // len += Time.deltaTime;
        float beat = (60 / bpm) * 4;
        if (len > beat)
        {
            clickObj.gameObject.SetActive(true);
            /*Debug.Log("up:"+up.position);
            Debug.Log("down:"+down.position);
            Debug.Log("right:"+right.position);
            Debug.Log("left:"+left.position); */
            float border = 100;
            float width = panelRt.rect.width - border;
            float height = panelRt.rect.height - border;
            RectTransform objRt = clickObj.gameObject.GetComponent<RectTransform>();
            //Debug.Log("width:"+width+" height:"+height);
            //Debug.Log("position:"+objRt.position);
            float newX = Random.Range(border, width);
            float newY = Random.Range(border, height);
            objRt.position = new Vector2(newX, newY);
            //Debug.Log("new position:"+objRt.position);                        
            clickObj.startAct();
            len -= beat;
        }
    }

    public int combo;
    public Text comboText;

    public void patchCombo(int patch)
    {
        combo += patch;
        if (combo > 1)
        {
            comboText.text = combo + " combo";
        }
        else
        {
            comboText.text = string.Empty;
        }
    }

    public void clearCombo()
    {
        combo = 0;
        patchCombo(0);
    }
}
