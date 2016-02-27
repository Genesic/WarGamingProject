using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterGroup : MonoBehaviour
{

    Dictionary<int, Character> list;

    void Awake()
    {
        init();
    }

    public void init()
    {
        list = new Dictionary<int, Character>();
        foreach (Transform child in transform)
        {
            var obj = child.gameObject.GetComponent<Character>();
            list.Add(obj.id, obj);
        }
    }

    public Character getCharacter(int id)
    {
        if (list.ContainsKey(id))
            return list[id];

        return null;
    }
}
