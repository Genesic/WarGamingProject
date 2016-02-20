using UnityEngine;
using System.Collections;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T> {
    public static T instance = null;
    
    protected virtual void Awake()
    {
        instance = this as T;
    }
    
    protected virtual void OnDestroy()
    {
        instance = null;
    }
}
