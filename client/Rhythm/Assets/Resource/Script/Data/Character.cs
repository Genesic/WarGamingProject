using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour {
    public int id;
    public string cName;
    public int maxHp;
    public int maxMp;    
    public SkillBase[] skillList;
    public float bpm;    
    public Sprite headPic;
    public float getBeat(float tempo)
    {
        return (60/bpm) * tempo;
    }
}
