using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CharacterId
{
    Characer1 = 1,
    Characer2,
    Characer3,
    Characer4
}
public class Character : MonoBehaviour {
    private static Dictionary<int, string> ChName = new Dictionary<int, string>(){
        {1, "ch1"},
        {2, "ch2"},
        {3, "ch3"},
        {4, "ch4"}        
    };
    
    public static string getName(int id)
    {
        if( ChName.ContainsKey(id) )
            return ChName[id];
            
        return string.Empty;
    }
}
