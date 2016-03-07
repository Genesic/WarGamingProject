using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillGroup : MonoBehaviour {
    public Dictionary<int, SkillBase> list;
    public Animator cameraAnime;
    void Awake()
    {
        init();
    }
    public void init()
    {
        list = new Dictionary<int, SkillBase>();
        foreach(Transform child in transform)
        {
            var obj = child.gameObject.GetComponent<SkillBase>();
            obj.setCameraAnime(cameraAnime);
            list.Add(obj.id, obj);
        }
        oriSelfHead = headPic.position;
        oriRivalHead = rivalPic.position;
    }
    
    public SkillBase getSkill(int skillId)
    {
        return list[skillId];
    }
    private float shake = 0f;
    float rivalShake = 0f;
    float shakeAmount = 15f;
    float decreaseFactor = 3f;
    public RectTransform headPic;
    public RectTransform rivalPic;
    private Vector2 oriSelfHead;
    private Vector2 oriRivalHead;
    public void startShakeHead(float sTime) { shake = sTime; }
    public void startShakeRivalHead(float sTime) {
        rivalShake = sTime; 
    }
    
    void FixedUpdate()
    {
        shakeHead();
        shakeRivalHead();        
    }
    void shakeHead(){
		if (shake > 0) {
			headPic.position = oriSelfHead + Random.insideUnitCircle * shakeAmount;
			shake -= Time.deltaTime * decreaseFactor;
		} else {
			shake = 0f;
			headPic.position = oriSelfHead;
		}
	}    
    void shakeRivalHead(){
		if (rivalShake > 0) {
			rivalPic.position = oriRivalHead + Random.insideUnitCircle * shakeAmount;
			rivalShake -= Time.deltaTime * decreaseFactor;
		} else {
			rivalShake = 0f;
			rivalPic.position = oriRivalHead;
		}
	}    
}
