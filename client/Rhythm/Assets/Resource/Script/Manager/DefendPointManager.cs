using UnityEngine;
using System.Collections;

public class DefendPointManager : ObjectPools<DefendPointManager, DefendPointBasic>
{
    protected override Transform ContainerTs { get { return this.transform; } }

    protected override void Awake() { base.Awake(); }

    protected override void OnDestroy() { base.OnDestroy(); }
    public GameObject defPointPrefab;
    
    public static void Retrieve(DefendPointBasic obj)
    {
        instance.InsRetrieve(obj);
    }
    
    public override DefendPointBasic CreateNew(string id)
    {
        // var path = string.Format("Prefab/DefendPoint");
        // var prefab = Resources.Load<GameObject>(path);
        var defPointGo = Instantiate(defPointPrefab);
        var defPointTs = defPointGo.transform;
        defPointTs.SetParent(ContainerTs);
        
        DefendPointBasic setting = defPointGo.GetComponent<DefendPointBasic>();
        setting.setParam(id);
        return setting;
    }
}
