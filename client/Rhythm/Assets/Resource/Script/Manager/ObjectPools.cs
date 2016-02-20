using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ObjectPools<T1, T2> : MonoSingleton<T1>
    where T1 : MonoSingleton<T1>
    where T2 : IPool  
{
    protected abstract Transform ContainerTs { get; }
    protected Dictionary<string, Queue<T2>> depotDict = new Dictionary<string, Queue<T2> >();
    
    protected override void Awake()
    {
        base.Awake();        
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
    
    public void InsRetrieve(T2 obj)
    {
        string id = obj.ID;
        
        if( !depotDict.ContainsKey(id) )
            depotDict[id] = new Queue<T2>();
            
        obj.SetDisable();
        var queue = depotDict[id];
        queue.Enqueue(obj);
    }
    
    public T2 Obtain(string id)
    {
        Queue<T2> queue;
        
        if( depotDict.TryGetValue(id, out queue))
        {
            if( queue.Count > 0 )
                return queue.Dequeue();
        }
        
        return CreateNew(id);
    }
    
    public abstract T2 CreateNew(string id);
}

public class IPool : MonoBehaviour
{
    protected string mID = string.Empty;
    
    public virtual string ID { get {return mID;} }
    
    public virtual void SetEnable()
    {
        gameObject.SetActive(true);
    }
    
    public virtual void SetDisable()
    {
        gameObject.SetActive(false);
        
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }
}